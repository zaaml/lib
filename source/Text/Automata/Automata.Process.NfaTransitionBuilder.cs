// <copyright file="Automata.Process.Thread.Dfa.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;
using Zaaml.Core.Pools;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private sealed class NfaTransitionBuilder : TransitionBuilder, IDisposable
			{
				private const int LookaheadDepth = 4;
				private DfaBranchNode[] _branches = new DfaBranchNode[32];
				private readonly StackPool<DfaBranch> _branchesPool = new();
				private readonly StackPool<DfaBranchNode> _branchNodesPool = new();
				private readonly DfaBuilder _dfaBuilder;
				private readonly Pool<NfaTransitionBuilder> _pool;
				private DfaBranchNode[] _nfa1 = new DfaBranchNode[32];
				private DfaBranchNode[] _nfa2 = new DfaBranchNode[32];

				public NfaTransitionBuilder(Automata<TInstruction, TOperand> automata, Pool<NfaTransitionBuilder> pool)
				{
					_dfaBuilder = automata._dfaBuilderInstance;
					_pool = pool;
				}

				public void Dispose()
				{
					_pool.Return(this);
				}

				public override void Build(ref Thread thread, ref ThreadContext context, ExecutionRailBuilder transitionBuilder)
				{
					var node = thread.Node;
					var stack = thread.Stack;
					ref var dfa = ref thread.Dfa;

					node.EnsureSafe();

					var operand = context.FetchInstructionOperand;

					dfa ??= node.Dfa;
					dfa = dfa.Expand(stack.Span);

					var nfa = dfa.GetNfa(operand);

					if (nfa.ExecutionRailNode != null)
					{
						transitionBuilder.Unfork(nfa.ExecutionRailNode, 1);

						return;
					}

					if (nfa.Transitions.Length == 0)
						return;

					if (TrieLookahead(ref context, nfa, transitionBuilder))
						return;

					NfaLookahead(ref context, nfa, dfa, transitionBuilder);
				}

				private void NfaLookahead(ref ThreadContext context, NfaState nfaState, DfaState dfa, ExecutionRailBuilder transitionBuilder)
				{
					var instructionStream = context.InstructionStream;
					var instructionStreamPointer = context.InstructionStreamPointer + 1;
					var headNode = DfaBranchNode.Empty;
					var trieNode = nfaState.Trie.RootNode;
					var transitionsLength = nfaState.Transitions.Length;

					if (transitionsLength == 0)
					{
						trieNode.Value = NfaExecution.Empty;
						transitionBuilder.Unfork(trieNode.Value.ExecutionRailList);

						return;
					}

					if (transitionsLength == 1)
					{
						transitionBuilder.AddExecutionRail(nfaState.Transitions[0].ExecutionRail);

						var nextDfa = dfa.GetNextDfa(transitionBuilder.ExecutionRail);

						trieNode.Value = new NfaExecution(transitionBuilder.Fork(dfa), nextDfa);
						transitionBuilder.Unfork(trieNode.Value.ExecutionRailList);

						return;
					}

					trieNode.Value = NfaExecution.Next;

					ArrayUtils.EnsureArrayLength(ref _branches, transitionsLength);
					ArrayUtils.EnsureArrayLength(ref _nfa1, transitionsLength);
					
					for (var index = 0; index < transitionsLength; index++)
					{
						var transition = nfaState.Transitions[index];
						var branch = _branchesPool.Rent();
						var branchNode = _branchNodesPool.Rent();

						branch.Succeeded = false;
						branch.ReferenceCount = 1;

						branchNode.Branch = branch;
						branchNode.Transition = transition;
						branchNode.Parent = null;

						branchNode.Next = headNode;
						headNode = branchNode;

						_branches[index] = _nfa1[index] = branchNode;
					}

					var branchesCount = nfaState.Transitions.Length;
					var nfaLen = nfaState.Transitions.Length;
					var depth = 0;

					while (depth < LookaheadDepth)
					{
						depth++;

						var operand = instructionStream.FetchReadOperand(ref instructionStreamPointer);
						var nextNfaHead = 0;
						var minTargetDfaIndex = int.MaxValue;
						var maxTargetDfaIndex = int.MinValue;

						trieNode = trieNode.GetNextOrCreate(operand);

						for (var j = 0; j < nfaLen; j++)
						{
							var branchNode = _nfa1[j];
							var branch = branchNode.Branch;

							if (branch.Succeeded)
							{
								branch.ReferenceCount--;

								continue;
							}

							var dfaTransition = branchNode.Transition;
							var currentDfa = dfaTransition.TargetDfa;

							if (currentDfa.Closed == false)
							{
								branch.Succeeded = true;
								branch.ReferenceCount--;
							}
							else
							{
								var nextNfa = currentDfa.GetNfa(operand);
								var nfaTransitions = nextNfa.Transitions;

								branch.ReferenceCount--;

								foreach (var nextTransition in nfaTransitions)
								{
									var nextBranchNode = _branchNodesPool.Rent();
									var nextTransitionTarget = nextTransition.TargetDfa;
									var nextTransitionTargetIndex = nextTransitionTarget.Index;

									nextBranchNode.Branch = branch;
									nextBranchNode.Transition = nextTransition;
									nextBranchNode.Parent = branchNode;
									nextBranchNode.Next = headNode;

									headNode = nextBranchNode;

									ArrayUtils.EnsureArrayLength(ref _nfa2, nextNfaHead + 1, true);

									_nfa2[nextNfaHead++] = nextBranchNode;

									branch.ReferenceCount++;

									minTargetDfaIndex = minTargetDfaIndex < nextTransitionTargetIndex ? minTargetDfaIndex : nextTransitionTargetIndex;
									maxTargetDfaIndex = maxTargetDfaIndex > nextTransitionTargetIndex ? maxTargetDfaIndex : nextTransitionTargetIndex;
								}
							}

							if (branch.ReferenceCount == 0)
								branchesCount--;
						}

						nfaLen = nextNfaHead;
						(_nfa1, _nfa2) = (_nfa2, _nfa1);

						if (operand == 0 || branchesCount < 2)
							break;

						if (minTargetDfaIndex == maxTargetDfaIndex)
							break;

						trieNode.Value = NfaExecution.Next;
					}

					var prevRail = MemorySpan<int>.Empty;

					for (var i = 0; i < transitionsLength; i++)
					{
						var branchNode = _branches[i];

						if (branchNode.Branch.Succeeded || branchNode.Branch.ReferenceCount > 0)
						{
							if (prevRail.IsEmpty || prevRail.SpanSafe.SequenceEqual(branchNode.Transition.ExecutionRail.SpanSafe) == false)
							{
								transitionBuilder.AddExecutionRail(branchNode.Transition.ExecutionRail);

								prevRail = branchNode.Transition.ExecutionRail;
							}
						}

						_branchesPool.Return(branchNode.Branch);
					}

					{
						var nextDfa = transitionBuilder.ExecutionPathsCount == 1 ? dfa.GetNextDfa(transitionBuilder.ExecutionRail) : null;

						trieNode.Value = transitionBuilder.ExecutionPathsCount == 0 ? NfaExecution.Empty : new NfaExecution(transitionBuilder.Fork(dfa), nextDfa);

						transitionBuilder.Unfork(trieNode.Value.ExecutionRailList);
					}

					while (true)
					{
						var next = headNode.Next;

						if (ReferenceEquals(headNode, DfaBranchNode.Empty))
							break;

						_branchNodesPool.Return(headNode);

						headNode = next;
					}
				}

				private static bool TrieLookahead(ref ThreadContext context, NfaState nfa, ExecutionRailBuilder transitionBuilder)
				{
					transitionBuilder.Reset();

					const int aheadLength = 4;

					var instructionStream = context.InstructionStream;
					var instructionPointer = context.InstructionStreamPointer + 1;
					var operandSpan = instructionStream.PrefetchReadOperandSpan(instructionPointer, aheadLength);
					var localIndex = 0;
					var trieNode = nfa.Trie.RootNode;

					while (true)
					{
						if (localIndex == operandSpan.Length)
						{
							operandSpan = instructionStream.PrefetchReadOperandSpan(instructionPointer + localIndex, aheadLength);
							localIndex = 0;
						}

						var operand = operandSpan[localIndex++];

						trieNode = trieNode.GetNext(operand);

						if (trieNode.IsEmpty)
							return false;

						if (trieNode.Value.ExecutionRailList.Count == -1)
							continue;

						transitionBuilder.Unfork(trieNode.Value.ExecutionRailList);

						return true;
					}
				}

				private sealed class DfaBranch
				{
					public int ReferenceCount;
					public bool Succeeded;
				}

				private sealed class DfaBranchNode
				{
					public static readonly DfaBranchNode Empty = new();

					public DfaBranch Branch;
					public DfaBranchNode Next;
					public DfaBranchNode Parent;
					public DfaTransition Transition;
				}
			}
		}
	}
}