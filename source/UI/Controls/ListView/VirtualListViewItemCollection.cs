// <copyright file="VirtualListViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ListView.Data;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class VirtualListViewItemCollection : VirtualItemCollection<ListViewControl, ListViewItem>, IItemCollection<ListViewItem>
	{
		private ListViewData _listViewData;

		public VirtualListViewItemCollection(ListViewControl listViewControl)
		{
			ListViewControl = listViewControl;
		}

		public ListViewItemCollection Items => ListViewControl.Items;

		public ListViewControl ListViewControl { get; }

		public ListViewData ListViewData
		{
			get => _listViewData;
			set
			{
				if (ReferenceEquals(_listViewData, value))
					return;

				_listViewData = value;

				Source = _listViewData.DataPlainListView;
			}
		}

		protected override void ObservableSourceOnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.ObservableSourceOnCollectionChanged(e);

			ListViewControl.InvalidatePanelInternal();
		}

		protected override void OnGeneratedItemAttached(int index, ListViewItem item)
		{
			base.OnGeneratedItemAttached(index, item);

			item.ListViewItemData = ListViewData.GetNode(index);

			var itemsControl = (IItemsControl<ListViewItem>)ListViewControl;

			itemsControl.OnItemAttaching(item);
			itemsControl.OnItemAttached(item);
		}

		protected override void OnGeneratedItemDetached(int index, ListViewItem item)
		{
			item.ListViewItemData = null;

			var itemsControl = (IItemsControl<ListViewItem>)ListViewControl;

			itemsControl.OnItemDetaching(item);
			itemsControl.OnItemDetached(item);

			base.OnGeneratedItemDetached(index, item);
		}

		public int ActualCount => ListViewData?.VisibleFlatCount ?? 0;

		public override int GetIndexFromItem(ListViewItem item)
		{
			if (ListViewData == null || item?.ListViewItemData == null)
				return -1;

			return ListViewData.FindIndex(item.ListViewItemData);
		}

		public void BringIntoView(int index)
		{
			var virtualPanel = (IItemsHost<ListViewItem>)ListViewControl?.ItemsPresenterInternal?.ItemsHostInternal;

			virtualPanel?.BringIntoView(new BringIntoViewRequest<ListViewItem>(index, ListViewControl.DefaultBringIntoViewMode));
		}
	}
}