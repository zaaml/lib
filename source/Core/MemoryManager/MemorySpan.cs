// <copyright file="MemorySpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Zaaml.Core.Utils;

// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace Zaaml.Core
{
	internal struct MemorySpan<T> : IDisposable
	{
		public static readonly MemorySpan<T> Empty = new(null, 0, 0, null);

		internal MemorySpan(T[] array, int start, int length, IMemorySpanAllocator<T> allocator)
		{
			_array = array;
			_start = start;
			_length = length;
			_allocator = allocator;
		}

		internal MemorySpan(T[] array, int start, int length)
		{
			_array = array;
			_start = start;
			_length = length;
			_allocator = null;
		}

		internal MemorySpan(T[] array)
		{
			_array = array;
			_start = 0;
			_length = array.Length;
			_allocator = null;
		}

		private readonly T[] _array;
		private readonly int _start;
		private readonly int _length;
		private IMemorySpanAllocator<T> _allocator;

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _length; }
		}

		public readonly Span<T> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _array == null ? Span<T>.Empty : new Span<T>(_array, _start, _length); }
		}

		internal readonly Span<T> SpanSafe
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new(_array, _start, _length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(MemorySpan<T> target)
		{
			SpanSafe.CopyTo(target.SpanSafe);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void CopyTo(MemorySpan<T> target, ref int position)
		{
			SpanSafe.CopyTo(target.SpanSafe.Slice(position));

			position += _length;
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				VerifyIndex(index);

				return _array[_start + index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				VerifyIndex(index);

				_array[_start + index] = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Conditional("DEBUG")]
		private void VerifyIndex(int index)
		{
			if (index < 0 || index >= _length || _length == 0)
				throw new ArgumentOutOfRangeException(nameof(index));
		}

		internal readonly bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _array == null; }
		}

		internal readonly IMemorySpanAllocator<T> Allocator
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _allocator; }
		}

		internal readonly T[] ArrayInternal
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _array; }
		}

		internal readonly int StartInternal
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _start; }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			Interlocked.Exchange(ref _allocator, null)?.Deallocate(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public MemorySpan<T> DisposeExchange()
		{
			Dispose();

			return default;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Span<T>(MemorySpan<T> memorySpan)
		{
			return memorySpan.Span;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureSizePower2Ceiling(int size)
		{
			if (size <= _length)
				return;

			ResizePower2Ceiling(size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T value, int index)
		{
			if (index + 1 >= _length)
				ResizePower2Ceiling(index + 1);

			_array[_start + index] = value;
		}

		private void ResizePower2Ceiling(int size)
		{
			Resize(BitUtils.Power2Ceiling(size), true);
		}

		public void Resize(int size, bool copy)
		{
			if (_allocator != null)
			{
				var memory = _allocator.Allocate(size);

				if (copy && _array != null)
					System.Array.Copy(_array, _start, memory._array, memory._start, _length < size ? _length : size);

				_allocator.Deallocate(this);

				this = memory;
			}
			else
			{
				var array = new T[size];

				if (copy && _array != null)
					System.Array.Copy(_array, _start, array, 0, _length < size ? _length : size);

				this = new MemorySpan<T>(array, 0, size);
			}
		}

		public readonly MemorySpan<T> Clone()
		{
			if (_array == null)
				return Empty;

			var clone = _allocator?.Allocate(_length) ?? new MemorySpan<T>(new T[_length], 0, _length);

			Span.CopyTo(clone.Span);

			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly MemorySpan<T> Slice(int start)
		{
			if (start < 0 || start > _length)
				throw new ArgumentOutOfRangeException();

			return new MemorySpan<T>(_array, _start + start, _length - start, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly MemorySpan<T> Slice(int start, int length)
		{
			if (start < 0 || start + length > _length)
				throw new ArgumentOutOfRangeException();

			return new MemorySpan<T>(_array, _start + start, length, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly MemorySpan<T> DetachAllocator()
		{
			return new MemorySpan<T>(_array, _start, _length, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal readonly MemorySpan<T> SliceSafe(int start)
		{
			return new MemorySpan<T>(_array, _start + start, _length - start, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal readonly MemorySpan<T> SliceSafe(int start, int length)
		{
			return new MemorySpan<T>(_array, _start + start, length, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Span<T>.Enumerator GetEnumerator()
		{
			return Span.GetEnumerator();
		}

		public override readonly string ToString()
		{
			return $"{base.ToString()}[{_length}]";
		}
	}
}