// <copyright file="Automata.Process.PredicateResultStream.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach


using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			internal sealed class PredicateResultStream : PoolSharedObject<PredicateResultStream>
			{
				private readonly MemorySpanAllocator<PredicateResult> _memorySpanAllocator;
				private MemorySpan<PredicateResult> _predicateResultMemorySpan;

				public PredicateResultStream(MemorySpanAllocator<PredicateResult> memorySpanAllocator, Pool<PredicateResultStream> pool) : base(pool)
				{
					_memorySpanAllocator = memorySpanAllocator;
				}

				public PredicateResult Dequeue(ref int predicateResultStreamPointer)
				{
					var predicateResult = _predicateResultMemorySpan[predicateResultStreamPointer];

					_predicateResultMemorySpan[predicateResultStreamPointer++] = default;

					return predicateResult;
				}

				public void Enqueue(PredicateResult predicateResult, ref int streamPointer)
				{
					_predicateResultMemorySpan.EnsureSizePower2Ceiling(streamPointer + 1);
					_predicateResultMemorySpan.Span[streamPointer++] = predicateResult;
				}

				protected override void OnMount()
				{
					base.OnMount();

					_predicateResultMemorySpan = _memorySpanAllocator.Allocate(1024);
				}

				protected override void OnReleased()
				{
					_predicateResultMemorySpan.Dispose();
					_predicateResultMemorySpan = MemorySpan<PredicateResult>.Empty;

					base.OnReleased();
				}
			}
		}
	}
}