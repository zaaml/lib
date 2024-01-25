// <copyright file="MemorySpanAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Buffers;
using System.Threading;

namespace Zaaml.Core
{
	internal static class MemorySpanAllocator
	{
		public static MemorySpanAllocator<T> Create<T>(ArrayPool<T> arrayPool, bool clearArray)
		{
			return new MemorySpanAllocator<T>(arrayPool, clearArray);
		}
	}

	internal sealed class MemorySpanAllocator<T> : IMemorySpanAllocator<T>
	{
		private static readonly ThreadLocal<MemorySpanAllocator<T>> ThreadLocalInstance = new(() => new MemorySpanAllocator<T>(PageMemorySpanAllocator<T>.Shared));

		public MemorySpanAllocator(ArrayPool<T> arrayPool, bool clearArray)
		{
			Allocator = new PageMemorySpanAllocator<T>(arrayPool, clearArray);
		}

		private MemorySpanAllocator(IMemorySpanAllocator<T> allocator)
		{
			Allocator = allocator;
		}

		public long Allocated { get; private set; }

		private IMemorySpanAllocator<T> Allocator { get; }

		public static MemorySpanAllocator<T> Shared => ThreadLocalInstance.Value;

		public MemorySpan<T> Allocate(int size)
		{
			Allocated += size;

			return Allocator.Allocate(size);
		}

		public void Deallocate(MemorySpan<T> memorySpan)
		{
			Allocated -= memorySpan.Length;

			Allocator.Deallocate(memorySpan);
		}
	}
}