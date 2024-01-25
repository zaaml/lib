// <copyright file="Automata.DfaState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

// ReSharper disable VirtualMemberCallInConstructor

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		

		private protected partial class LexerAutomata
		{
			private readonly struct LexerDetachedDfaState
			{
				public LexerDetachedDfaState(int position, LexerDfaState state)
				{
					Position = position;
					State = state;
				}

				public readonly int Position;
				public readonly LexerDfaState State;
			}

			private sealed class LexerDfaState
			{
				public const int ArrayLimit = 130;
				public const int ArraySentinel = 129;


				public const int SwitchReturn = 6;
				public const int SwitchSave = 1;
				public const int SwitchBreakSkip = 2;
				public const int SwitchBreakSkipSave = 3;
				public const int SwitchBreakTake = 4;
				public const int SwitchBreakTakeSave = 5;


				public const int EndSwitchTake = 0;
				public const int EndSwitchSkip = 1;


				private static readonly LexerDfaState EmptyState = new(false);
				private static readonly LexerDfaState SentinelState = new(true);

				public readonly LexerDfaState[] Array;
				public readonly bool Break;
				public readonly LexerDfaBuilder Builder;
				public readonly bool Continue;
				public readonly bool Sentinel;
				public readonly LexerDfaDictionary<LexerDfaState> Dictionary;
				public readonly int EndSwitch;
				public readonly LexerDfaState[] FalsePredicateArray;
				public readonly Dictionary<int, LexerDfaState> FalsePredicateDictionary;
				public readonly int HashCode;
				public readonly int Index;
				public readonly LexerDfaTransition[] LazyTransitions;
				public readonly LexerDfaNode[] Nodes;
				public readonly LexerDfaState NullSaveState;
				public readonly LexerDfaState NullState;
				public readonly object Predicate;
				public readonly LexerDfaTransition PrevSuccessTransition;
				public readonly bool SavePointer;
				public readonly bool Skip;
				public readonly SubGraph SuccessSubGraph;
				public readonly LexerDfaTransition SuccessTransition;
				public readonly int Switch;

				public readonly TToken Token;
				public readonly int TokenCode;
				public readonly LexerDfaState[] TruePredicateArray;
				public readonly Dictionary<int, LexerDfaState> TruePredicateDictionary;

				public readonly LexerDfaState EndState = null;
				public LexerDfaState FalsePredicateEndState;
				public bool FastNoNext;
				public LexerDfaState TruePredicateEndState;


				private LexerDfaState(bool sentinel)
				{
					Sentinel = sentinel;
				}

				private LexerDfaState(LexerDfaState state, bool save)
				{
					Nodes = state.Nodes;
					LazyTransitions = state.LazyTransitions;

					HashCode = state.HashCode;
					SuccessSubGraph = state.PrevSuccessTransition?.SubGraph;
					Skip = GetSkip(SuccessSubGraph);
					Break = true;
					Builder = state.Builder;
					SavePointer = save;
					Index = LexerDfaBuilder.GetNullIndex(save, state.Index);

					SetBreakContinue(out Switch, out EndSwitch);

					if (SuccessSubGraph == null)
						return;

					var lexerRule = (LexerSyntax)SuccessSubGraph.Syntax;

					Token = lexerRule.Token;
					TokenCode = lexerRule.TokenCode;
				}

				private static bool ShouldBreak(LexerDfaNode[] nodes, LexerDfaTransition successTransition)
				{
					if (nodes.Length != 1 || successTransition == null) 
						return false;

					var dfaNode = nodes[0];
					var node = dfaNode.Node;

					if (node is ReturnSyntaxNode or EndProductionNode or ExitSyntaxNode)
						return true;
						
					if (node.ExecutionPaths.Count(e => e.OutputReturn == false && e.OutputEnd == false) == 0)
						return true;

					return false;
				}

				private static bool ShouldBreakOld(LexerDfaNode[] nodes, LexerDfaTransition successTransition)
				{
					if (nodes.Length != 1 || successTransition == null) 
						return false;

					var dfaNode = nodes[0];
					var node = dfaNode.Node;

					return (node is ReturnSyntaxNode || node.HasReturn == false) && node.ExecutionPaths.Length == 0;
				}

				public LexerDfaState(LexerDfaNode[] nodes, LexerDfaTransition[] lazyTransitions, LexerDfaTransition successTransition, LexerDfaTransition prevSuccessTransition, int hashCode, LexerDfaBuilder builder)
				{
					Index = builder.Register(this);
					Array = new LexerDfaState[ArrayLimit];
					Array[ArraySentinel] = SentinelState;
					Dictionary = new LexerDfaDictionary<LexerDfaState>();
					Builder = builder;
					Nodes = nodes;
					LazyTransitions = lazyTransitions;
					HashCode = hashCode;
					SuccessTransition = successTransition;
					PrevSuccessTransition = prevSuccessTransition;
					SuccessSubGraph = SuccessTransition?.SubGraph;

					if (SuccessSubGraph != null)
						SavePointer = true;

					Break = ShouldBreak(nodes, successTransition);
					Skip = GetSkip(SuccessSubGraph);

					Predicate = Nodes.FirstOrDefault(a => a.ExecutionPath != null)?.ExecutionPath;

					if (Predicate != null)
					{
						TruePredicateArray = builder.FastLookup ? new LexerDfaState[ArrayLimit] : System.Array.Empty<LexerDfaState>();
						TruePredicateDictionary = new Dictionary<int, LexerDfaState>();

						FalsePredicateArray = builder.FastLookup ? new LexerDfaState[ArrayLimit] : System.Array.Empty<LexerDfaState>();
						FalsePredicateDictionary = new Dictionary<int, LexerDfaState>();
					}

					NullState = CreateNullState(false);
					NullSaveState = CreateNullState(true);

					if (SavePointer == false && Break == false && Predicate == null)
						Continue = true;

					SetBreakContinue(out Switch, out EndSwitch);

					if (SuccessSubGraph == null)
						return;

					var lexerStateRule = (LexerSyntax)SuccessSubGraph.Syntax;

					Token = lexerStateRule.Token;
					TokenCode = lexerStateRule.TokenCode;
				}

				public LexerDfaState EvalPredicates(int operand, AutomataContext context, out LexerDetachedDfaState detachedState)
				{
					var lexerContext = (LexerAutomataContext)context;
					var prevPosition = lexerContext.Position;
					var predicate = ((ExecutionPath)Predicate).Predicate;
					var predicateResult = predicate.PassInternal(context) != null;
					var nextPosition = lexerContext.Position;

					detachedState = default;

					if (prevPosition != nextPosition)
					{
						if (predicateResult == false)
							throw new InvalidOperationException("Predicate can change position with success execution result.");

						var predicateNode = Nodes.First(n => n.ExecutionPath != null);
						
						detachedState = new LexerDetachedDfaState(nextPosition, Builder.GetDetachedState(this, predicateNode));

						lexerContext.Position = prevPosition;
						predicateResult = false;
					}

					return TryGetPredicateState(operand, predicateResult) ?? Builder.BuildPredicateState(operand, this, predicateResult);
				}

				public LexerDfaState EvalPredicates(AutomataContext context, out LexerDetachedDfaState detachedState)
				{
					var predicate = ((ExecutionPath)Predicate).Predicate;
					var predicateResult = predicate.PassInternal(context) != null;

					detachedState = default;

					return TryGetPredicateState(predicateResult) ?? Builder.BuildPredicateState(0, this, predicateResult);
				}

				private void SetBreakContinue(out int flowSwitch, out int flowEndSwitch)
				{
					flowSwitch = SwitchReturn;
					flowEndSwitch = 0;

					if (SavePointer)
						flowSwitch = SwitchSave;

					if (Break)
					{
						if (Skip)
							flowSwitch = SavePointer ? SwitchBreakSkipSave : SwitchBreakSkip;
						else
							flowSwitch = SavePointer ? SwitchBreakTakeSave : SwitchBreakTake;
					}

					if (SuccessSubGraph != null)
						flowEndSwitch = Skip ? EndSwitchSkip : EndSwitchTake;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void SetPredicateState(int operand, LexerDfaState state, bool predicateResult)
				{
					if (predicateResult)
					{
						if (operand < TruePredicateArray.Length)
							TruePredicateArray[operand] = state;
						else
							TruePredicateDictionary[operand] = state;
					}
					else
					{
						if (operand < FalsePredicateArray.Length)
							FalsePredicateArray[operand] = state;
						else
							FalsePredicateDictionary[operand] = state;
					}
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void SetPredicateState(LexerDfaState state, bool predicateResult)
				{
					if (predicateResult)
						TruePredicateEndState = state;
					else
						FalsePredicateEndState = state;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void SetState(int operand, LexerDfaState state)
				{
					if (operand < ArraySentinel)
						Array[operand] = state;
					else
						Dictionary.Add(operand, state);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LexerDfaState TryGetPredicateState(int operand, bool predicateResult)
				{
					if (predicateResult)
					{
						if (operand < TruePredicateArray.Length)
							return TruePredicateArray[operand];

						return TruePredicateDictionary.TryGetValue(operand, out var result) ? result : null;
					}
					else
					{
						if (operand < FalsePredicateArray.Length)
							return FalsePredicateArray[operand];

						return FalsePredicateDictionary.TryGetValue(operand, out var result) ? result : null;
					}
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LexerDfaState TryGetPredicateState(bool predicateResult)
				{
					return predicateResult ? TruePredicateEndState : FalsePredicateEndState;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LexerDfaState TryGetState(int operand)
				{
					return operand < ArraySentinel ? Array[operand] : Dictionary.GetValueOrDefault(operand);
				}

				private LexerDfaState CreateNullState(bool save)
				{
					return new LexerDfaState(this, save);
				}

				private bool GetSkip(SubGraph subGraph)
				{
					if (subGraph == null)
						return false;

					var lexerSyntax = (LexerSyntax)SuccessSubGraph.Syntax;

					return lexerSyntax.IsTrivia;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static int ReadFastNoPredicate(LexerDfaState initial, ref ReadOnlySpan<char> span, ref int instructionPointer, ref Lexeme<TToken> lexeme, ref int operand)
				{
					break_skip:

					var lexemeEnd = instructionPointer;
					var current = initial;
					var index = instructionPointer - 1;

					for (var i = instructionPointer; i < span.Length; i++)
					{
						var ch = span[i];

						index++;

						var d = ch - ArraySentinel;
						var dfaIndex = ((d >> 31) & d) + ArraySentinel;

						current = current.Array[dfaIndex];

						switch (current.Switch)
						{
							case SwitchSave:

								lexemeEnd = index;

								break;

							case SwitchBreakTake:

								lexemeEnd++;

								goto switch_break_take;

							case SwitchBreakTakeSave:

								lexemeEnd = index + 1;

								goto switch_break_take;

							case SwitchBreakSkip:

								instructionPointer = lexemeEnd + 1;

								goto break_skip;

							case SwitchBreakSkipSave:

								instructionPointer = index + 1;

								goto break_skip;
						}
					}

					throw new InvalidOperationException("Impossible flow.");

					switch_break_take:

					operand = current.TokenCode;
					lexeme.TokenField = current.Token;
					lexeme.StartField = instructionPointer;
					lexeme.EndField = lexemeEnd;

					instructionPointer = lexemeEnd;

					return 1;
				}
			}
		}
	}
}