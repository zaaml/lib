// <copyright file="ReadOnlyListEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Zaaml.Core.Utils
{
	internal sealed class ReadOnlyListEnumerator<T> : IEnumerator<T>
	{
		private int _count;
		private int _index;
		private IReadOnlyList<T> _list;
		private ReadOnlyListEnumeratorPool<T> _pool;

		public void Mount(IReadOnlyList<T> list, ReadOnlyListEnumeratorPool<T> pool)
		{
			if (_list != null)
				throw new InvalidOperationException();

			_list = list;
			_pool = pool;
			_index = -1;
			_count = list.Count;
		}

		public void Release()
		{
			if (_list == null)
				throw new InvalidOperationException();

			_list = null;
			_pool.Release(this);
		}

		public bool MoveNext()
		{
			_index++;

			return _index < _count;
		}

		public void Reset()
		{
			_index = -1;
		}

		public T Current => _list[_index];

		object IEnumerator.Current => Current;

		public void Dispose()
		{
			Release();
		}
	}

	internal sealed class ReadOnlyListEnumeratorPool<T>
	{
		private static readonly ThreadLocal<ReadOnlyListEnumeratorPool<T>> ThreadLocalPool = new(() => new ReadOnlyListEnumeratorPool<T>());
		private readonly Stack<ReadOnlyListEnumerator<T>> _pool = new();

		private ReadOnlyListEnumeratorPool()
		{
		}

		public static ReadOnlyListEnumeratorPool<T> Shared => ThreadLocalPool.Value;

		public void Release(ReadOnlyListEnumerator<T> enumerator)
		{
			_pool.Push(enumerator);
		}

		public ReadOnlyListEnumerator<T> Rent(IReadOnlyList<T> list)
		{
			var enumerator = _pool.Count > 0 ? _pool.Pop() : new ReadOnlyListEnumerator<T>();

			enumerator.Mount(list, this);

			return enumerator;
		}
	}
}