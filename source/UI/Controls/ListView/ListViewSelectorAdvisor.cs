// <copyright file="ListViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class ListViewSelectorAdvisor : ItemCollectionSelectorAdvisor<ListViewControl, ListViewItem>
	{
		public ListViewSelectorAdvisor(ListViewControl listViewControl) : base(listViewControl, listViewControl.Items)
		{
			ListViewControl = listViewControl;
		}

		public ListViewControl ListViewControl { get; }

		public override bool TryGetItem(int index, out ListViewItem item)
		{
			ListViewControl.EnsureVirtualItemCollection();

			item = ListViewControl.VirtualItemCollection.EnsureItem(index);

			return item != null;
		}

		public override bool TryGetItemBySource(object itemSource, out ListViewItem item)
		{
			ListViewControl.EnsureVirtualItemCollection();

			var listViewData = ListViewControl.EnsureListViewData();
			var listViewItemData = listViewData.FindNode(itemSource);

			if (listViewItemData == null)
			{
				item = default;

				return false;
			}

			item = listViewItemData.ListViewItem;

			if (item == null)
			{
				var index = listViewData.FindIndex(listViewItemData);

				item = ListViewControl.VirtualItemCollection.EnsureItem(index);
			}

			return item != null;
		}
	}
}