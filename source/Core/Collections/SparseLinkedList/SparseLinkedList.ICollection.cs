// <copyright file="SparseLinkedList.ICollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedList<T> : ICollection
	{
		void ICollection.CopyTo(Array array, int index)
		{
			if (array is T[] typeArray)
				CopyTo(typeArray, index);
			else
				throw new InvalidOperationException();
		}

		object ICollection.SyncRoot { get; } = new object();

		bool ICollection.IsSynchronized => false;

		void ICollection<T>.Clear()
		{
			Clear();
		}

		bool ICollection<T>.Contains(T item)
		{
			return Contains(item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item)
		{
			return Remove(item);
		}

		public bool IsReadOnly => false;
	}
}