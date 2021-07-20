// <copyright file="VirtualListViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ListView.Data;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class VirtualListViewItemCollection : VirtualItemCollection<ListViewItem>, IItemCollection<ListViewItem>
	{
		private ListViewData _listViewData;

		public VirtualListViewItemCollection(ListViewControl listViewControl)
		{
			ListViewControl = listViewControl;
		}

		public ListViewItemCollection Items => ListViewControl.ItemCollection;

		public ListViewControl ListViewControl { get; }

		public ListViewData ListViewData
		{
			get => _listViewData;
			set
			{
				if (ReferenceEquals(_listViewData, value))
					return;

				_listViewData = value;

				Init(_listViewData.FlatListView, ReferenceEquals(_listViewData.Source, ListViewControl.ItemCollection) ? OperatingMode.Real : OperatingMode.Virtual);
			}
		}

		protected override void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnSourceCollectionChanged(e);

			ListViewControl.InvalidatePanelInternal();
		}

		protected override void OnGeneratedItemAttached(int index, ListViewItem item)
		{
			base.OnGeneratedItemAttached(index, item);

			item.ListViewItemData = ListViewData.GetNode(index);

			var itemsControl = (IItemsControl<ListViewItem>) ListViewControl;

			if (ItemCollectionBase.GetInItemCollection(item))
				ListViewControl.DetachLogical(item);
			
			itemsControl.OnItemAttaching(item);
			itemsControl.OnItemAttached(item);
		}

		protected override void OnGeneratedItemDetached(int index, ListViewItem item)
		{
			item.ListViewItemData = null;

			var itemsControl = (IItemsControl<ListViewItem>) ListViewControl;

			itemsControl.OnItemDetaching(item);
			itemsControl.OnItemDetached(item);

			if (ItemCollectionBase.GetInItemCollection(item))
				ListViewControl.AttachLogical(item);

			base.OnGeneratedItemDetached(index, item);
		}

		int IItemCollection<ListViewItem>.ActualCount => ListViewData?.VisibleFlatCount ?? 0;

		public override int GetIndexFromItem(ListViewItem item)
		{
			if (ListViewData == null || item?.ListViewItemData == null)
				return base.GetIndexFromItem(item);

			return ListViewData.FindIndex(item.ListViewItemData);
		}

		public void BringIntoView(BringIntoViewRequest<ListViewItem> bringIntoViewRequest)
		{
			var virtualPanel = (IItemsHost<ListViewItem>) ListViewControl?.ItemsPresenterInternal?.ItemsHostInternal;

			virtualPanel?.BringIntoView(bringIntoViewRequest);
		}
	}
}