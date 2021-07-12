// <copyright file="PooledList.ICollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Zaaml.Core.Collections.Pooled
{
	internal partial class PooledList<T> : ICollection<T>, ICollection
	{
		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if (array != null && array.Rank != 1)
				throw new ArgumentException(nameof(array));

			try
			{
				Array.Copy(_items, 0, array, arrayIndex, _size);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException(nameof(array));
			}
		}

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot
		{
			get
			{
				if (_syncRoot == null)
					Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);

				return _syncRoot;
			}
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(_items, 0, array, arrayIndex, _size);
		}

		bool ICollection<T>.IsReadOnly => false;
	}
}