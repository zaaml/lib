// <copyright file="ListViewFocusNavigator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ListView.Data;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class ListViewFocusNavigator : IndexedFocusNavigator<ListViewControl, ListViewItem>
	{
		private ListViewData _listViewData;

		public ListViewFocusNavigator(ListViewControl control) : base(control)
		{
			control.ListViewDataChanged += OnListViewDataChanged;
			ListViewData = control.ListViewData;
		}

		private ListViewControl ListView => Control;

		private ListViewData ListViewData
		{
			set
			{
				if (ReferenceEquals(_listViewData, value))
					return;

				if (_listViewData?.FlatListView is INotifyCollectionChanged oldListView)
					oldListView.CollectionChanged -= OnPlainListViewCollectionChanged;

				_listViewData = value;

				if (_listViewData?.FlatListView is INotifyCollectionChanged newListView)
					newListView.CollectionChanged += OnPlainListViewCollectionChanged;
			}
		}

		public override bool HandleNavigationKey(Key key)
		{
			var handled = HandleNavigationKeyImpl(key);

			if (handled)
				ListView.OnNavigationKeyHandled();

			return handled;
		}

		private bool HandleNavigationKeyImpl(Key key)
		{
			if (IsNavigationKey(key))
				SyncFocusCacheIndex();

			return base.HandleNavigationKey(key);
		}

		private void OnListViewDataChanged(object sender, ValueChangedEventArgs<ListViewData> e)
		{
			ListViewData = e.NewValue;
		}

		private void OnPlainListViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SyncFocusCacheIndex();
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