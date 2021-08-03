// <copyright file="Lexer.Automata.SpanProcess.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		#region Nested Types

		private protected partial class LexerAutomata
		{
			#region Nested Types

			public sealed class SpanProcess : LexerProcess
			{
				#region Fields

				private readonly ReadOnlyMemory<char> _charMemory;
				private readonly LexerDfaState _dfaInitialState;
				private readonly LexerDfaState _noOpDfaInitialState;
				private int _instructionPointer;
				private bool _noOperandLexeme;

				#endregion

				#region Ctors

				public SpanProcess(TextSpan textSourceSpan, Lexer<TGrammar, TToken> lexer, LexerContext<TToken> lexerContext) : base(textSourceSpan)
				{
					_charMemory = textSourceSpan.AsMemory();
					_dfaInitialState = lexer.Automata._dfaBuilder.InitialState;
					_noOpDfaInitialState = lexer.Automata._dfaBuilder.NoOpInitialState;

					Context = new LexerAutomataContext();

					Context.Mount(textSourceSpan, lexerContext);
				}

				#endregion

				#region Properties

				private LexerAutomataContext Context { get; }

				public override int TextPointer
				{
					get => _instructionPointer;
					set => _instructionPointer = value;
				}

				#endregion

				#region Methods

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
						dfaState = dfaState.EndState ?? dfaState.Builder.Build(dfaState);

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

				private static LexerDfaState GetDfaState(LexerDfaState dfaState, char ch, ReadOnlySpan<char> slicedSpan, int index)
				{
					if (dfaState.Dictionary.TryGetValue(ch, out var result))
						dfaState = result;
					else
					{
						if (char.IsHighSurrogate(ch))
							dfaState.SetState(ch, dfaState);
						else if (char.IsLowSurrogate(ch))
						{
							var prevState = dfaState;

							dfaState = dfaState.Builder.Build(char.ConvertToUtf32(slicedSpan[index - 1], ch), dfaState);
							prevState.SetState(ch, dfaState);
						}
						else
							dfaState = dfaState.Builder.Build(ch, dfaState);
					}

					return dfaState;
				}

				private static LexerDfaState GetBuildDfaState(LexerDfaState dfaState, char ch, ReadOnlySpan<char> slicedSpan, int index)
				{
					if (char.IsHighSurrogate(ch))
						dfaState.SetState(ch, dfaState);
					else if (char.IsLowSurrogate(ch))
					{
						var prevState = dfaState;

						dfaState = dfaState.Builder.Build(char.ConvertToUtf32(slicedSpan[index - 1], ch), dfaState);
						prevState.SetState(ch, dfaState);
					}
					else
						dfaState = dfaState.Builder.Build(ch, dfaState);

					return dfaState;
				}

				public override int Run(Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					if (_noOperandLexeme)
						_noOperandLexeme = _instructionPointer >= _charMemory.Length;

					if (_dfaInitialState.Builder.FastLookup)
					{
						return RunFast(lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);
					}

					return RunSlow(lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);
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

							var array = dfaState.Array;

							dfaState = ch < array.Length ? array[ch] : GetDfaState(dfaState, ch, slicedSpan, index);

							if (dfaState.Continue)
								continue;

							if (dfaState.Predicate != null)
							{
								SyncTextPointer(index);

								while (dfaState.Predicate != null)
									dfaState = dfaState.EvalPredicates(ch, Context);

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

							dfaState = dfaState.EndState ?? dfaState.Builder.Build(dfaState);

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

							dfaState = dfaState.Dictionary.TryGetValue(ch, out var result) ? result : GetBuildDfaState(dfaState, ch, slicedSpan, index);

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

							dfaState = dfaState.EndState ?? dfaState.Builder.Build(dfaState);

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

				#endregion
			}



			#endregion
		}

		#endregion
	}
}