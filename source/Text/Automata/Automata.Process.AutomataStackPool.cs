// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			internal sealed class AutomataStackPool
			{
				private readonly Automata<TInstruction, TOperand> _automata;
				private readonly MemorySpanAllocator<int> _spanAllocator;
				private AutomataStack _poolHead;

				public AutomataStackPool(Automata<TInstruction, TOperand> automata, MemorySpanAllocator<int> spanAllocator)
				{
					_automata = automata;
					_spanAllocator = spanAllocator;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Release(AutomataStack automataStack)
				{
					if (automataStack.CleanerNode == null)
						automataStack.Clean();

					automataStack.PoolNext = _poolHead;

					_poolHead = automataStack;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public AutomataStack Rent()
				{
					if (_poolHead == null)
						return new AutomataStack(_automata, _spanAllocator, this);

					var reference = _poolHead;
					var next = reference.PoolNext;

					reference.PoolNext = null;

					_poolHead = next;

					return reference;
				}
			}
		}
	}
}