// <copyright file="ItemCollectionBase.Implementation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		private readonly List<TItem> _changeList = new List<TItem>();
		
		private protected override event NotifyCollectionChangedEventHandler CollectionChanged;

		private int CountImpl => DependencyObjectCollection.Count;

		private ItemDependencyObjectCollection DependencyObjectCollection { get; }

		internal DependencyObjectCollectionBase<TItem> InternalCollection => DependencyObjectCollection;

		private int AddImpl(TItem item)
		{
			var index = Count;

			DependencyObjectCollection.Add(item);

			if (VirtualCollection == null)
			{
				_changeList.Add(item);

				HostInsert(index, _changeList);
				RaiseInsert(index, _changeList);

				_changeList.Clear();
			}

			return index;
		}

		private void ClearImpl()
		{
			DependencyObjectCollection.Clear();

			if (VirtualCollection == null)
			{
				HostClear();
				RaiseReset();
			}
		}

		private bool ContainsImpl(TItem item)
		{
			return DependencyObjectCollection.Contains(item);
		}

		private void CopyToImpl(Array array, int index)
		{
			((ICollection) DependencyObjectCollection).CopyTo(array, index);
		}

		private void CopyToImpl(TItem[] array, int index)
		{
			DependencyObjectCollection.CopyTo(array, index);
		}

		private IEnumerator<TItem> GetEnumeratorImpl()
		{
			return DependencyObjectCollection.GetEnumerator();
		}

		private TItem GetItemImpl(int index)
		{
			return DependencyObjectCollection[index];
		}

		private int IndexOfImpl(TItem item)
		{
			return DependencyObjectCollection.IndexOf(item);
		}

		private void InsertImpl(int index, TItem item)
		{
			DependencyObjectCollection.Insert(index, item);

			OnItemAdded(item);

			if (VirtualCollection == null)
			{
				_changeList.Add(item);

				HostInsert(index, _changeList);
				RaiseInsert(index, _changeList);

				_changeList.Clear();
			}
		}

		private protected virtual void OnItemAdded(TItem item)
		{
		}

		private protected virtual void OnItemRemoved(TItem item)
		{
		}

		private void RaiseInsert(int index, List<TItem> items)
		{
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, index);

			CollectionChanged?.Invoke(this, args);

			OnCollectionChangedPrivate(this, args);
		}

		private void RaiseRemove(int index, List<TItem> items)
		{
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index);

			CollectionChanged?.Invoke(this, args);

			OnCollectionChangedPrivate(this, args);
		}

		private void RaiseReset()
		{
			var args = Constants.NotifyCollectionChangedReset;

			CollectionChanged?.Invoke(this, args);

			OnCollectionChangedPrivate(this, args);
		}

		private void RemoveAtImpl(int index)
		{
			var item = GetItemImpl(index);

			DependencyObjectCollection.RemoveAt(index);

			if (VirtualCollection == null)
			{
				_changeList.Add(item);

				HostRemove(index, _changeList);
				RaiseRemove(index, _changeList);

				_changeList.Clear();
			}
		}

		private bool RemoveImpl(TItem item)
		{
			var index = IndexOfImpl(item);

			if (index == -1)
				return false;

			DependencyObjectCollection.RemoveAt(index);

			if (VirtualCollection == null)
			{
				_changeList.Add(item);

				HostRemove(index, _changeList);
				RaiseRemove(index, _changeList);

				_changeList.Clear();
			}

			return true;
		}

		private void SetItemImpl(int index, TItem item)
		{
			RemoveAtImpl(index);
			InsertImpl(index, item);
		}

		private sealed class ItemDependencyObjectCollection : DependencyObjectCollectionBase<TItem>
		{
			public ItemDependencyObjectCollection(ItemCollectionBase<TItemsControl, TItem> itemCollection)
			{
				ItemCollection = itemCollection;
			}

			public ItemCollectionBase<TItemsControl, TItem> ItemCollection { get; }

			protected override void OnItemAdded(TItem item)
			{
				base.OnItemAdded(item);

				SetInItemCollection(item, true);

				ItemCollection.OnItemAdded(item);
			}

			protected override void OnItemRemoved(TItem item)
			{
				base.OnItemRemoved(item);

				SetInItemCollection(item, false);

				ItemCollection.OnItemRemoved(item);
			}
		}
	}
}