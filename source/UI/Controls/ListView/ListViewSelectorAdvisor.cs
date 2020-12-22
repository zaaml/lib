// <copyright file="ListViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class ListViewSelectorAdvisor : SelectorBaseControllerAdvisor<ListViewControl, ListViewItem, ListViewItemCollection, ListViewItemsPresenter, ListViewItemsPanel>
	{
		public ListViewSelectorAdvisor(ListViewControl listViewControl) : base(listViewControl)
		{
			ListViewControl = listViewControl;
			ListViewControl.VirtualItemCollection.AttachObserver(this);
		}

		public ListViewControl ListViewControl { get; }

		public override bool TryGetItem(int index, bool ensure, out ListViewItem item)
		{
			if (index == -1)
			{
				item = null;
				
				return false;
			}
			
			ListViewControl.EnsureVirtualItemCollection();

			var viewItemCollection = ListViewControl.VirtualItemCollection;
			
			item = ensure ? viewItemCollection.EnsureItem(index) : viewItemCollection.GetItemFromIndex(index);

			return item != null;
		}

		public override bool CanSelect(ListViewItem item)
		{
			return item.CanSelectInternal;
		}

		public override bool HasSource => ListViewControl.SourceCollection != null;

		public override bool TryGetItemBySource(object source, bool ensure, out ListViewItem item)
		{
			ListViewControl.EnsureVirtualItemCollection();

			var listViewData = ListViewControl.EnsureListViewData();
			var listViewItemData = listViewData.FindNode(source);

			if (listViewItemData == null)
			{
				item = default;

				return false;
			}

			item = listViewItemData.ListViewItem;

			if (item != null) 
				return true;

			return TryGetItem(listViewData.FindIndex(listViewItemData), ensure, out item);
		}
	}
}