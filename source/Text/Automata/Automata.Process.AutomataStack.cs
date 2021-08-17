// <copyright file="Automata.Process.AutomataStack.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			// ReSharper disable once MemberCanBeProtected.Local
			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			internal partial class AutomataStack : PoolSharedObject<AutomataStack>
			{
				private readonly Automata<TInstruction, TOperand> _automata;
				private readonly List<SubGraph> _automataSubGraphRegistry;
				private readonly MemorySpanAllocator<int> _memorySpanAllocator;
				private int _count;
				private int _hashCode;
				private int _hashCodeDepth;
				private bool _hashCodeDirty;
				private MemorySpan<int> _memorySpan;

				public AutomataStack(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> memorySpanAllocator, Pool<AutomataStack> pool) : base(pool)
				{
					_automata = automata;
					_memorySpanAllocator = memorySpanAllocator;
					_automataSubGraphRegistry = _automata._subGraphRegistry;
				}

				public AutomataStack(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> memorySpanAllocator) : base(null)
				{
					_automata = automata;
					_memorySpanAllocator = memorySpanAllocator;
					_automataSubGraphRegistry = _automata._subGraphRegistry;
				}

				public int Capacity => _memorySpan.Length;

				public IEnumerable<string> DebugItems => SpanPrivate.ToArray().Select(id => _automataSubGraphRegistry[id & SubGraphIdMask].ToString());

				public string DebugView => string.Join("\n", DebugItems);

				public int HashCodeDepth
				{
					get
					{
						if (_hashCodeDirty)
							EvalHashCode();

						return _hashCodeDepth;
					}
				}

				public int HashCode => _hashCodeDirty ? EvalHashCode() : _hashCode;

				public ReadOnlySpan<int> HashCodeSpan
				{
					get
					{
						var hashCodeDepth = HashCodeDepth;

						return _memorySpan.Span.Slice(_count - hashCodeDepth, hashCodeDepth);
					}
				}

				public ReadOnlySpan<int> Span => SpanPrivate;

				private Span<int> SpanPrivate => _memorySpan.Span.Slice(0, _count);

				public AutomataStack Allocate(int stackSize)
				{
					if (_memorySpan.IsEmpty == false)
						throw new InvalidOperationException("Allocated");

					_memorySpan.Dispose();
					_memorySpan = _memorySpanAllocator.Allocate(stackSize);

					return this;
				}

				public static bool AreHashDepthEqual(AutomataStack stack1, AutomataStack stack2)
				{
					stack1.EvalHashCode();
					stack2.EvalHashCode();

					var span1 = stack1.SpanPrivate.Slice(stack1._hashCode);
					var span2 = stack2.SpanPrivate.Slice(stack2._hashCode);

					return span1.SequenceEqual(span2);
				}

				public void Clear()
				{
					_count = 0;
					_hashCode = 0;
					_hashCodeDirty = false;
				}

				public AutomataStack Clone()
				{
					var clone = Pool.Get();

					clone.CopyFrom(this);

					return clone;
				}

				private AutomataStack CloneDfa(MemorySpan<int> memorySpan, MemorySpanAllocator<int> memorySpanAllocator)
				{
					var automataStack = new AutomataStack(_automata, memorySpanAllocator)
					{
						_memorySpan = memorySpan,
						_count = memorySpan.Length,
						_hashCodeDirty = _hashCodeDirty,
						_hashCodeDepth = _hashCodeDepth,
						_hashCode = _hashCode
					};

					return automataStack;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void CopyFrom(AutomataStack source)
				{
					_count = source._count;

					if (_memorySpan.Length < _count)
						_memorySpan.Resize(_count, false);

					source.SpanPrivate.CopyTo(SpanPrivate);

					_hashCodeDirty = source._hashCodeDirty;
					_hashCodeDepth = source._hashCodeDepth;
					_hashCode = source._hashCode;
				}

				public AutomataStack CreateDfaStack(MemorySpanAllocator<int> memorySpanAllocator)
				{
					var hashCodeDepth = HashCodeDepth;
					var memorySpan = memorySpanAllocator.Allocate(hashCodeDepth);

					HashCodeSpan.CopyTo(memorySpan.Span);

					return CloneDfa(memorySpan, memorySpanAllocator);
				}

				public void Deallocate()
				{
					if (_memorySpan.IsEmpty)
						throw new InvalidOperationException("Not allocated");

					_memorySpan = _memorySpan.DisposeExchange();
					_count = 0;
					_hashCodeDirty = true;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				[Conditional("AUTOMATA_VERIFY_STACK")]
				private void EnsureDepth(int depthDelta)
				{
					if (_count + depthDelta >= _memorySpan.Length)
						Resize(_count + depthDelta);
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

					_hashCodeDirty = true;

					return path.OutputReturn ? _automataSubGraphRegistry[output & SubGraphIdMask].LeaveNode : path.Output;
				}

				public AutomataStack EvalDfa(ReadOnlySpan<int> executionPaths, out Node node)
				{
					var executionPathRegistry = _automata._executionPathRegistry;
					var stackDepth = 0;

					for (var i = 0; i < executionPaths.Length; i++)
					{
						var executionPath = executionPathRegistry[executionPaths[i]];

						if (stackDepth < executionPath.StackDepth)
							stackDepth = executionPath.StackDepth;
					}

					var nextCount = _count + stackDepth;
					var memorySpan = _memorySpanAllocator.Allocate(nextCount);
					var span = Span;

					span.CopyTo(memorySpan);

					var automataStack = new AutomataStack(_automata, _memorySpanAllocator)
					{
						_memorySpan = memorySpan,
						_count = span.Length,
						_hashCodeDirty = true
					};

					node = null;

					foreach (var executionPathId in executionPaths)
					{
						var executionPath = executionPathRegistry[executionPathId];

						node = automataStack.Eval(executionPath);
					}

					return automataStack;
				}

				private int EvalHashCode()
				{
					_hashCode = EvalHashCode(0, out _hashCodeDepth);
					_hashCodeDirty = false;
					
					return _hashCode;
				}

				public int EvalHashCode(int tail, out int hashCodeDepth)
				{
					var hashCode = 0;


					hashCodeDepth = 0;

					var lastReturnFound = false;
					var stackHashCodeDepthThreshold = _automata.StackHashCodeDepthThreshold;
					
					unchecked
					{
						var span = _memorySpan.Span.Slice(0, _count);

						for (var index = span.Length - 1; index >= tail; index--)
						{
							var id = span[index];

							hashCode *= 397;
							hashCode ^= id & SubGraphIdMask;

							hashCodeDepth++;

							if (lastReturnFound == false && id > SubGraphIdMask)
								continue;

							lastReturnFound = true;

							if (--stackHashCodeDepthThreshold <= 0)
								break;
						}
					}

					return hashCode;
				}

				private void Expand()
				{
					_memorySpan.Resize(BitUtils.Power2Ceiling(_memorySpan.Length + 1), true);
				}

				public AutomataStack Fork()
				{
					var fork = Pool.Get();

					fork.CopyFrom(this);

					return fork;
				}

				protected override void OnReleased()
				{
					if (_memorySpan.IsEmpty == false)
						Deallocate();

					base.OnReleased();
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
				public LeaveRuleNode PeekLeaveNode(int headIndex)
				{
					return _automataSubGraphRegistry[_memorySpan[_count - headIndex - 1] & SubGraphIdMask].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveRuleNode PeekLeaveNode()
				{
					return _automataSubGraphRegistry[_memorySpan[_count - 1] & SubGraphIdMask].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public SubGraph Pop()
				{
					_hashCodeDirty = true;

					return _automataSubGraphRegistry[_memorySpan[--_count] & SubGraphIdMask];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveRuleNode PopLeaveNode()
				{
					_hashCodeDirty = true;

					return _automataSubGraphRegistry[_memorySpan[--_count] & SubGraphIdMask].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void PopNoReturn()
				{
					_hashCodeDirty = true;
					_count--;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Push(SubGraph subGraph)
				{
					_hashCodeDirty = true;

					//if (_count == _memorySpan.Length)
					//	Expand();

					_memorySpan[_count++] = subGraph.RId;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void Push(int id)
				{
					_hashCodeDirty = true;

					//if (_count == _memorySpan.Length)
					//	Expand();

					_memorySpan[_count++] = id;
				}

				private void Resize(int size)
				{
					_memorySpan.Resize(BitUtils.Power2Ceiling(size), true);
				}

				public static IEqualityComparer<AutomataStack> EqualityComparer => AutomataStackEqualityComparer.Instance;
				
				public static IEqualityComparer<AutomataStack> FullEqualityComparer => AutomataStackFullEqualityComparer.Instance;

				private sealed class AutomataStackEqualityComparer : IEqualityComparer<AutomataStack>
				{
					public static readonly AutomataStackEqualityComparer Instance = new();

					private AutomataStackEqualityComparer()
					{
					}

					public bool Equals(AutomataStack x, AutomataStack y)
					{
						var xs = x.HashCodeSpan;
						var ys = y.HashCodeSpan;

						if (xs.Length != ys.Length)
							return false;

						for (var i = 0; i < xs.Length; i++)
						{
							if (xs[i] != ys[i])
								return false;
						}

						return true;
					}

					public int GetHashCode(AutomataStack key)
					{
						return key.HashCode;
					}
				}
				
				private sealed class AutomataStackFullEqualityComparer : IEqualityComparer<AutomataStack>
				{
					public static readonly AutomataStackFullEqualityComparer Instance = new();

					private AutomataStackFullEqualityComparer()
					{
					}

					public bool Equals(AutomataStack x, AutomataStack y)
					{
						var xs = x.Span;
						var ys = y.Span;

						if (xs.Length != ys.Length)
							return false;

						for (var i = 0; i < xs.Length; i++)
						{
							if (xs[i] != ys[i])
								return false;
						}

						return true;
					}

					public int GetHashCode(AutomataStack key)
					{
						return key.HashCode;
					}
				}
			}
		}
	}
}