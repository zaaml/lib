// <copyright file="TreeFlatListView.IList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.Core.Trees
{
	internal partial class TreeFlatListView<T> : IList
	{
		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object value)
		{
			return IndexOf((T) value) != -1;
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T) value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		object IList.this[int index]
		{
			get => GetItem(index);
			set => throw new NotSupportedException();
		}

		bool IList.IsReadOnly => true;

		bool IList.IsFixedSize => false;
	}
}