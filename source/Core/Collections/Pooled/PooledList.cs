// <copyright file="PooledList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Zaaml.Core.Collections.Pooled
{
	internal partial class PooledList<T>
	{
		private const int MaxArrayLength = 0x7FEFFFFF;
		private const int DefaultCapacity = 4;
		private static readonly T[] EmptyArray = Array.Empty<T>();
		private static readonly ThreadLocal<Stack<PooledList<T>>> ThreadPool = new(() => new Stack<PooledList<T>>());
		private readonly bool _clearArray;
		private readonly int _defaultCapacity;
		private readonly ArrayPool<T> _pool;
		private T[] _items;
		private int _size;
		private object _syncRoot;
		private int _version;

		public PooledList() : this(DefaultCapacity, true, ArrayPool<T>.Shared)
		{
		}

		public PooledList(int defaultCapacity, bool clearArray) : this(defaultCapacity, clearArray, ArrayPool<T>.Shared)
		{
		}

		public PooledList(bool clearArray) : this(DefaultCapacity, clearArray, ArrayPool<T>.Shared)
		{
		}

		public PooledList(int defaultCapacity, bool clearArray, ArrayPool<T> pool)
		{
			_defaultCapacity = defaultCapacity;
			_items = EmptyArray;
			_pool = pool ?? ArrayPool<T>.Shared;
			_clearArray = clearArray;
		}

		public int Capacity
		{
			get => _items.Length;
			set
			{
				if (value < _size)
					throw new ArgumentOutOfRangeException();

				if (value == _items.Length)
					return;

				if (value > 0)
				{
					var newItems = _pool.Rent(value);

					if (_size > 0)
						Array.Copy(_items, newItems, _size);

					ReturnArray();

					_items = newItems;
				}
				else
				{
					ReturnArray();

					_size = 0;
				}
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			InsertRange(_size, collection);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AddWithResize(T item)
		{
			var size = _size;

			EnsureCapacity(size + 1);

			_size = size + 1;
			_items[size] = item;
		}

		private void EnsureCapacity(int min)
		{
			if (_items.Length >= min)
				return;

			var newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;

			if ((uint) newCapacity > MaxArrayLength)
				newCapacity = MaxArrayLength;

			if (newCapacity < min)
				newCapacity = min;

			Capacity = newCapacity;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		public int IndexOf(T item, int index)
		{
			if (index > _size)
				throw new ArgumentOutOfRangeException(nameof(index));

			return Array.IndexOf(_items, item, index, _size - index);
		}

		public int IndexOf(T item, int index, int count)
		{
			if (index > _size)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (count < 0 || index > _size - count)
				throw new ArgumentOutOfRangeException(nameof(count));

			return Array.IndexOf(_items, item, index, count);
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			if ((uint) index > (uint) _size)
				throw new ArgumentOutOfRangeException(nameof(index));

			switch (collection)
			{
				case null:

					throw new ArgumentNullException(nameof(collection));

				case ICollection<T> c:

					var count = c.Count;

					if (count > 0)
					{
						EnsureCapacity(_size + count);

						if (index < _size)
							Array.Copy(_items, index, _items, index + count, _size - index);

						// ReSharper disable once SuspiciousTypeConversion.Global
						if (ReferenceEquals(this, c))
						{
							Array.Copy(_items, 0, _items, index, index);
							Array.Copy(_items, index + count, _items, index * 2, _size - index);
						}
						else
							c.CopyTo(_items, index);

						_size += count;
					}

					break;

				default:

					using (var en = collection.GetEnumerator())
					{
						while (en.MoveNext())
							Insert(index++, en.Current);
					}

					break;
			}

			_version++;
		}

		public void RemoveRange(int index, int count)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));

			if (_size - index < count)
				throw new ArgumentException();

			if (count <= 0)
				return;

			_size -= count;

			if (index < _size) Array.Copy(_items, index + count, _items, index, _size - index);

			_version++;

			if (_clearArray)
				Array.Clear(_items, _size, count);
		}

		internal static PooledList<T> RentList()
		{
			var pool = ThreadPool.Value;

			return pool.Count > 0 ? pool.Pop() : new PooledList<T>(true);
		}

		private void ReturnArray()
		{
			if (_items.Length == 0)
				return;

			try
			{
				_pool.Return(_items, _clearArray);
			}
			catch (ArgumentException)
			{
			}

			_items = EmptyArray;
		}

		internal static void ReturnList(PooledList<T> list)
		{
			list.Clear();

			ThreadPool.Value.Push(list);
		}

		public void TrimExcess()
		{
			var threshold = (int) (_items.Length * 0.9);

			if (_size < threshold)
				Capacity = _size;
		}

		public int Count => _size;

		public T this[int index]
		{
			get
			{
				if ((uint) index >= (uint) _size)
					throw new ArgumentOutOfRangeException();

				return _items[index];
			}

			set
			{
				if ((uint) index >= (uint) _size)
					throw new ArgumentOutOfRangeException();

				_items[index] = value;
				_version++;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			_version++;

			var size = _size;

			if ((uint) size < (uint) _items.Length)
			{
				_size = size + 1;
				_items[size] = item;
			}
			else
				AddWithResize(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			ReturnArray();

			_size = 0;
			_version++;
		}

		public bool Contains(T item)
		{
			return _size != 0 && IndexOf(item) != -1;
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf(_items, item, 0, _size);
		}

		public void Insert(int index, T item)
		{
			if ((uint) index > (uint) _size)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (_size == _items.Length)
				EnsureCapacity(_size + 1);

			if (index < _size)
				Array.Copy(_items, index, _items, index + 1, _size - index);

			_items[index] = item;
			_size++;
			_version++;
		}

		public bool Remove(T item)
		{
			var index = IndexOf(item);

			if (index < 0)
				return false;

			RemoveAt(index);

			return true;
		}

		public void RemoveAt(int index)
		{
			if ((uint) index >= (uint) _size)
				throw new ArgumentOutOfRangeException(nameof(index));

			_size--;

			if (index < _size)
				Array.Copy(_items, index + 1, _items, index, _size - index);

			_version++;

			if (_clearArray)
				_items[_size] = default;
		}
	}
}