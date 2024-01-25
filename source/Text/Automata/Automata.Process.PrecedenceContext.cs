// <copyright file="Automata.Process.PrecedenceContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Collections;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			internal sealed class PrecedenceContextPool
			{
				private readonly Automata<TInstruction, TOperand> _automata;
				private readonly PrecedenceStackPool _stackPool;
				private PrecedenceContext _poolHead;

				public PrecedenceContextPool(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> spanAllocator)
				{
					_automata = automata;
					_stackPool = new PrecedenceStackPool(spanAllocator);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Release(PrecedenceContext precedenceContext)
				{
					precedenceContext.PoolNext = _poolHead;

					_poolHead = precedenceContext;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public PrecedenceContext Rent()
				{
					if (_poolHead == null)
						return new PrecedenceContext(_automata, this, _stackPool);

					var reference = _poolHead;
					var next = reference.PoolNext;

					reference.PoolNext = null;

					_poolHead = next;

					return reference;
				}
			}

			internal sealed class PrecedenceContextCleaner : ResourceCleaner<PrecedenceContextCleaner, PrecedenceContext>
			{
				public PrecedenceContextCleaner(NodePool nodePool, PrecedenceStackCleaner stackCleaner) : base(nodePool)
				{
					StackCleaner = stackCleaner;
				}

				public PrecedenceStackCleaner StackCleaner { get; }
			}

			internal sealed class PrecedenceContext : ResourceCleaner<PrecedenceContextCleaner, PrecedenceContext>.IResource
			{
				private readonly PrecedenceContextPool _pool;
				private readonly PrecedenceStackPool _stackPool;
				private readonly PrecedenceStack[] _stacks;

				public PrecedenceContext PoolNext;

				public PrecedenceContext(Automata<TInstruction, TOperand> automata, PrecedenceContextPool pool, PrecedenceStackPool stackPool)
				{
					_pool = pool;
					_stackPool = stackPool;

					var definitionsCount = automata.PrecedenceDefinitions.Count;

					if (definitionsCount == 0)
						return;

					_stacks = new PrecedenceStack[definitionsCount];
				}

				public PrecedenceContextCleaner.Node CleanerNode { get; set; }

				public void Clean()
				{
				}

				public PrecedenceContext Allocate(int capacity)
				{
					if (_stacks == null)
						return this;

					for (var i = 0; i < _stacks.Length; i++)
					{
						var stack = _stacks[i] ?? _stackPool.Rent();

						stack.Allocate(capacity);

						_stacks[i] = stack;
					}

					return this;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public PrecedenceContext Fork()
				{
					var fork = _pool.Rent();

					if (_stacks == null)
						return fork;

					var forkStacks = fork._stacks;
					var stackCleaner = CleanerNode.Cleaner.StackCleaner;

					for (var i = 0; i < _stacks.Length; i++) 
						forkStacks[i] = stackCleaner.Add(_stacks[i].Fork());

					CleanerNode.Cleaner.Add(fork);

					return fork;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Leave(PrecedencePredicate precedencePredicate)
				{
					_stacks[precedencePredicate.Id].Leave(precedencePredicate);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void LeaveCode(int precedenceCode)
				{
					_stacks[(precedenceCode & 0xFF_0000) >> 16].LeaveCode(precedenceCode);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool TryEnter(PrecedencePredicate precedencePredicate)
				{
					return _stacks[precedencePredicate.Id].TryEnter(precedencePredicate);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public bool TryEnterCode(int precedenceCode)
				{
					return _stacks[(precedenceCode & 0xFF_0000) >> 16].TryEnterCode(precedenceCode);
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Unfork(PrecedenceContext precedence)
				{
					if (_stacks == null)
						return;

					for (var i = 0; i < _stacks.Length; i++)
						_stacks[i].Unfork(precedence._stacks[i]);
				}

				public void Dispose()
				{
					if (_stacks != null)
					{
						for (var i = 0; i < _stacks.Length; i++)
						{
							_stacks[i].Dispose();
							_stacks[i] = null;
						}
					}

					_pool.Release(this);
				}
			}

			internal sealed class PrecedenceStackPool
			{
				private readonly MemorySpanAllocator<int> _spanAllocator;
				private PrecedenceStack _poolHead;

				public PrecedenceStackPool(MemorySpanAllocator<int> spanAllocator)
				{
					_spanAllocator = spanAllocator;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Release(PrecedenceStack precedenceContext)
				{
					precedenceContext.PoolNext = _poolHead;

					_poolHead = precedenceContext;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public PrecedenceStack Rent()
				{
					if (_poolHead == null)
						return new PrecedenceStack(_spanAllocator, this);

					var reference = _poolHead;
					var next = reference.PoolNext;

					reference.PoolNext = null;

					_poolHead = next;

					return reference;
				}
			}

			internal sealed class PrecedenceStackCleaner : ResourceCleaner<PrecedenceStackCleaner, PrecedenceStack>
			{
				public PrecedenceStackCleaner(NodePool nodePool) : base(nodePool)
				{
				}
			}

			internal sealed class PrecedenceStack : PrecedenceStackCleaner.IResource
			{
				private readonly MemorySpanAllocator<int> _allocator;
				private readonly PrecedenceStackPool _pool;
				private int _count;
				private int _forkCount;
				private PrecedenceStack _forkTarget;
				private MemorySpan<int> _memorySpan;
				public PrecedenceStack PoolNext;

				public PrecedenceStack(MemorySpanAllocator<int> allocator, PrecedenceStackPool pool)
				{
					_allocator = allocator;
					_pool = pool;
					_memorySpan = MemorySpan<int>.Empty;
				}

				public void Clean()
				{
					if (_memorySpan.IsEmpty == false)
						Deallocate();
				}

				PrecedenceStackCleaner.Node PrecedenceStackCleaner.IResource.CleanerNode { get; set; }

				public void Allocate(int capacity)
				{
					if (_memorySpan.Length >= capacity)
						return;

					_memorySpan.Dispose();
					_memorySpan = _allocator.Allocate(capacity);
				}

				public void CopyFrom(PrecedenceStack source)
				{
					_count = source._count;

					if (_memorySpan.Length < _count)
						_memorySpan.Resize(_count, false);

					source._memorySpan.Span.Slice(0, _count).CopyTo(_memorySpan.Span);
				}

				private void Deallocate()
				{
					_memorySpan = _memorySpan.DisposeExchange();
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public PrecedenceStack Fork()
				{
					var fork = _pool.Rent();

					fork._forkTarget = _forkTarget;
					fork._forkCount = _forkCount;

					_forkTarget = fork;
					_forkCount = _count;

					return fork;
				}

				public void Leave(PrecedencePredicate precedencePredicate)
				{
					if (_forkTarget != null && _forkCount == _count)
					{
						_forkTarget.Allocate(_count);
						_forkTarget.CopyFrom(this);

						_forkTarget = null;
						_forkCount = 0;
					}

					var span = _memorySpan.SpanSafe;

					if (precedencePredicate.Level)
						span[_count--] = 0;

					span[_count--] = 0;
				}

				public void LeaveCode(int precedenceCode)
				{
					if (_forkTarget != null && _forkCount == _count)
					{
						_forkTarget.Allocate(_count);
						_forkTarget.CopyFrom(this);

						_forkTarget = null;
						_forkCount = 0;
					}

					var span = _memorySpan.SpanSafe;

					if (precedenceCode > 0xFFFFFF)
						span[_count--] = 0;

					span[_count--] = 0;
				}

				public bool TryEnter(PrecedencePredicate precedencePredicate)
				{
					var span = _memorySpan.SpanSafe;
					var precedence = precedencePredicate.Precedence;
					var currentPrecedence = _count == 0 ? 0 : span[_count - 1];

					if (precedence < currentPrecedence)
						return false;

					span[_count++] = precedence;

					if (precedencePredicate.Level)
						span[_count++] = 0;

					return true;
				}

				public bool TryEnterCode(int precedenceCode)
				{
					var span = _memorySpan.SpanSafe;
					var precedence = precedenceCode & 0xFFFF;
					var currentPrecedence = _count == 0 ? 0 : span[_count - 1];

					if (precedence < currentPrecedence)
						return false;

					span[_count++] = precedence;

					if (precedenceCode > 0xFFFFFF)
						span[_count++] = 0;

					return true;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Unfork(PrecedenceStack precedence)
				{
					if (precedence._forkTarget == null)
						precedence.CopyFrom(this);
					else
						precedence._count = precedence._forkCount;

					precedence._forkCount = _forkCount;
					precedence._forkTarget = _forkTarget;

					_forkTarget = null;
					_forkCount = 0;
				}

				public void Dispose()
				{
					_count = 0;
					_pool.Release(this);
				}
			}
		}
	}
}