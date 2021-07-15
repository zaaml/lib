// <copyright file="PooledList.IList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Collections.Pooled
{
	internal partial class PooledList<T> : IList<T>, IList
	{
		int IList.Add(object value)
		{
			Add((T) value);

			return Count - 1;
		}

		bool IList.Contains(object value)
		{
			return Contains((T) value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T) value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T) value);
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		bool IList.IsFixedSize => false;

		bool IList.IsReadOnly => false;

		object IList.this[int index]
		{
			get => this[index];
			set => this[index] = (T) value;
		}
	}
}