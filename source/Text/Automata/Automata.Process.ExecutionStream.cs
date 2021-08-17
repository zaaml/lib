// <copyright file="Automata.Process.ExecutionStream.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach


using System;
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
				private bool _forkPath;

				public ExecutionStream(MemorySpanAllocator<int> memorySpanAllocator, Pool<ExecutionStream> pool) : base(pool)
				{
					_memorySpanAllocator = memorySpanAllocator;
				}

				public void Enqueue(ExecutionPath executionPath, ref int streamPointer)
				{
					_forkPath |= executionPath.IsForkExecutionPath;
					_executionPathMemorySpan.EnsureSizePower2Ceiling(streamPointer + 1);
					_executionPathMemorySpan.Span[streamPointer++] = executionPath.Id;
				}

				public Span<int> GetSpan(int executionStreamPointer)
				{
					return _executionPathMemorySpan.Span.Slice(0, executionStreamPointer);
				}

				protected override void OnMount()
				{
					base.OnMount();

					_executionPathMemorySpan = _memorySpanAllocator.Allocate(1024);
				}

				protected override void OnReleased()
				{
					if (_forkPath)
					{

					}

					_executionPathMemorySpan.Dispose();
					_executionPathMemorySpan = MemorySpan<int>.Empty;

					base.OnReleased();
				}
			}
		}
	}
}