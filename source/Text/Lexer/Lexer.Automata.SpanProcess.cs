// <copyright file="Lexer.Automata.SpanProcess.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			public sealed class SpanProcess : LexerProcess
			{
				private readonly LexerAutomata _automata;
				private readonly ReadOnlyMemory<char> _charMemory;
				private readonly LexerDfaState _dfaInitialState;
				private readonly LexerDfaState _noOpDfaInitialState;
				private LexerDfaBuilder _dfaBuilder;
				private int _instructionPointer;
				private bool _noOperandLexeme;

				public SpanProcess(TextSpan textSourceSpan, Lexer<TGrammar, TToken> lexer, LexerContext<TToken> lexerContext) : base(textSourceSpan)
				{
					_charMemory = textSourceSpan.AsMemory();
					_automata = lexer.Automata;
					_dfaBuilder = lexer.Automata._dfaBuilder;
					_dfaInitialState = _dfaBuilder.InitialState;
					_noOpDfaInitialState = _dfaBuilder.NoOpInitialState;

					Context = new LexerAutomataContext(_automata);

					Context.Mount(textSourceSpan, lexerContext);
				}

				private LexerAutomataContext Context { get; }

				public override int TextPointer
				{
					get => _instructionPointer;
					set => _instructionPointer = value;
				}

				private static LexerDfaState BuildDfaState(LexerDfaState dfaState, char ch, ReadOnlySpan<char> slicedSpan, int index)
				{
					if (char.IsHighSurrogate(ch))
						dfaState.SetState(ch + 1, dfaState);
					else if (char.IsLowSurrogate(ch))
					{
						var prevState = dfaState;

						dfaState = dfaState.Builder.Build(1 + char.ConvertToUtf32(slicedSpan[index - 1], ch), dfaState);
						prevState.SetState(ch + 1, dfaState);
					}
					else
					{
						dfaState = dfaState.Builder.Build(1 + ch, dfaState);
					}

					return dfaState;
				}

				public override void Dispose()
				{
				}

				private int EvalNoOperandLexeme(Lexeme<TToken>[] lexemesBuffer, int[] operands, bool skipLexemes, int iLexeme)
				{
					_noOperandLexeme = true;

					if (_noOpDfaInitialState == null)
						return iLexeme;

					SyncTextPointer(_instructionPointer);

					var dfaState = _noOpDfaInitialState;

					for (;;)
					{
						dfaState = dfaState.EndState ?? dfaState.Builder.Build(0, dfaState);

						while (dfaState.Predicate != null)
							dfaState = dfaState.EvalPredicates(Context);

						if (dfaState.Continue)
							continue;

						if (dfaState.Break)
							break;
					}

					if (dfaState.SuccessSubGraph != null && (skipLexemes == false || dfaState.Skip == false))
					{
						ref var lexeme = ref lexemesBuffer[iLexeme];

						lexeme.TokenField = dfaState.Token;
						lexeme.StartField = _instructionPointer;
						lexeme.EndField = _instructionPointer;
						operands[iLexeme] = dfaState.TokenCode;
						iLexeme++;
					}

					return iLexeme;
				}

				public override int Run(Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					if (_noOperandLexeme)
						_noOperandLexeme = _instructionPointer >= _charMemory.Length;

					if (_automata._dfaBuilder.FastLookup == false)
						return RunSlow(lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);

					//return _automata.HasPredicates ? RunFast(lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes) : RunFastNoPredicate(lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);
					return RunFast(lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);
				}

				private int RunFast(Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					var iLexeme = lexemesBufferOffset;
					var instructionPointer = _instructionPointer;
					var lexemeStart = instructionPointer;
					var span = _charMemory.Span;

					while (iLexeme < lexemesBufferSize && instructionPointer < span.Length)
					{
						var dfaState = _dfaInitialState;
						var successInstructionPointer = instructionPointer;
						var slicedSpan = span.Slice(instructionPointer);
						var index = -1;

						foreach (var ch in slicedSpan)
						{
							index++;

							var operand = ch + 1;
							var dfaIndex = DfaState.ArraySentinel + (((operand - DfaState.ArraySentinel) >> 31) & (operand - DfaState.ArraySentinel));

							dfaState = dfaState.Array[dfaIndex];

							if (dfaState.Continue)
								continue;

							if (dfaState.Predicate != null)
							{
								SyncTextPointer(index);

								while (dfaState.Predicate != null)
									dfaState = dfaState.EvalPredicates(operand, Context);

								if (dfaState.Continue)
									continue;

								if (dfaState.Break)
								{
									index--;

									break;
								}
							}

							if (dfaState.Break)
								break;

							successInstructionPointer = index;
						}

						if (dfaState.Break == false)
						{
							SyncTextPointer(index + 1);

							dfaState = dfaState.EndState ?? dfaState.Builder.Build(0, dfaState);

							while (dfaState.Predicate != null)
								dfaState = dfaState.EvalPredicates(Context);
						}

						if (dfaState.SavePointer)
							successInstructionPointer = index;

						if (dfaState.SuccessSubGraph == null)
						{
							_instructionPointer = instructionPointer;

							return iLexeme - lexemesBufferOffset;
						}

						successInstructionPointer += instructionPointer + 1;

						if (skipLexemes == false || dfaState.Skip == false)
						{
							ref var lexeme = ref lexemesBuffer[iLexeme];

							lexeme.TokenField = dfaState.Token;
							lexeme.StartField = lexemeStart;
							lexeme.EndField = successInstructionPointer;

							operands[iLexeme] = dfaState.TokenCode;
							iLexeme++;
						}

						lexemeStart = successInstructionPointer;
						instructionPointer = successInstructionPointer;
					}

					_instructionPointer = instructionPointer;

					if (iLexeme < lexemesBufferSize && instructionPointer == span.Length && _noOperandLexeme == false)
						iLexeme = EvalNoOperandLexeme(lexemesBuffer, operands, skipLexemes, iLexeme);

					return iLexeme - lexemesBufferOffset;
				}

				private int RunFastNoPredicate(Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					var iLexeme = lexemesBufferOffset;
					var instructionPointer = _instructionPointer;
					var span = _charMemory.Span;
					var initial = _dfaInitialState;

					while (iLexeme < lexemesBufferSize && instructionPointer < span.Length)
					{
						//iLexeme += LexerDfaState.ReadFastNoPredicate(initial, ref span, ref instructionPointer, ref lexemesBuffer[iLexeme], ref operands[iLexeme]);

						ref var lexeme = ref lexemesBuffer[iLexeme];
						ref var operand = ref operands[iLexeme];

						break_skip:

						var lexemeEnd = instructionPointer;
						var current = initial;
						var index = instructionPointer - 1;

						for (var i = instructionPointer; i < span.Length; i++)
						{
							var ch = span[i];

							index++;

							var d = ch - DfaState.ArraySentinel;
							var dfaIndex = ((d >> 31) & d) + DfaState.ArraySentinel;

							current = current.Array[dfaIndex];

							switch (current.Switch)
							{
								case DfaState.SwitchSave:

									lexemeEnd = index;

									break;

								case DfaState.SwitchBreakTake:

									lexemeEnd++;

									goto switch_break_take;

								case DfaState.SwitchBreakTakeSave:

									lexemeEnd = index + 1;

									goto switch_break_take;

								case DfaState.SwitchBreakSkip:

									instructionPointer = lexemeEnd + 1;

									goto break_skip;

								case DfaState.SwitchBreakSkipSave:

									instructionPointer = index + 1;

									goto break_skip;
							}
						}

						if (current.SavePointer)
							lexemeEnd = index + 1;

						if (current.SuccessSubGraph == null)
						{
							_instructionPointer = instructionPointer;

							return iLexeme - lexemesBufferOffset;
						}

						switch_break_take:

						operand = current.TokenCode;
						lexeme.TokenField = current.Token;
						lexeme.StartField = instructionPointer;
						lexeme.EndField = lexemeEnd;

						instructionPointer = lexemeEnd;
						iLexeme++;
					}

					_instructionPointer = instructionPointer;

					if (iLexeme < lexemesBufferSize && instructionPointer == span.Length && _noOperandLexeme == false)
						iLexeme = EvalNoOperandLexeme(lexemesBuffer, operands, skipLexemes, iLexeme);

					return iLexeme - lexemesBufferOffset;
				}

				private int RunSlow(Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					var iLexeme = lexemesBufferOffset;
					var instructionPointer = _instructionPointer;
					var lexemeStart = instructionPointer;
					var span = _charMemory.Span;

					while (iLexeme < lexemesBufferSize && instructionPointer < span.Length)
					{
						var dfaState = _dfaInitialState;
						var successInstructionPointer = instructionPointer;
						var slicedSpan = span.Slice(instructionPointer);
						var index = -1;

						foreach (var ch in slicedSpan)
						{
							index++;

							var operand = ch + 1;

							dfaState = dfaState.Dictionary.TryGetValue(operand, out var result) ? result : BuildDfaState(dfaState, ch, slicedSpan, index);

							if (dfaState.Continue)
								continue;

							if (dfaState.Predicate != null)
							{
								SyncTextPointer(instructionPointer + index);

								while (dfaState.Predicate != null)
									dfaState = dfaState.EvalPredicates(ch, Context);

								if (dfaState.Continue)
									continue;
							}

							if (dfaState.Break)
								break;

							successInstructionPointer = index;
						}

						if (dfaState.Break == false)
						{
							SyncTextPointer(index + 1);

							dfaState = dfaState.EndState ?? dfaState.Builder.Build(0, dfaState);

							while (dfaState.Predicate != null)
								dfaState = dfaState.EvalPredicates(Context);
						}

						if (dfaState.SavePointer)
							successInstructionPointer = index;

						if (dfaState.SuccessSubGraph == null)
						{
							_instructionPointer = instructionPointer;

							return iLexeme - lexemesBufferOffset;
						}

						successInstructionPointer += instructionPointer + 1;

						if (skipLexemes == false || dfaState.Skip == false)
						{
							ref var lexeme = ref lexemesBuffer[iLexeme];

							lexeme.TokenField = dfaState.Token;
							lexeme.StartField = lexemeStart;
							lexeme.EndField = successInstructionPointer;

							operands[iLexeme] = dfaState.TokenCode;
							iLexeme++;
						}

						lexemeStart = successInstructionPointer;
						instructionPointer = successInstructionPointer;
					}

					_instructionPointer = instructionPointer;

					if (iLexeme < lexemesBufferSize && instructionPointer == span.Length && _noOperandLexeme == false)
						iLexeme = EvalNoOperandLexeme(lexemesBuffer, operands, skipLexemes, iLexeme);

					return iLexeme - lexemesBufferOffset;
				}

				private void SyncTextPointer(int index)
				{
					if (Context.LexerContext != null)
						Context.LexerContext.TextPointer = index;
				}
			}
		}
	}
}