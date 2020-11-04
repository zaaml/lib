// <copyright file="TreeViewFocusNavigator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.TreeView.Data;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class TreeViewFocusNavigator : IndexedFocusNavigator<TreeViewControl, TreeViewItem>
	{
		private TreeViewData _treeViewData;

		public TreeViewFocusNavigator(TreeViewControl control) : base(control)
		{
			control.TreeViewDataChanged += OnTreeViewDataChanged;
			TreeViewData = control.TreeViewData;
		}

		private TreeViewControl TreeView => Control;

		private TreeViewData TreeViewData
		{
			set
			{
				if (ReferenceEquals(_treeViewData, value))
					return;

				if (_treeViewData?.DataPlainListView is INotifyCollectionChanged oldListView)
					oldListView.CollectionChanged -= OnPlainListViewCollectionChanged;

				_treeViewData = value;

				if (_treeViewData?.DataPlainListView is INotifyCollectionChanged newListView)
					newListView.CollectionChanged += OnPlainListViewCollectionChanged;
			}
		}

		public override bool HandleNavigationKey(Key key)
		{
			var handled = HandleNavigationKeyImpl(key);

			if (handled)
				TreeView.OnNavigationKeyHandled();

			return handled;
		}

		private bool HandleNavigationKeyImpl(Key key)
		{
			if (IsNavigationKey(key))
				SyncFocusCacheIndex();

			var focusedItem = FocusedItem;

			switch (key)
			{
				case Key.Left:

					// TODO Remember navigation path to the root. Restore this path on Right key down.
					if (focusedItem != null)
					{
						if (focusedItem.IsExpanded && focusedItem.HasItems)
							focusedItem.Collapse();
						else
						{
							var parentNode = focusedItem.TreeViewItemData.ActualParent;

							if (parentNode != null)
								FocusedIndex = parentNode.FlatIndex;
						}
					}

					return true;

				case Key.Right:

					focusedItem?.Expand();

					return true;

				default:

					return base.HandleNavigationKey(key);
			}
		}

		private void OnPlainListViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SyncFocusCacheIndex();
		}

		private void OnTreeViewDataChanged(object sender, ValueChangedEventArgs<TreeViewData> e)
		{
			TreeViewData = e.NewValue;
		}

		private void SyncFocusCacheIndex()
		{
			if (FocusedItemCache == null)
				return;

			var focusedIndex = GetIndexFromItem(FocusedItemCache);

			if (focusedIndex == -1)
			{
				FocusedIndex = -1;

				return;
			}

			if (ReferenceEquals(GetItemFromIndex(focusedIndex), FocusedItemCache))
				SyncFocusIndex(focusedIndex);
		}
	}
}