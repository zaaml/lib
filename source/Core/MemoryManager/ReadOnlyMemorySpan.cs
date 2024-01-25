// <copyright file="ReadOnlyMemorySpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

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

		public ReadOnlySpan<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _memorySpan.Span; }
		}

		internal bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _memorySpan.IsEmpty; }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlyMemorySpan<T>(MemorySpan<T> memorySpan)
		{
			return new ReadOnlyMemorySpan<T>(memorySpan);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ReadOnlySpan<T>(ReadOnlyMemorySpan<T> memorySpan)
		{
			return memorySpan.Span;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemorySpan<T> Slice(int start)
		{
			return _memorySpan.Slice(start);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlyMemorySpan<T> Slice(int start, int length)
		{
			return _memorySpan.Slice(start, length);
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _memorySpan.Length; }
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _memorySpan[index]; }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<T>.Enumerator GetEnumerator()
		{
			return Span.GetEnumerator();
		}
	}
}