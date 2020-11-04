// <copyright file="Automata.ArrayPlane.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private sealed class ArrayPage<T> : PoolSharedObject<ArrayPage<T>>
		{
			private T[] _array;
			private int _offset;

			public ArrayPage(IPool<ArrayPage<T>> pool) : base(pool)
			{
			}

			public Span<T> Allocate(int length, out int offset)
			{
				offset = _offset;

				var span = new Span<T>(_array, _offset, length);

				_offset += length;

				return span;
			}

			public bool CanAllocate(int length)
			{
				return _offset + length <= _array.Length;
			}

			protected override void OnMount()
			{
				base.OnMount();

				_array = ArrayPool<T>.Shared.Rent(4096);
			}

			protected override void OnReleased()
			{
				ArrayPool<T>.Shared.Return(_array);

				_array = null;

				base.OnReleased();
			}
		}

		private sealed class ArrayPageManager<T> : Pool<ArrayPage<T>>
		{
			private ArrayPage<T> _currentPage;

			public ArrayPageManager() : base(p => new ArrayPage<T>(p))
			{
			}

			public ArrayPage<T> GetPlane(int capacity)
			{
				if (_currentPage == null)
					return _currentPage = Get().AddReference();

				if (_currentPage.CanAllocate(capacity))
					return _currentPage;

				_currentPage.ReleaseReference();
				_currentPage = Get().AddReference();

				return _currentPage;
			}
		}

		private readonly ref struct ArrayPageSegment<T>
		{
			public ArrayPageSegment(ArrayPage<T> page, int offset, ReadOnlySpan<T> span)
			{
				Page = page;
				Offset = offset;
				Span = span;
			}

			public readonly ArrayPage<T> Page;
			public readonly int Offset;
			public readonly ReadOnlySpan<T> Span;

			public void Lock()
			{
				Page.AddReference();
			}

			public void Unlock()
			{
				Page.ReleaseReference();
			}
		}
	}
}