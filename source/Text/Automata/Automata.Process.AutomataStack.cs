// <copyright file="Automata.Process.AutomataStack.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			internal sealed class AutomataStackCleaner : ResourceCleaner<AutomataStackCleaner, AutomataStack>
			{
				public AutomataStackCleaner(NodePool nodePool) : base(nodePool)
				{
				}
			}

			// ReSharper disable once MemberCanBeProtected.Local
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			internal partial class AutomataStack : IDisposable, ResourceCleaner<AutomataStackCleaner, AutomataStack>.IResource
			{
				private readonly Automata<TInstruction, TOperand> _automata;
				private readonly List<SubGraph> _automataSubGraphRegistry;
				private readonly MemorySpanAllocator<int> _memorySpanAllocator;
				private readonly AutomataStackPool _pool;
				private int _count;
				private int _forkCount;
				private AutomataStack _forkTarget;
				private MemorySpan<int> _memorySpan;
				public AutomataStack PoolNext;

				public AutomataStack(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> memorySpanAllocator, AutomataStackPool pool)
				{
					_pool = pool;
					_automata = automata;
					_memorySpanAllocator = memorySpanAllocator;
					_automataSubGraphRegistry = _automata._subGraphRegistry;
				}

				public AutomataStack(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> memorySpanAllocator)
				{
					_automata = automata;
					_memorySpanAllocator = memorySpanAllocator;
					_automataSubGraphRegistry = _automata._subGraphRegistry;
				}

				private AutomataStack()
				{
				}

				public int Capacity => _memorySpan.Length;

				public IEnumerable<string> DebugItems => SpanPrivate.ToArray().Select(id => _automataSubGraphRegistry[id & SubGraphIdMask].ToString());

				public string DebugView => string.Join("\n", DebugItems);

				public bool IsAllocated => _memorySpan.IsEmpty == false;

				public Span<int> Span => SpanPrivate;

				private Span<int> SpanPrivate => _memorySpan.SpanSafe.Slice(0, _count);

				public void Dispose()
				{
					_pool.Release(this);
				}

				public void Clean()
				{
					if (_memorySpan.IsEmpty == false)
						Deallocate();
				}

				public ResourceCleaner<AutomataStackCleaner, AutomataStack>.Node CleanerNode { get; set; }

				public AutomataStack Allocate(int stackSize)
				{
					if (_memorySpan.Length >= stackSize)
						return this;

					_memorySpan.Dispose();
					_memorySpan = _memorySpanAllocator.Allocate(stackSize);

					return this;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void BeforePop()
				{
					if (_count != _forkCount)
						return;

					_forkTarget.Allocate(_count);
					_forkTarget.CopyFrom(this);

					_forkTarget = null;
					_forkCount = 0;
				}

				public void Clear()
				{
					_count = 0;
					_forkCount = 0;
					_forkTarget = null;
				}

				public AutomataStack Clone()
				{
					var clone = _pool.Rent();

					clone.CopyFrom(this);

					return clone;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void CopyFrom(AutomataStack source)
				{
					_count = source._count;

					if (_memorySpan.Length < _count)
						_memorySpan.Resize(_count, false);

					source.SpanPrivate.CopyTo(SpanPrivate);
				}

				public void Deallocate()
				{
					_memorySpan = _memorySpan.DisposeExchange();
					_count = 0;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void EnsureDepth(int depthDelta)
				{
					if (_count + depthDelta >= _memorySpan.Length)
						Resize(_count + depthDelta);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Node Eval(ReadOnlySpan<int> executionPaths)
				{
					var executionPathRegistry = _automata._executionPathRegistry;

					Node node = null;

					foreach (var executionPathIndex in executionPaths)
					{
						var executionPath = executionPathRegistry[executionPathIndex];

						node = Eval(executionPath);
					}

					return node;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public Node Eval(ExecutionPath path)
				{
					var output = 0;

					EnsureDepth(path.StackDepth);

					foreach (var subGraphId in path.EnterReturnSubGraphs)
					{
						if (subGraphId < 0)
							output = _memorySpan[--_count];
						else
							_memorySpan[_count++] = subGraphId;
					}

					return path.OutputReturn ? _automataSubGraphRegistry[output & SubGraphIdMask].LeaveNode : path.Output;
				}

				public int EvalHashCode()
				{
					var hashCode = 0;

					unchecked
					{
						var span = _memorySpan.Span.Slice(0, _count);

						for (var index = span.Length - 1; index >= 0; index--)
						{
							var id = span[index];

							hashCode *= 397;
							hashCode ^= id & SubGraphIdMask;
						}
					}

					return hashCode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public AutomataStack Fork()
				{
					var fork = _pool.Rent();

					CleanerNode?.Cleaner.Add(fork);

					fork._forkTarget = _forkTarget;
					fork._forkCount = _forkCount;

					_forkTarget = fork;
					_forkCount = _count;

					return fork;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public SubGraph Peek(int headIndex)
				{
					return _automataSubGraphRegistry[_memorySpan[_count - headIndex - 1] & SubGraphIdMask];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public SubGraph Peek()
				{
					return _automataSubGraphRegistry[_memorySpan[_count - 1] & SubGraphIdMask];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveSyntaxNode PeekLeaveNode(int headIndex)
				{
					return _automataSubGraphRegistry[_memorySpan[_count - headIndex - 1] & SubGraphIdMask].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveSyntaxNode PeekLeaveNode()
				{
					return _automataSubGraphRegistry[_memorySpan[_count - 1] & SubGraphIdMask].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public SubGraph Pop()
				{
					BeforePop();

					return _automataSubGraphRegistry[_memorySpan[--_count] & SubGraphIdMask];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveSyntaxNode PopLeaveNode()
				{
					BeforePop();

					return _automataSubGraphRegistry[_memorySpan[--_count] & SubGraphIdMask].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void PopNoReturn()
				{
					BeforePop();

					_count--;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Push(SubGraph subGraph)
				{
					_memorySpan[_count++] = subGraph.RId;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void Push(int id)
				{
					_memorySpan[_count++] = id;
				}

				private void Resize(int size)
				{
					_memorySpan.Resize(BitUtils.Power2Ceiling(size), true);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Unfork(AutomataStack stack)
				{
					if (stack._forkTarget == null)
						stack.CopyFrom(this);
					else
						stack._count = stack._forkCount;

					stack._forkCount = _forkCount;
					stack._forkTarget = _forkTarget;

					_forkTarget = null;
					_forkCount = 0;
				}

				public void Load(MemorySpan<int> stack)
				{
					Allocate(stack.Length);
					stack.CopyTo(_memorySpan.SliceSafe(0, stack.Length));

					_count = stack.Length;
				}
			}
		}
	}
}