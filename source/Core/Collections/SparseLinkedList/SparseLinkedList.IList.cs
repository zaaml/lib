// <copyright file="SparseLinkedList.IList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T> : IList<T>, IList
	{
		int IList.Add(object value)
		{
			Add((T) value);

			return Count;
		}

		bool IList.Contains(object value)
		{
			return Contains((T) value);
		}

		int IList.IndexOf(object value)
		{
			return (int) IndexOf((T) value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T) value);
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		object IList.this[int index]
		{
			get => this[index];
			set => this[index] = (T) value;
		}

		bool IList.IsFixedSize => false;

		void IList<T>.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		T IList<T>.this[int index]
		{
			get => this[index];
			set => this[index] = value;
		}

		int IList<T>.IndexOf(T item)
		{
			return (int) IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			Insert(index, item);
		}
	}
}