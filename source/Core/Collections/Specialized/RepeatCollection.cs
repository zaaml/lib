// <copyright file="RepeatCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Collections.Specialized
{
	internal sealed class RepeatCollection<T> : IList, IReadOnlyList<T>, IRepeatCollection
	{
		public RepeatCollection(int count, T value)
		{
			Count = count;
			Value = value;
		}

		public T Value { get; }

		public int Count { get; }

		public object SyncRoot => this;

		public bool IsSynchronized => false;

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return Enumerable.Repeat(Value, Count).GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return Enumerable.Repeat(Value, Count).GetEnumerator();
		}

		public object this[int index]
		{
			get => CollectionUtils.IsWithinRanges(index, this) ? Value : throw new IndexOutOfRangeException(nameof(index));
			set => throw new NotSupportedException();
		}

		public bool IsReadOnly => true;

		public bool IsFixedSize => true;

		public int Add(object value)
		{
			throw new NotSupportedException();
		}

		public bool Contains(object value)
		{
			return Equals(Value, value);
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public int IndexOf(object value)
		{
			return Equals(Value, value) ? 0 : -1;
		}

		public void Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		public void Remove(object value)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		T IReadOnlyList<T>.this[int index] => throw new NotImplementedException();
	}

	internal interface IRepeatCollection
	{
	}
}