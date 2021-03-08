// <copyright file="Memory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace Zaaml.Core
{
	internal struct Memory<T>
	{
		public static readonly Memory<T> Empty = new Memory<T>(null, -1, -1, null);

		internal Memory(T[] array, int start, int length, IMemoryAllocator<T> memoryAllocator)
		{
			_array = array;
			_start = start;
			_length = length;
			_memoryAllocator = memoryAllocator;
		}

		private readonly T[] _array;
		private IMemoryAllocator<T> _memoryAllocator;
		private readonly int _start;
		private readonly int _length;

		public Span<T> Span => new(_array, _start, _length);

		public int Length => _length;

		internal bool IsEmpty => _array == null;

		public void Dispose()
		{
			_memoryAllocator?.Deallocate(this);

			_memoryAllocator = null;
		}
	}
}