// <copyright file="MemorySpanIntern.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Utils;

namespace Zaaml.Core
{
	internal sealed class MemorySpanIntern<T> where T : IEquatable<T>
	{
		private readonly MemorySpanAllocator<T> _allocator;
		private readonly Dictionary<MemorySpanKey, InternedMemorySpan> _internDictionary = new(EqualityComparer.Instance);
		private T[] _tempSpan;

		public MemorySpanIntern(MemorySpanAllocator<T> allocator)
		{
			_allocator = allocator;
		}

		public MemorySpan<T> Intern(Span<T> span)
		{
			ArrayUtils.EnsureArrayLength(ref _tempSpan, span.Length);

			var tempSpan = new Span<T>(_tempSpan, 0, span.Length);

			span.CopyTo(tempSpan);

			return Intern(new MemorySpan<T>(_tempSpan, 0, span.Length));
		}

		public MemorySpan<T> Intern(MemorySpan<T> span)
		{
			var key = new MemorySpanKey(span);

			if (_internDictionary.TryGetValue(key, out var internedMemorySpan))
				return internedMemorySpan.Key.Span;

			var internedSpan = _allocator.Allocate(span.Length).DetachAllocator();

			span.CopyTo(internedSpan);

			internedMemorySpan = new InternedMemorySpan(new MemorySpanKey(internedSpan));

			_internDictionary.Add(internedMemorySpan.Key, internedMemorySpan);

			return internedSpan;
		}

		private readonly struct MemorySpanKey
		{
			public readonly int HashCode;
			public readonly MemorySpan<T> Span;

			public MemorySpanKey(MemorySpan<T> span)
			{
				Span = span;

				var hashCode = 0;

				unchecked
				{
					foreach (var value in span.SpanSafe)
					{
						hashCode = (hashCode * 397) ^ value.GetHashCode();
					}
				}

				HashCode = hashCode;
			}
		}

		private sealed class EqualityComparer : IEqualityComparer<MemorySpanKey>
		{
			public static readonly EqualityComparer Instance = new();

			private EqualityComparer()
			{
			}

			public bool Equals(MemorySpanKey x, MemorySpanKey y)
			{
				return x.HashCode == y.HashCode && x.Span.SpanSafe.SequenceEqual(y.Span.SpanSafe);
			}

			public int GetHashCode(MemorySpanKey key)
			{
				return key.HashCode;
			}
		}

		private sealed class InternedMemorySpan
		{
			public readonly MemorySpanKey Key;

			public InternedMemorySpan(MemorySpanKey key)
			{
				Key = key;
			}
		}
	}
}