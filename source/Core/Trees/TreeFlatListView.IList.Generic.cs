// <copyright file="TreeFlatListView.IList.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal partial class TreeFlatListView<T> : IList<T>, IReadOnlyList<T> where T : class
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		int ICollection<T>.Count => Count;

		bool ICollection<T>.IsReadOnly => true;

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Contains(T item)
		{
			return IndexOf(item) != -1;
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		T IList<T>.this[int index]
		{
			get => ElementAt(index);
			set => throw new NotSupportedException();
		}

		int IList<T>.IndexOf(T item)
		{
			return IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		int IReadOnlyCollection<T>.Count => Count;

		T IReadOnlyList<T>.this[int index] => ElementAt(index);
	}
}