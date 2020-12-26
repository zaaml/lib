// <copyright file="ItemCollectionBase.Interface.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		public override int Count => CountImpl;

		private static bool IsFixedSize => false;

		public bool IsReadOnly => false;

		private static bool IsSynchronized => false;

		public override TItem this[int index]
		{
			get => GetItemImpl(index);
			set => SetItemImpl(index, value);
		}

		private object SyncRoot => this;

		public int Add(TItem item)
		{
			return AddImpl(item);
		}

		public void Clear()
		{
			ClearImpl();
		}

		private bool Contains(TItem item)
		{
			return ContainsImpl(item);
		}

		private void CopyTo(Array array, int index)
		{
			CopyToImpl(array, index);
		}

		private void CopyTo(TItem[] array, int index)
		{
			CopyToImpl(array, index);
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return GetEnumeratorImpl();
		}

		public int IndexOf(TItem item)
		{
			return IndexOfImpl(item);
		}

		public void Insert(int index, TItem item)
		{
			InsertImpl(index, item);
		}

		public bool Remove(TItem item)
		{
			return RemoveImpl(item);
		}

		public void RemoveAt(int index)
		{
			RemoveAtImpl(index);
		}
	}
}