// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class DfaBuilder
		{
			private readonly Automata<TInstruction, TOperand> _automata;
			private readonly Process.AutomataStackPool _automataStackPool;
			private readonly List<DfaState> _dfaList = new();
			private readonly ArrayPool<int> _arrayPool = ArrayPool<int>.Create(1024 * 1024, 256);
			private readonly MemorySpanAllocator<int> _dfaStackAllocator;
			private readonly Stack<int> _executionPathBuilder = new();
			private readonly List<DfaTransition> _nfaBuilderList = new();
			private readonly MemorySpanAllocator<int> _railPathAllocator;

			public const int DefaultNextDepth = 1;
			public const int MaxNextDepth = 1;

			public DfaBuilder(Automata<TInstruction, TOperand> automata)
			{
				_automata = automata;
				_dfaStackAllocator = new MemorySpanAllocator<int>(_arrayPool, false);
				_railPathAllocator = new MemorySpanAllocator<int>(_arrayPool, false);
				_automataStackPool = new Process.AutomataStackPool(automata, _dfaStackAllocator);
			}

			private DfaState CreateDfa(Node node, Span<int> stack, DfaState sourceDfa)
			{
				var stackCopy =  _dfaStackAllocator.Allocate(stack.Length);

				stack.CopyTo(stackCopy.SpanSafe);

				var dfa = new DfaState(this, _dfaList.Count, node, stackCopy, sourceDfa);

				_dfaList.Add(dfa);

				return dfa;
			}

			public DfaState GetDfa(Node node, Span<int> stack, DfaState sourceDfa, bool next)
			{
				var depth = sourceDfa?.Depth ?? node.DfaDepth;

				if (next)
					depth--;

				if (node.HasReturn == false && depth <= 1)
					return node.Dfa;

				var trie = node.DfaTrie;
				var depthThreshold = Math.Max(1, depth);

				lock (trie)
				{
					var trieNode = trie.RootNode;
					var stackDepth = 0;
					var length = stack.Length - 1;

					for (var index = length; index >= 0; index--)
					{
						stackDepth++;

						var id = stack[index];

						trieNode = trieNode.GetNextOrCreateSafe(id & SubGraphIdMask);

						if (id > SubGraphIdMask)
							continue;

						if (--depthThreshold == 0)
							break;
					}

					trieNode.Value ??= CreateDfa(node, stack.Slice(stack.Length - stackDepth, stackDepth), sourceDfa);

					return trieNode.Value;
				}
			}

			public DfaState CreateNodeDfa(Node node)
			{
				return CreateDfa(node, Span<int>.Empty, null);
			}

			public DfaState BuildNextDfa(DfaState dfaState, int executionPathId)
			{
				using var automataStack = _automataStackPool.Rent();
				var executionPath = _automata._executionPathRegistry[executionPathId];

				automataStack.Load(dfaState.Stack);

				var node = automataStack.Eval(executionPath);

				node.EnsureSafe();

				return GetDfa(node, automataStack.Span, dfaState, true);
			}

			public NfaState BuildNfa(DfaState dfa, int operand)
			{
				RecursiveBuildNfa(operand, dfa.Node, dfa.Stack, dfa);

				var dfaTransitions = new DfaTransition[_nfaBuilderList.Count];

				for (var index = 0; index < _nfaBuilderList.Count; index++)
					dfaTransitions[index] = _nfaBuilderList[index];

				_nfaBuilderList.Clear();

				return new NfaState(dfaTransitions, BuildPrefixExecutionList(dfa, dfaTransitions));
			}

			private static int GetPrefixLength(DfaTransition[] transitions)
			{
				var prefixLength = 0;

				while (true)
				{
					var executionPath = 0;

					foreach (var dfaTransition in transitions)
					{
						var executionRail = dfaTransition.ExecutionRail;

						if (executionRail.Length <= prefixLength)
						{
							executionPath = 0;

							break;
						}

						if (executionPath == 0)
							executionPath = executionRail[prefixLength];
						else if (executionPath != executionRail[prefixLength])
						{
							executionPath = 0;

							break;
						}
					}

					if (executionPath == 0)
						break;

					prefixLength++;
				}

				return prefixLength;
			}

			private void RecursiveBuildNfa(int operand, Node node, MemorySpan<int> stack, DfaState sourceDfa)
			{
				node.EnsureSafe();

				var executionPaths = node.GetExecutionPaths(operand);

				foreach (var executionPath in executionPaths)
				{
					if (executionPath.IsPredicate)
					{
						var outNode = executionPath.Output;

						_executionPathBuilder.Push(executionPath.Id);

						RecursiveBuildNfa(operand, outNode, stack, sourceDfa);

						_executionPathBuilder.Pop();
					}
					else if (executionPath.OutputReturn)
					{
						var leaveNode = _automata._subGraphRegistry[stack[stack.Length - 1] & SubGraphIdMask].LeaveNode;

						_executionPathBuilder.Push(executionPath.Id);

						RecursiveBuildNfa(operand, leaveNode, stack.Slice(0, stack.Length - 1), sourceDfa);

						_executionPathBuilder.Pop();
					}
					else
					{
						var dfa = GetDfa(node, stack, sourceDfa, true);
						var nextDfa = dfa.GetNextDfa(executionPath.Id);
						var executionPathRail = _railPathAllocator.Allocate(_executionPathBuilder.Count + 1);
						var ei = 0;

						foreach (var e in _executionPathBuilder.Reverse())
							executionPathRail[ei++] = e;

						executionPathRail[ei] = executionPath.Id;

						_nfaBuilderList.Add(new DfaTransition(nextDfa, executionPathRail));
					}
				}
			}

			public ExecutionRailNode BuildPrefixExecutionList(DfaState sourceDfa, DfaTransition[] transitions)
			{
				var prefixLength = GetPrefixLength(transitions);

				if (prefixLength == 0)
					return null;

				var sourceExecutionRail = transitions[0].ExecutionRail.Slice(0, prefixLength);
				var targetExecutionRail = _railPathAllocator.Allocate(prefixLength);
				var targetDfa = sourceDfa.GetNextDfa(sourceExecutionRail);

				sourceExecutionRail.CopyTo(targetExecutionRail);

				var executionRailNode = new ExecutionRailNode(targetDfa)
				{
					ExecutionRail = targetExecutionRail
				};

				return executionRailNode;
			}
		}
	}
}