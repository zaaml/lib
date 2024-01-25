// <copyright file="ArrayPoolSpanAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Threading;

namespace Zaaml.Core
{
	internal sealed class ArrayPoolSpanAllocator<T> : IMemorySpanAllocator<T>
	{
		private static readonly ThreadLocal<ArrayPoolSpanAllocator<T>> ThreadLocalInstance = new(() => new ArrayPoolSpanAllocator<T>(ArrayPool<T>.Shared, false));
		private readonly ArrayPool<T> _arrayPool;
		private readonly bool _clearArray;

		public ArrayPoolSpanAllocator(ArrayPool<T> arrayPool, bool clearArray)
		{
			_arrayPool = arrayPool;
			_clearArray = clearArray;
		}

		public static ArrayPoolSpanAllocator<T> Shared => ThreadLocalInstance.Value;

		public MemorySpan<T> Allocate(int size)
		{
			return new MemorySpan<T>(_arrayPool.Rent(size), 0, size, this);
		}

		public void Deallocate(MemorySpan<T> memorySpan)
		{
			if (ReferenceEquals(memorySpan.Allocator, this) == false)
				throw new InvalidOperationException("Allocator");

			_arrayPool.Return(memorySpan.ArrayInternal, _clearArray);
		}
	}
}