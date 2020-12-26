// <copyright file="FocusableItemsControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public abstract class FocusableItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel> : ItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>, IIndexedFocusNavigatorAdvisor<TItem>
		where TItem : System.Windows.Controls.Control
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPresenter : ItemsPresenterBase<TControl, TItem, TCollection, TPanel>
		where TPanel : ItemsPanel<TItem>
		where TControl : System.Windows.Controls.Control
	{
		private FocusNavigator<TItem> _focusNavigator;
		private WeakReference<TItem> _weakFocusedItem;

		protected TItem FocusedItem => FocusNavigator.FocusedItem;

		private TItem FocusedItemPrivate
		{
			get => _weakFocusedItem != null && _weakFocusedItem.TryGetTarget(out var item) ? item : default;
			set
			{
				var focusedItem = FocusedItemPrivate;

				if (ReferenceEquals(focusedItem, value))
					return;

				if (focusedItem != null)
					ItemCollection.UnlockItemInternal(focusedItem);

				_weakFocusedItem = value != null ? new WeakReference<TItem>(value) : null;

				if (value != null)
					ItemCollection.LockItemInternal(value);
			}
		}

		internal FocusNavigator<TItem> FocusNavigator => _focusNavigator ??= CreateFocusNavigator();

		protected virtual bool HasFocus => this.HasFocus();

		internal virtual FocusNavigator<TItem> CreateFocusNavigator()
		{
			return new IndexedFocusNavigator<FocusableItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>, TItem>(this);
		}

		protected virtual void FocusItem(TItem item)
		{
			var focusScope = FocusManager.GetFocusScope(item);

			if (focusScope != null && ReferenceEquals(item, focusScope) == false)
				FocusManager.SetFocusedElement(focusScope, item);
		}

		internal void FocusItemInternal(TItem item)
		{
			FocusItem(item);
		}
		
		protected bool IsNavigationKey(Key key)
		{
			return FocusNavigator.IsNavigationKey(key);
		}

		internal override void OnItemAttachedInternal(TItem item)
		{
			base.OnItemAttachedInternal(item);

			FocusNavigator.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(TItem item)
		{
			FocusNavigator.OnItemDetachedInternal(item);

			base.OnItemDetachedInternal(item);
		}

		private protected virtual void OnItemGotFocus(TItem item)
		{
			FocusNavigator.OnItemGotFocusInternal(item);
		}

		internal void OnItemGotFocusInternal(TItem item)
		{
			OnItemGotFocus(item);

			FocusedItemPrivate = item;
		}

		private protected virtual void OnItemLostFocus(TItem item)
		{
			FocusNavigator.OnItemLostFocusInternal(item);
		}

		internal void OnItemLostFocusInternal(TItem item)
		{
			if (ReferenceEquals(item, FocusedItemPrivate))
				FocusedItemPrivate = null;

			OnItemLostFocus(item);
		}

		internal void SyncFocusedIndex(int index)
		{
			throw Error.Refactoring;
			//_focusedIndex = index.Clamp(-1, ItemsCount);
		}

		bool IFocusNavigatorAdvisor<TItem>.IsVirtualizing => IsVirtualizing;

		int IFocusNavigatorAdvisor<TItem>.ItemsCount => ItemsCount;

		Orientation IFocusNavigatorAdvisor<TItem>.LogicalOrientation => LogicalOrientation;

		bool IFocusNavigatorAdvisor<TItem>.HasFocus => HasFocus;

		bool IFocusNavigatorAdvisor<TItem>.IsItemsHostVisible => IsItemsHostVisible;

		TItem IIndexedFocusNavigatorAdvisor<TItem>.EnsureItem(int index)
		{
			return ItemCollectionOverride.EnsureItem(index);
		}

		TItem IIndexedFocusNavigatorAdvisor<TItem>.GetItemFromIndex(int index)
		{
			return GetItemFromIndex(index);
		}

		int IIndexedFocusNavigatorAdvisor<TItem>.GetIndexFromItem(TItem item)
		{
			return GetIndexFromItem(item);
		}

		void IIndexedFocusNavigatorAdvisor<TItem>.LockItem(TItem item)
		{
			ItemCollectionOverride.LockItem(item);
		}

		void IIndexedFocusNavigatorAdvisor<TItem>.UnlockItem(TItem item)
		{
			ItemCollectionOverride.UnlockItem(item);
		}
	}
}