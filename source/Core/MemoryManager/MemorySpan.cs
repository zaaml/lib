// <copyright file="MemorySpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Zaaml.Core.Utils;

// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace Zaaml.Core
{
	internal struct MemorySpan<T> : IDisposable
	{
		public static readonly MemorySpan<T> Empty = new(null, -1, -1, null);

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

		private readonly T[] _array;
		private readonly int _start;
		private readonly int _length;
		private IMemorySpanAllocator<T> _allocator;

		public int Length => _length;

		public readonly Span<T> Span => _array == null ? Span<T>.Empty : new Span<T>(_array, _start, _length);

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

		internal readonly bool IsEmpty => _array == null;

		internal readonly IMemorySpanAllocator<T> Allocator => _allocator;

		internal readonly T[] Array => _array;

		public void Dispose()
		{
			_allocator?.Deallocate(this);
			_allocator = null;
		}

		public MemorySpan<T> DisposeExchange()
		{
			Dispose();

			return default;
		}

		public static implicit operator Span<T>(MemorySpan<T> memorySpan)
		{
			return memorySpan.Span;
		}

		public void EnsureSizePower2Ceiling(int size)
		{
			if (size <= _length)
				return;

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

		public readonly MemorySpan<T> Slice(int start)
		{
			if (start < 0 || start > _length)
				throw new ArgumentOutOfRangeException();

			return new MemorySpan<T>(_array, _start + start, _length - start, null);
		}

		public readonly MemorySpan<T> Slice(int start, int length)
		{
			if (start < 0 || start + length > _length)
				throw new ArgumentOutOfRangeException();

			return new MemorySpan<T>(_array, _start + start, length, null);
		}

		public readonly Span<T>.Enumerator GetEnumerator() => Span.GetEnumerator();

		public override readonly string ToString()
		{
			return $"{base.ToString()}[{_length}]";
		}
	}
}