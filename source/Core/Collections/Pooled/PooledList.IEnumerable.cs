// <copyright file="PooledList.IEnumerable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections.Pooled
{
	internal partial class PooledList<T> : IEnumerable<T>
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public struct Enumerator : IEnumerator<T>
		{
			private readonly PooledList<T> _list;
			private int _index;
			private readonly int _version;
			private T _current;

			internal Enumerator(PooledList<T> list)
			{
				_list = list;
				_index = 0;
				_version = list._version;
				_current = default;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				var localList = _list;

				if (_version == localList._version && ((uint) _index < (uint) localList._size))
				{
					_current = localList._items[_index];
					_index++;

					return true;
				}

				return MoveNextRare();
			}

			private bool MoveNextRare()
			{
				if (_version != _list._version)
					throw new InvalidOperationException();

				_index = _list._size + 1;
				_current = default;
				return false;
			}

			public T Current => _current;

			object IEnumerator.Current
			{
				get
				{
					if (_index == 0 || _index == _list._size + 1)
						throw new InvalidOperationException();

					return Current;
				}
			}

			void IEnumerator.Reset()
			{
				if (_version != _list._version)
					throw new InvalidOperationException();

				_index = 0;
				_current = default;
			}
		}
	}
}