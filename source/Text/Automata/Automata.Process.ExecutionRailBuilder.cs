// <copyright file="Automata.Process.ThreadTransitionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			private sealed class ExecutionRailBuilder
			{
				private readonly MemorySpanAllocator<int> _allocator = new(ArrayPool<int>.Create(65536, 1024), false);
				private readonly ExecutionRailNodePool _executionRailNodePool = new();
				private readonly Pool<ExecutionRailBuilder> _pool;
				private ExecutionRailNode _currentNode;
				private ExecutionRailNode _rootNode;
				private ExecutionRailNode _prefixNode;
				public int ExecutionPathsCount;

				internal ExecutionRailBuilder(Pool<ExecutionRailBuilder> pool)
				{
					_pool = pool;
				}

				public MemorySpan<int> ExecutionRail
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => _rootNode.ExecutionRail;
				}

				public DfaState Dfa
				{
					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					get => _rootNode.Dfa;
				}

				public void AddExecutionPath(int executionPathId)
				{
					if (_rootNode == null)
						_rootNode = _currentNode = RentNode();
					else
						_currentNode.Next = _currentNode = RentNode();

					_currentNode.ExecutionRail = _allocator.Allocate(1);
					_currentNode.ExecutionRail[0] = executionPathId;

					ExecutionPathsCount++;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private ExecutionRailNode RentNode()
				{
					return _executionRailNodePool.Rent();
				}

				public void AddPrefix(ExecutionRailNode prefixNode)
				{
					if (_prefixNode != null)
						throw new InvalidOperationException();

					_prefixNode = prefixNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void AddExecutionRail(MemorySpan<int> executionRail)
				{
					if (_rootNode == null)
						_rootNode = _currentNode = RentNode();
					else
						_currentNode.Next = _currentNode = RentNode();

					_currentNode.ExecutionRail = executionRail;

					ExecutionPathsCount++;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public int Build(ref Thread thread, ref ThreadContext context, TransitionBuilder transitionBuilder)
				{
					//if (thread.Node.PrefixExecution != null)
					//{
					//	Unfork(new ExecutionRailList(1, thread.Node.PrefixExecution));

					//	return 1;
					//}

					Reset();

					transitionBuilder.Build(ref thread, ref context, this);

#if false
					if (ExecutionPathsCount > 1 && _rootNode.RunStat is { FailRatio: > 0.5f })
						ReduceExecution(ref thread, ref context);
#endif

					return ExecutionPathsCount;
				}

				private void ReduceExecution(ref Thread thread, ref ThreadContext context)
				{
					var root = _rootNode;

					while (ExecutionPathsCount > 0)
					{
						var forkRunStat = _rootNode.RunStat;

						if (forkRunStat == null)
							break;

						var failRatio = forkRunStat.FailRatio;

						if (failRatio > 0.5f && ReduceExecution(root, ref thread, ref context))
						{
							_rootNode = _rootNode.Next;

							root.Dispose();

							root = _rootNode;

							ExecutionPathsCount--;
						}
						else
							break;
					}
				}

				private bool ReduceExecution(ExecutionRailNode executionRailNode, ref Thread thread, ref ThreadContext context)
				{
					return false;
				}

				public ExecutionRailList DetachRailList()
				{
					var railList = new ExecutionRailList(ExecutionPathsCount, _rootNode, _prefixNode);

					_rootNode = _currentNode = _prefixNode = null;

					return railList;
				}

				public void Dispose()
				{
					_rootNode?.Dispose();
					_rootNode = _currentNode = _prefixNode = null;

					_pool.Return(this);
				}

				public ExecutionRailList Fork(DfaState sourceDfa)
				{
					var dfaState = sourceDfa.GetNextDfa(_rootNode.ExecutionRail);
					var rootNode = new ExecutionRailNode(dfaState)
					{
						ExecutionRail = _rootNode.ExecutionRail
					};

					Debug.Assert(_rootNode.ExecutionRail.Allocator == null);

					var currentTarget = rootNode;
					var currentSource = _rootNode.Next;
					var prefixNode = _prefixNode;

					while (currentSource != null)
					{
						currentTarget = currentTarget.Next = new ExecutionRailNode(sourceDfa.GetNextDfa(currentSource.ExecutionRail))
						{
							ExecutionRail = currentSource.ExecutionRail
						};

						Debug.Assert(currentSource.ExecutionRail.Allocator == null);

						currentSource = currentSource.Next;
					}

					var executionRailList = new ExecutionRailList(ExecutionPathsCount, rootNode, prefixNode);

					Unfork(executionRailList);

					return executionRailList;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Reset()
				{
					ExecutionPathsCount = 0;

					_rootNode?.Dispose();
					_rootNode = null;
					_currentNode = null;
					_prefixNode = null;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Unfork(ExecutionRailList executionRailList)
				{
					_rootNode?.Dispose();

					_rootNode = executionRailList.Root;
					_prefixNode = executionRailList.Prefix;
					ExecutionPathsCount = executionRailList.Count;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Unfork(ExecutionRailNode executionRailNode, int count)
				{
					_rootNode?.Dispose();

					_rootNode = executionRailNode;
					_prefixNode = null;
					ExecutionPathsCount = count;
				}
			}
		}
	}
}