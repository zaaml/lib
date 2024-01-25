// <copyright file="Automata.Process.ExecutionStream.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach


using System;
using System.Runtime.CompilerServices;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			internal sealed class ExecutionStream : PoolSharedObject<ExecutionStream>
			{
				private readonly MemorySpanAllocator<int> _memorySpanAllocator;
				private MemorySpan<int> _executionPathMemorySpan;

				public ExecutionStream(MemorySpanAllocator<int> memorySpanAllocator, Pool<ExecutionStream> pool) : base(pool)
				{
					_memorySpanAllocator = memorySpanAllocator;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Enqueue(int executionPath, int streamPointer)
				{
					_executionPathMemorySpan.Add(executionPath, streamPointer);
				}

				public Span<int> GetSpan(int length)
				{
					return _executionPathMemorySpan.SpanSafe.Slice(0, length);
				}

				public Span<int> GetSpan(int index, int length)
				{
					return _executionPathMemorySpan.SpanSafe.Slice(index, length);
				}

				protected override void OnMount()
				{
					base.OnMount();

					_executionPathMemorySpan = _memorySpanAllocator.Allocate(1024);
				}

				protected override void OnReleased()
				{
					_executionPathMemorySpan = _executionPathMemorySpan.DisposeExchange();

					base.OnReleased();
				}
			}
		}
	}
}