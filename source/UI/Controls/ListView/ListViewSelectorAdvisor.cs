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
			ListView = listViewControl;
		}

		public ListViewControl ListView { get; }

		public override bool TryGetItem(int index, out ListViewItem item)
		{
			ListView.EnsureVirtualItemCollection();

			item = ListView.VirtualItemCollection.EnsureItem(index);

			return item != null;
		}

		public override bool TryGetItemBySource(object itemSource, out ListViewItem item)
		{
			ListView.EnsureVirtualItemCollection();

			var listViewData = ListView.EnsureListViewData();
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

				item = ListView.VirtualItemCollection.EnsureItem(index);
			}

			return item != null;
		}
	}
}