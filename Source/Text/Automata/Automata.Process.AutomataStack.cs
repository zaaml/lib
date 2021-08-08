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
			private sealed partial class AutomataStack : PoolSharedObject<AutomataStack>
			{
				private readonly Automata<TInstruction, TOperand> _automata;
				private readonly List<SubGraph> _automataSubGraphRegistry;
				private readonly MemorySpanAllocator<int> _memorySpanAllocator;
				private int _count;
				private MemorySpan<int> _memorySpan;
				private int _returnHashCode;
				private int _returnHashCodeDepth;
				private int _stackHashCode;
				private int _stackHashCodeDepth;
				private bool _stackHashCodeDirty;

				public AutomataStack(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> memorySpanAllocator, Pool<AutomataStack> pool) : base(pool)
				{
					_automata = automata;
					_memorySpanAllocator = memorySpanAllocator;
					_automataSubGraphRegistry = _automata._subGraphRegistry;
				}

				public int Capacity => _memorySpan.Length;

				public IEnumerable<string> DebugItems => SpanPrivate.ToArray().Select(id => _automataSubGraphRegistry[id].ToString());

				public string DebugView => string.Join("\n", DebugItems);

				public int ReturnDepth
				{
					get
					{
						if (_stackHashCodeDirty)
							EvalStackHashCode();

						return _returnHashCodeDepth;
					}
				}

				public int ReturnHashCode => _stackHashCodeDirty ? EvalStackHashCode() : _returnHashCode;

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
					stack1.EvalStackHashCode();
					stack2.EvalStackHashCode();

					var span1 = stack1.SpanPrivate.Slice(stack1._stackHashCode);
					var span2 = stack2.SpanPrivate.Slice(stack2._stackHashCode);

					return span1.SequenceEqual(span2);
				}

				public static bool AreReturnDepthEqual(AutomataStack stack1, AutomataStack stack2)
				{
					stack1.EvalStackHashCode();
					stack2.EvalStackHashCode();

					var span1 = stack1.SpanPrivate.Slice(stack1._returnHashCodeDepth);
					var span2 = stack2.SpanPrivate.Slice(stack2._returnHashCodeDepth);

					return span1.SequenceEqual(span2);
				}

				public void Clear()
				{
					_count = 0;
					_returnHashCode = 0;
					_returnHashCodeDepth = 0;
					_stackHashCode = 0;
					_stackHashCodeDirty = false;
				}

				public AutomataStack Clone()
				{
					var clone = Pool.Get();

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

					_stackHashCodeDirty = source._stackHashCodeDirty;
					_stackHashCodeDepth = source._stackHashCodeDepth;
					_returnHashCode = source._returnHashCode;
					_stackHashCode = source._stackHashCode;
					_returnHashCodeDepth = source._returnHashCodeDepth;
				}

				public AutomataStack Deallocate()
				{
					if (_memorySpan.IsEmpty)
						throw new InvalidOperationException("Not allocated");

					_memorySpan = _memorySpan.DisposeExchange();
					_count = 0;
					_stackHashCodeDirty = true;

					return this;
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

					_stackHashCodeDirty = true;

					return path.OutputReturn ? _automataSubGraphRegistry[output].LeaveNode : path.Output;
				}

				private int EvalStackHashCode()
				{
					_returnHashCodeDepth = 0;
					_stackHashCodeDepth = 0;
					_returnHashCode = 0;
					_stackHashCode = 0;

					var lastReturnFound = false;
					var stackHashCodeDepthThreshold = _automata.StackHashCodeDepthThreshold;

					unchecked
					{
						var span = _memorySpan.Span.Slice(0, _count);
						var subGraphRegistry = _automataSubGraphRegistry;

						for (var index = span.Length - 1; index >= 0; index--)
						{
							var id = span[index];
							var subGraph = subGraphRegistry[id];

							_stackHashCode *= 397;
							_stackHashCode ^= id;
							_stackHashCodeDepth++;

							if (lastReturnFound == false && subGraph.LeaveNode.HasReturnPathSafe)
							{
								_returnHashCodeDepth = _stackHashCodeDepth;
								_returnHashCode = _stackHashCode;

								continue;
							}

							lastReturnFound = true;

							if (--stackHashCodeDepthThreshold <= 0)
								break;
						}
					}

					_stackHashCodeDirty = false;

					return _stackHashCode;
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

				//public void MemoryExchange(AutomataStack source, AutomataStackMemoryExchangeKind exchangeKind)
				//{
				//	switch (exchangeKind)
				//	{
				//		case AutomataStackMemoryExchangeKind.KeepNone:
				//			break;

				//		case AutomataStackMemoryExchangeKind.KeepTarget:

				//		{
				//			var targetSpan = _memorySpan.Span.Slice(0, _count);
				//			var sourceSpan = source._memorySpan.Span.Slice(0, _count);

				//			targetSpan.CopyTo(sourceSpan);
				//		}

				//			break;
				//		case AutomataStackMemoryExchangeKind.KeepSource:

				//		{
				//			var targetSpan = _memorySpan.Span.Slice(0, source._count);
				//			var sourceSpan = source._memorySpan.Span.Slice(0, source._count);

				//			sourceSpan.CopyTo(targetSpan);
				//		}

				//			break;
				//		default:
				//			throw new NotSupportedException();
				//	}

				//	(_memorySpan, source._memorySpan) = (source._memorySpan, _memorySpan);
				//}

				protected override void OnReleased()
				{
					if (_memorySpan.IsEmpty == false)
						Deallocate();

					base.OnReleased();
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public SubGraph Peek(int headIndex)
				{
					return _automata._subGraphRegistry[_memorySpan[_count - headIndex - 1]];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public SubGraph Peek()
				{
					return _automata._subGraphRegistry[_memorySpan[_count - 1]];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveRuleNode PeekLeaveNode(int headIndex)
				{
					return _automata._subGraphRegistry[_memorySpan[_count - headIndex - 1]].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveRuleNode PeekLeaveNode()
				{
					return _automata._subGraphRegistry[_memorySpan[_count - 1]].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public SubGraph Pop()
				{
					_stackHashCodeDirty = true;

					return _automataSubGraphRegistry[_memorySpan[--_count]];
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public LeaveRuleNode PopLeaveNode()
				{
					_stackHashCodeDirty = true;

					return _automataSubGraphRegistry[_memorySpan[--_count]].LeaveNode;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void PopNoReturn()
				{
					_stackHashCodeDirty = true;
					_count--;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Push(SubGraph subGraph)
				{
					_stackHashCodeDirty = true;

					if (_count == _memorySpan.Length)
						Expand();

					_memorySpan[_count++] = subGraph.Id;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void Push(int id)
				{
					_stackHashCodeDirty = true;

					if (_count == _memorySpan.Length)
						Expand();

					_memorySpan[_count++] = id;
				}

				private void Resize(int size)
				{
					_memorySpan.Resize(BitUtils.Power2Ceiling(size), true);
				}
			}
		}
	}
}