// <copyright file="IndexedFocusNavigator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Extensions;
using Zaaml.UI.Utils;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Core
{
	internal interface IFocusNavigatorAdvisor<TItem> where TItem : NativeControl
	{
		bool HasFocus { get; }

		bool IsItemsHostVisible { get; }

		bool IsVirtualizing { get; }

		int ItemsCount { get; }

		Orientation LogicalOrientation { get; }
	}

	internal interface IScrollableFocusNavigatorAdvisor<TItem> : IFocusNavigatorAdvisor<TItem> where TItem : NativeControl
	{
		ScrollViewControl ScrollView { get; }
	}

	internal interface IIndexedFocusNavigatorAdvisor<TItem> : IFocusNavigatorAdvisor<TItem> where TItem : NativeControl
	{
		TItem EnsureItem(int index);

		int GetIndexFromItem(TItem item);

		TItem GetItemFromIndex(int index);

		void LockItem(TItem item);

		void UnlockItem(TItem item);
	}

	internal interface IIndexedScrollableFocusNavigatorAdvisor<TItem> : IFocusNavigatorAdvisor<TItem> where TItem : NativeControl
	{
		bool IsOnCurrentPage(int index);

		void ScrollIntoView(BringIntoViewRequest<TItem> bringIntoViewRequest);
	}

	internal class IndexedFocusNavigator<TControl, TItem> : FocusNavigator<TControl, TItem>
		where TControl : NativeControl, IIndexedFocusNavigatorAdvisor<TItem>
		where TItem : NativeControl
	{
		private int _focusedIndex = -1;
		private TItem _lockedItem;
		private bool _suspendFocus;

		public IndexedFocusNavigator(TControl control) : base(control)
		{
		}

		public int FocusedIndex
		{
			get => _focusedIndex;
			set
			{
				if (_focusedIndex == value)
					return;

				var oldFocusedIndex = value;
				TItem oldFocusedItem = null;
				TItem newFocusedItem = null;

				try
				{
					if (_focusedIndex != -1)
						oldFocusedItem = IsValidIndex(_focusedIndex) ? GetItemFromIndex(_focusedIndex) : default;

					_focusedIndex = CoerceIndex(value);

					if (_focusedIndex != -1)
						newFocusedItem = Control.EnsureItem(_focusedIndex);

					if (ReferenceEquals(oldFocusedItem, newFocusedItem))
						return;

					LockedItem = newFocusedItem;

					if (newFocusedItem == null)
					{
						// TODO Investigate issue (Editable DropDownTreeView textbox loses focus on keyboard navigation)
						//if (_suspendFocus == false && Control.HasFocus == false)
						//	Control.Focus();
					}
					else
					{
						if (newFocusedItem.IsMouseOver == false)
							ScrollIntoView(new BringIntoViewRequest<TItem>(FocusedIndex));

						if (_suspendFocus == false)
							FocusItem(newFocusedItem, false);
					}
				}
				finally
				{
					if (ReferenceEquals(oldFocusedItem, newFocusedItem) == false)
						OnFocusedItemChanged(oldFocusedItem, newFocusedItem);

					if (oldFocusedIndex != _focusedIndex)
						OnFocusedIndexChanged(oldFocusedIndex, _focusedIndex);
				}
			}
		}

		public override TItem FocusedItem
		{
			get => IsValidIndex(FocusedIndex) ? GetItemFromIndex(FocusedIndex) : default;
			set => FocusedIndex = GetIndexFromItem(value);
		}

		public TItem FocusedItemCache { get; private set; }

		private TItem LockedItem
		{
			get => _lockedItem;
			set
			{
				if (ReferenceEquals(LockedItem, value))
					return;

				if (_lockedItem != null)
					Control.UnlockItem(_lockedItem);

				_lockedItem = value;

				if (_lockedItem != null)
					Control.LockItem(_lockedItem);
			}
		}

		private IIndexedScrollableFocusNavigatorAdvisor<TItem> ScrollableControl => Control as IIndexedScrollableFocusNavigatorAdvisor<TItem>;

		public override void ClearFocus()
		{
			base.ClearFocus();

			FocusedItemCache = null;
			FocusedIndex = -1;
		}

		private int CoerceIndex(int index)
		{
			if (index == -1)
				return -1;

			var itemsCount = ItemsCount;

			if (itemsCount == 0)
				return -1;

			return index.Clamp(0, itemsCount - 1);
		}

		public override void EnsureFocus()
		{
			if (FocusedIndex == -1)
				FocusedIndex = 0;
			else
				FocusItem(FocusedItem, true);
		}

		protected int GetFirstItemOnCurrentPage(int startIndex, Direction direction)
		{
			var inc = direction == Direction.Forward ? 1 : -1;
			var index = CoerceIndex(startIndex);
			var resultIndex = -1;

			while (index >= 0 && index < ItemsCount && IsOnCurrentPage(index) == false)
			{
				resultIndex = index;
				index += inc;
			}

			while (index >= 0 && index < ItemsCount && IsOnCurrentPage(index))
			{
				resultIndex = index;
				index += inc;
			}

			return resultIndex;
		}

		protected int GetIndexFromItem(TItem item)
		{
			return Control.GetIndexFromItem(item);
		}

		protected TItem GetItemFromIndex(int index)
		{
			return Control.GetItemFromIndex(index);
		}

		public override bool HandleNavigationKey(Key key)
		{
			var isLTR = Control.FlowDirection == FlowDirection.LeftToRight;

			switch (key)
			{
				case Key.PageUp:

					if (IsItemsHostVisible)
					{
						NavigateByPage(Direction.Backward);

						return true;
					}

					return false;

				case Key.PageDown:

					if (IsItemsHostVisible)
					{
						NavigateByPage(Direction.Forward);

						return true;
					}

					return false;

				case Key.End:

					Navigate(Direction.Forward);

					return true;

				case Key.Home:

					Navigate(Direction.Backward);

					return true;

				case Key.Left:

					NavigateByLine(isLTR ? Direction.Backward : Direction.Forward);

					return true;

				case Key.Up:

					NavigateByLine(Direction.Backward);

					return true;

				case Key.Right:

					NavigateByLine(isLTR ? Direction.Forward : Direction.Backward);

					return true;

				case Key.Down:

					NavigateByLine(Direction.Forward);

					return true;
			}

			return false;
		}

		private bool IsOnCurrentPage(int index)
		{
			return ScrollableControl?.IsOnCurrentPage(index) ?? true;
		}

		private bool IsValidIndex(int index)
		{
			var itemsCount = ItemsCount;

			if (itemsCount == 0)
				return false;

			return index >= 0 && index < itemsCount;
		}

		protected void Navigate(Direction direction)
		{
			var selectDirection = direction == Direction.Backward ? SelectDirection.First : SelectDirection.Last;

			FocusedIndex = SelectNextHelper.SelectNext(FocusedIndex, ItemsCount, selectDirection, false);
		}

		internal virtual void NavigateByLine(Direction direction)
		{
			FocusedIndex = SelectNextHelper.SelectNext(FocusedIndex, ItemsCount, direction == Direction.Forward ? SelectDirection.Next : SelectDirection.Prev, false);
		}

		internal virtual void NavigateByPage(Direction direction)
		{
			if (FocusedIndex == -1)
			{
				FocusedIndex = CoerceIndex(FocusedIndex);

				return;
			}

			var focusedItem = FocusedItem;

			if (focusedItem != null && IsOnCurrentPage(FocusedIndex) == false)
				ScrollIntoView(new BringIntoViewRequest<TItem>(FocusedIndex));

			if (focusedItem == null)
			{
				FocusedIndex = GetFirstItemOnCurrentPage(FocusedIndex, direction);

				return;
			}

			var currentPageItem = GetFirstItemOnCurrentPage(FocusedIndex, direction);

			if (currentPageItem != FocusedIndex)
			{
				FocusedIndex = currentPageItem;

				return;
			}

			if (ScrollView != null)
			{
				ScrollView.AsOriented(LogicalOrientation).NavigatePage(direction);

				ScrollView.UpdateLayout();
			}

			FocusedIndex = GetFirstItemOnCurrentPage(FocusedIndex, direction);
		}

		protected virtual void OnFocusedIndexChanged(int oldFocusedIndex, int newFocusedIndex)
		{
		}

		protected virtual void OnFocusedItemChanged(TItem oldFocusedItem, TItem newFocusedItem)
		{
			FocusedItemCache = newFocusedItem;
		}

		protected override void OnItemAttached(TItem item)
		{
			if (FocusedIndex == -1)
				return;

			if (FocusHelper.HasKeyboardFocus(Control) && GetIndexFromItem(item) == FocusedIndex)
				FocusItem(item, false);
		}

		protected override void OnItemDetached(TItem item)
		{
		}

		protected override void OnItemGotFocus(TItem item)
		{
			try
			{
				_suspendFocus = true;

				FocusedIndex = GetIndexFromItem(item);
			}
			finally
			{
				_suspendFocus = false;
			}
		}

		private void ScrollIntoView(BringIntoViewRequest<TItem> bringIntoViewRequest)
		{
			ScrollableControl?.ScrollIntoView(bringIntoViewRequest);
		}

		protected void SyncFocusIndex(int focusIndex)
		{
			_focusedIndex = focusIndex;
		}
	}
}