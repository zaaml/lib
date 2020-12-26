// <copyright file="ItemCollectionBase.IList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem> : IList
	{
		int IList.Add(object value)
		{
			return Add((TItem) value);
		}

		bool IList.Contains(object value)
		{
			return Contains((TItem) value);
		}

		void IList.Clear()
		{
			Clear();
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((TItem) value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (TItem) value);
		}

		void IList.Remove(object value)
		{
			Remove((TItem) value);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		object IList.this[int index]
		{
			get => this[index];
			set => this[index] = (TItem) value;
		}

		bool IList.IsReadOnly => IsReadOnly;

		bool IList.IsFixedSize => IsFixedSize;
	}
}