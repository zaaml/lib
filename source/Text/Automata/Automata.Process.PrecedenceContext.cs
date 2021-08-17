// <copyright file="Automata.Process.PrecedenceContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using Zaaml.Core;
using Zaaml.Core.Utils;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			internal struct PrecedenceContext
			{
				public static readonly PrecedenceContext Empty = new(MemorySpan<int>.Empty);

				private MemorySpan<int> _precedence;

				public PrecedenceContext(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> allocator)
				{
					if (automata.PrecedenceDefinitions.Count == 0)
					{
						_precedence = MemorySpan<int>.Empty;

						return;
					}

					var last = automata.PrecedenceDefinitions[automata.PrecedenceDefinitions.Count - 1];

					_precedence = allocator.Allocate(last.Id + last.PrecedenceCount + 1);
					_precedence.Span.Clear();
				}

				private PrecedenceContext(MemorySpan<int> precedence)
				{
					_precedence = precedence;
				}

				public bool IsEmpty => _precedence.IsEmpty;

				public PrecedenceContext Clone()
				{
					var precedence = _precedence.Clone();

					return new PrecedenceContext(precedence);
				}

				public void CopyFrom(ref PrecedenceContext source)
				{
					Debug.Assert(_precedence.Length == source._precedence.Length);

					source._precedence.Span.CopyTo(_precedence.Span);
				}

				public void Leave(PrecedencePredicate precedencePredicate)
				{
					var precedence = precedencePredicate.Precedence;
					var precedenceId = precedencePredicate.Id;
					var span = _precedence.Span;
					ref var head = ref span[precedenceId];
					var offset = precedenceId + 1;

					Debug.Assert(head == precedence);
					Debug.Assert(span[head + offset] > 0);

					if (--span[head + offset] > 0)
						return;

					while (head > 0 && span[--head + offset] == 0)
					{
					}
				}

				public bool TryEnter(PrecedencePredicate precedencePredicate)
				{
					var precedenceId = precedencePredicate.Id;
					var precedence = precedencePredicate.Precedence;
					var span = _precedence.Span;
					ref var head = ref span[precedenceId];

					if (precedence < head)
						return false;

					if (precedence > head)
						head = precedence;

					span[head + precedenceId + 1]++;

					return true;
				}

				public PrecedenceContext DisposeExchange()
				{
					_precedence = _precedence.DisposeExchange();

					return Empty;
				}

				public void Reset()
				{
					_precedence.Span.Clear();
				}
			}

			internal sealed class PrecedenceContextStack : PoolSharedObject<PrecedenceContextStack>
			{
				private readonly Automata<TInstruction, TOperand> _automata;
				private readonly MemorySpanAllocator<int> _allocator;
				private PrecedenceContext[] _stack;
				private int _head;

				public PrecedenceContextStack(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> allocator, Pool<PrecedenceContextStack> pool) : base(pool)
				{
					_automata = automata;
					_allocator = allocator;

					_head = 0;
					_stack = new PrecedenceContext[32];
					
					for (var i = 0; i < _stack.Length; i++)
						_stack[i] = new PrecedenceContext(automata, allocator);
				}

				public bool TryEnter(PrecedencePredicate precedencePredicate)
				{
					if (_stack[_head].TryEnter(precedencePredicate) == false)
						return false;

					if (precedencePredicate.Level == false)
						return true;

					_head++;

					if (_head < _stack.Length)
						return true;
					
					ArrayUtils.ExpandArrayLength(ref _stack, true);

					for (var i = _head; i < _stack.Length; i++)
						_stack[i] = new PrecedenceContext(_automata, _allocator);

					return true;
				}

				public void Leave(PrecedencePredicate precedencePredicate)
				{
					if (precedencePredicate.Level) 
						_head--;

					_stack[_head].Leave(precedencePredicate);
				}

				public PrecedenceContextStack Clone()
				{
					var clone = Pool.Get();

					clone.CopyFrom(this);

					return clone;
				}

				public void CopyFrom(PrecedenceContextStack precedence)
				{
					_head = precedence._head;

					for (var i = 0; i <= precedence._head; i++)
						_stack[i].CopyFrom(ref precedence._stack[i]);
				}

				protected override void OnReleased()
				{
					while (_head >= 0)
					{
						_stack[_head--].Reset();
					}

					_head = 0;

					base.OnReleased();
				}
			}
		}
	}
}