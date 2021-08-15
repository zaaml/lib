// <copyright file="ReadOnlyMemorySpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

// ReSharper disable ReplaceSliceWithRangeIndexer

namespace Zaaml.Core
{
	internal readonly struct ReadOnlyMemorySpan<T>
	{
		private readonly MemorySpan<T> _memorySpan;

		public static readonly ReadOnlyMemorySpan<T> Empty = new(MemorySpan<T>.Empty);

		internal ReadOnlyMemorySpan(MemorySpan<T> memorySpan)
		{
			_memorySpan = memorySpan;
		}

		public ReadOnlySpan<T> Span => _memorySpan.Span;

		internal bool IsEmpty => _memorySpan.IsEmpty;

		public static implicit operator ReadOnlyMemorySpan<T>(MemorySpan<T> memorySpan)
		{
			return new ReadOnlyMemorySpan<T>(memorySpan);
		}

		public static implicit operator ReadOnlySpan<T>(ReadOnlyMemorySpan<T> memorySpan)
		{
			return memorySpan.Span;
		}

		public ReadOnlyMemorySpan<T> Slice(int start)
		{
			return _memorySpan.Slice(start);
		}

		public ReadOnlyMemorySpan<T> Slice(int start, int length)
		{
			return _memorySpan.Slice(start, length);
		}

		public int Length => _memorySpan.Length;

		public T this[int index] => _memorySpan[index];

		public ReadOnlySpan<T>.Enumerator GetEnumerator() => Span.GetEnumerator();
	}
}