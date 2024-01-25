// <copyright file="Lexer.Automata.SpanProcess.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

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
				private DfaRunNode _dfaRunNodePool;
				private bool _noOperandLexeme;

				public SpanProcess(TextSpan textSourceSpan, Lexer<TGrammar, TToken> lexer) : base(textSourceSpan)
				{
					_charMemory = textSourceSpan.AsMemory();
					_automata = lexer.Automata;
					_dfaBuilder = lexer.Automata._dfaBuilder;
					_dfaInitialState = _dfaBuilder.InitialState;
					_noOpDfaInitialState = _dfaBuilder.NoOpInitialState;

					Context = new LexerAutomataContext(lexer, textSourceSpan, _automata);
				}

				private LexerAutomataContext Context { get; }

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
						dfaState = dfaState.Builder.Build(1 + ch, dfaState);

					return dfaState;
				}

				public override void Dispose()
				{
				}

				private int EvalNoOperandLexeme(ref int instructionPointer, Lexeme<TToken>[] lexemesBuffer, int[] operands, bool skipLexemes, int iLexeme)
				{
					_noOperandLexeme = true;

					if (_noOpDfaInitialState == null)
						return iLexeme;

					SyncTextPointer(instructionPointer);

					var dfaState = _noOpDfaInitialState;

					for (;;)
					{
						dfaState = dfaState.EndState ?? dfaState.Builder.Build(0, dfaState);

						while (dfaState.Predicate != null)
						{
							dfaState = dfaState.EvalPredicates(Context, out var detachedState);
						}

						if (dfaState.Continue)
							continue;

						if (dfaState.Break)
							break;
					}

					if (dfaState.SuccessSubGraph != null && (skipLexemes == false || dfaState.Skip == false))
					{
						ref var lexeme = ref lexemesBuffer[iLexeme];

						lexeme.TokenField = dfaState.Token;
						lexeme.StartField = instructionPointer;
						lexeme.EndField = instructionPointer;
						operands[iLexeme] = dfaState.TokenCode;
						iLexeme++;
					}

					return iLexeme;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private static void FillLexeme(bool skipLexemes, LexerDfaState dfaState, int instructionPointer,
					int lexemeStart, ref int iLexeme, ref Lexeme<TToken> lexeme, ref int operand)
				{
					if (skipLexemes && dfaState.Skip)
						return;

					if (instructionPointer <= lexeme.EndField && (instructionPointer != lexeme.EndField || dfaState.TokenCode >= operand))
						return;

					lexeme.TokenField = dfaState.Token;
					lexeme.StartField = lexemeStart;
					lexeme.EndField = instructionPointer;
					operand = dfaState.TokenCode;
					iLexeme++;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private DfaRunNode GetRunNode(LexerDfaState state, int instructionPointer)
				{
					var node = _dfaRunNodePool;

					if (node == null)
						node = new DfaRunNode();
					else
					{
						_dfaRunNodePool = _dfaRunNodePool.Next;

						node.Next = null;
					}

					node.State = state;
					node.InstructionPointer = instructionPointer;

					return node;
				}


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void ReleaseRunNode(DfaRunNode node)
				{
					node.Next = _dfaRunNodePool;

					_dfaRunNodePool = node;
				}

				public override int Run(ref int startInstructionPointer, Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					if (_noOperandLexeme)
						_noOperandLexeme = startInstructionPointer >= _charMemory.Length;

					return _automata.HasPredicates
						? RunWithPredicates(ref startInstructionPointer, lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes)
						: RunWithoutPredicates(ref startInstructionPointer, lexemesBuffer, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private LexerDfaState RunDfaStateWithoutPredicate(ReadOnlySpan<char> span, LexerDfaState dfaState, ref int instructionPointer)
				{
					var slicedSpan = span.Slice(instructionPointer);
					var successInstructionPointer = instructionPointer;
					var index = -1;

					foreach (var ch in slicedSpan)
					{
						index++;

						var operand = ch + 1;

						dfaState = dfaState.TryGetState(operand) ?? BuildDfaState(dfaState, ch, slicedSpan, index);

						if (dfaState.Continue)
							continue;

						if (dfaState.Break)
							break;

						successInstructionPointer = index;
					}

					if (dfaState.Break == false)
					{
						SyncTextPointer(instructionPointer + index + 1);

						dfaState = dfaState.EndState ?? dfaState.Builder.Build(0, dfaState);
					}

					if (dfaState.SavePointer)
						successInstructionPointer = index;

					successInstructionPointer += instructionPointer + 1;

					instructionPointer = successInstructionPointer;

					return dfaState;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private LexerDfaState RunDfaStateWithPredicates(ReadOnlySpan<char> span, DfaRunNode dfaRunNode)
				{
					var instructionPointer = dfaRunNode.InstructionPointer;
					var successInstructionPointer = instructionPointer;
					var index = -1;
					var dfaState = dfaRunNode.State;

					if (dfaState.Continue)
					{
						var slicedSpan = span.Slice(instructionPointer);

						foreach (var ch in slicedSpan)
						{
							index++;

							var operand = ch + 1;

							dfaState = dfaState.TryGetState(operand) ?? BuildDfaState(dfaState, ch, slicedSpan, index);

							if (dfaState.Continue)
								continue;

							if (dfaState.Predicate != null)
							{
								SyncTextPointer(instructionPointer + index);

								while (dfaState.Predicate != null)
								{
									dfaState = dfaState.EvalPredicates(operand, Context, out var detachedState);

									if (detachedState.State != null)
										dfaRunNode.Next = GetRunNode(detachedState.State, detachedState.Position);
								}

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
					}

					if (dfaState.Break == false)
					{
						SyncTextPointer(instructionPointer + index + 1);

						dfaState = dfaState.EndState ?? dfaState.Builder.Build(0, dfaState);

						while (dfaState.Predicate != null)
						{
							dfaState = dfaState.EvalPredicates(Context, out var detachedState);

							if (detachedState.State != null)
								dfaRunNode.Next = GetRunNode(detachedState.State, detachedState.Position);
						}
					}

					if (dfaState.SavePointer)
						successInstructionPointer = index;

					successInstructionPointer += instructionPointer + 1;

					dfaRunNode.InstructionPointer = successInstructionPointer;

					return dfaState;
				}

				private int RunFastNoPredicateExperimental(ref int startInstructionPointer, Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					var iLexeme = lexemesBufferOffset;
					var instructionPointer = startInstructionPointer;
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

							var d = ch - LexerDfaState.ArraySentinel;
							var dfaIndex = ((d >> 31) & d) + LexerDfaState.ArraySentinel;

							current = current.Array[dfaIndex];

							switch (current.Switch)
							{
								case LexerDfaState.SwitchSave:

									lexemeEnd = index;

									break;

								case LexerDfaState.SwitchBreakTake:

									lexemeEnd++;

									goto switch_break_take;

								case LexerDfaState.SwitchBreakTakeSave:

									lexemeEnd = index + 1;

									goto switch_break_take;

								case LexerDfaState.SwitchBreakSkip:

									instructionPointer = lexemeEnd + 1;

									goto break_skip;

								case LexerDfaState.SwitchBreakSkipSave:

									instructionPointer = index + 1;

									goto break_skip;
							}
						}

						if (current.SavePointer)
							lexemeEnd = index + 1;

						if (current.SuccessSubGraph == null)
						{
							startInstructionPointer = instructionPointer;

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

					startInstructionPointer = instructionPointer;

					if (iLexeme < lexemesBufferSize && instructionPointer == span.Length && _noOperandLexeme == false)
						iLexeme = EvalNoOperandLexeme(ref startInstructionPointer, lexemesBuffer, operands, skipLexemes, iLexeme);

					return iLexeme - lexemesBufferOffset;
				}

				private int RunWithoutPredicates(ref int startInstructionPointer, Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					var iLexeme = lexemesBufferOffset;
					var instructionPointer = startInstructionPointer;
					var lexemeStart = instructionPointer;
					var span = _charMemory.Span;

					while (iLexeme < lexemesBufferSize && instructionPointer < span.Length)
					{
						ref var lexeme = ref lexemesBuffer[iLexeme];
						ref var operand = ref operands[iLexeme];

						lexeme = default;
						operand = default;

						var dfaState = RunDfaStateWithoutPredicate(span, _dfaInitialState, ref instructionPointer);

						if (dfaState.SuccessSubGraph != null)
							FillLexeme(skipLexemes, dfaState, instructionPointer, lexemeStart, ref iLexeme, ref lexeme, ref operand);
						else
						{
							startInstructionPointer = instructionPointer;

							return iLexeme - lexemesBufferOffset;
						}

						lexemeStart = instructionPointer;
					}

					startInstructionPointer = instructionPointer;

					if (iLexeme < lexemesBufferSize && instructionPointer == span.Length && _noOperandLexeme == false)
						iLexeme = EvalNoOperandLexeme(ref startInstructionPointer, lexemesBuffer, operands, skipLexemes, iLexeme);

					return iLexeme - lexemesBufferOffset;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private int RunWithPredicates(ref int startInstructionPointer, Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
				{
					var iLexeme = lexemesBufferOffset;
					var instructionPointer = startInstructionPointer;
					var lexemeStart = instructionPointer;
					var span = _charMemory.Span;

					while (iLexeme < lexemesBufferSize && instructionPointer < span.Length)
					{
						var dfaRunNode = GetRunNode(_dfaInitialState, instructionPointer);
						ref var lexeme = ref lexemesBuffer[iLexeme];
						ref var operand = ref operands[iLexeme];

						lexeme = default;
						operand = default;

						var runNext = false;

						while (dfaRunNode != null)
						{
							var dfaState = RunDfaStateWithPredicates(span, dfaRunNode);

							instructionPointer = dfaRunNode.InstructionPointer;

							var currentRunNode = dfaRunNode;

							dfaRunNode = dfaRunNode.Next;

							ReleaseRunNode(currentRunNode);

							if (dfaState.SuccessSubGraph == null)
								continue;

							FillLexeme(skipLexemes, dfaState, instructionPointer, lexemeStart, ref iLexeme, ref lexeme, ref operand);

							runNext = true;
						}

						if (runNext == false)
							return iLexeme - lexemesBufferOffset;

						lexemeStart = instructionPointer;
					}

					startInstructionPointer = instructionPointer;

					if (iLexeme < lexemesBufferSize && instructionPointer == span.Length && _noOperandLexeme == false)
						iLexeme = EvalNoOperandLexeme(ref startInstructionPointer, lexemesBuffer, operands, skipLexemes, iLexeme);

					return iLexeme - lexemesBufferOffset;
				}

				private void SyncTextPointer(int index)
				{
					Context.Position = index;
				}

				private sealed class DfaRunNode
				{
					public int InstructionPointer;
					public DfaRunNode Next;
					public LexerDfaState State;
				}
			}
		}
	}
}