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
		#region Fields

		private readonly List<TItem> _changeList = new List<TItem>();
		private readonly DependencyObjectCollectionRaw<TItem> _dependencyObjectCollection = new DependencyObjectCollectionRaw<TItem>();
		private event NotifyCollectionChangedEventHandler CollectionChangedImpl;

		#endregion

		#region Properties

		private int CountImpl => _dependencyObjectCollection.Count;

		#endregion

		#region  Methods

		private int AddImpl(TItem item)
		{
			var index = Count;

			_dependencyObjectCollection.Add(item);

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
			_dependencyObjectCollection.Clear();

			if (VirtualCollection == null)
			{
				HostClear();
				RaiseReset();
			}
		}

		private bool ContainsImpl(TItem item)
		{
			return _dependencyObjectCollection.Contains(item);
		}

		private void CopyToImpl(Array array, int index)
		{
			((ICollection) _dependencyObjectCollection).CopyTo(array, index);
		}

		private void CopyToImpl(TItem[] array, int index)
		{
			_dependencyObjectCollection.CopyTo(array, index);
		}

		private IEnumerator<TItem> GetEnumeratorImpl()
		{
			return _dependencyObjectCollection.GetEnumerator();
		}

		private TItem GetItemImpl(int index)
		{
			return _dependencyObjectCollection[index];
		}

		private int IndexOfImpl(TItem item)
		{
			return _dependencyObjectCollection.IndexOf(item);
		}

		private void InsertImpl(int index, TItem item)
		{
			_dependencyObjectCollection.Insert(index, item);

			if (VirtualCollection == null)
			{
				_changeList.Add(item);

				HostInsert(index, _changeList);
				RaiseInsert(index, _changeList);

				_changeList.Clear();
			}
		}

		private void RaiseInsert(int index, List<TItem> items)
		{
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, index);

			CollectionChangedImpl?.Invoke(this, args);

			OnCollectionChangedPrivate(this, args);
		}

		private void RaiseRemove(int index, List<TItem> items)
		{
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index);

			CollectionChangedImpl?.Invoke(this, args);

			OnCollectionChangedPrivate(this, args);
		}

		private void RaiseReset()
		{
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

			CollectionChangedImpl?.Invoke(this, args);

			OnCollectionChangedPrivate(this, args);
		}

		private void RemoveAtImpl(int index)
		{
			var item = GetItemImpl(index);

			_dependencyObjectCollection.RemoveAt(index);

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

			_dependencyObjectCollection.RemoveAt(index);

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

		#endregion
	}
}