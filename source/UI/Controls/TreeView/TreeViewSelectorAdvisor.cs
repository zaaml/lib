// <copyright file="TreeViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class TreeViewSelectorAdvisor : ItemCollectionSelectorAdvisor<TreeViewControl, TreeViewItem>
	{
		public TreeViewSelectorAdvisor(TreeViewControl treeViewControl) : base(treeViewControl, treeViewControl.Items)
		{
			TreeView = treeViewControl;
		}

		public TreeViewControl TreeView { get; }

		public override object GetItemSource(TreeViewItem item)
		{
			return item?.TreeViewItemData?.Data;
		}

		public override bool TryGetItemBySource(object itemSource, out TreeViewItem item)
		{
			TreeView.EnsureVirtualItemCollection();

			var treeViewData = TreeView.EnsureTreeViewData();
			var treeViewItemData = treeViewData.FindNode(itemSource);

			if (treeViewItemData == null)
			{
				item = default;

				return false;
			}

			item = treeViewItemData.TreeViewItem;

			if (item == null)
			{
				treeViewData.ExpandRoot(treeViewItemData);

				var current = treeViewItemData;

				while (current != null)
				{
					var index = treeViewData.FindIndex(current);

					TreeView.VirtualItemCollection.EnsureItem(index);

					current = current.Parent;
				}
			}

			return item != null;
		}
	}
}