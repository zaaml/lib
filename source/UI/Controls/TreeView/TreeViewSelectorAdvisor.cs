// <copyright file="TreeViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class TreeViewSelectorAdvisor : SelectorBaseControllerAdvisor<TreeViewControl, TreeViewItem, TreeViewItemRootCollection, TreeViewItemsPresenter, TreeViewItemsPanel>
	{
		public TreeViewSelectorAdvisor(TreeViewControl treeViewControl) : base(treeViewControl)
		{
			TreeViewControl = treeViewControl;
			TreeViewControl.VirtualItemCollection.AttachObserver(this);
		}

		public override bool HasSource => TreeViewControl.SourceCollection != null;

		public TreeViewControl TreeViewControl { get; }

		public override bool CanSelectItem(TreeViewItem item)
		{
			return item.ActualCanSelect;
		}

		public override object GetSource(TreeViewItem item)
		{
			if (item == null)
				return null;

			if (item.TreeViewItemData != null)
				return item.TreeViewItemData.Data;

			return base.GetSource(item);
		}

		public override object GetValue(TreeViewItem item, object source)
		{
			if (item != null && source == null)
				return item.Value;

			return TreeViewControl.GetValueInternal(null, source);
		}

		public override bool TryGetItem(object source, bool ensure, out TreeViewItem item)
		{
			TreeViewControl.EnsureVirtualItemCollection();

			var treeViewData = TreeViewControl.EnsureTreeViewData();
			var treeViewItemData = treeViewData.FindNode(source);

			if (treeViewItemData == null)
			{
				item = default;

				return false;
			}

			item = treeViewItemData.TreeViewItem;

			if (item == null && ensure)
			{
				treeViewData.ExpandRoot(treeViewItemData);

				var current = treeViewItemData;

				while (current != null)
				{
					var index = treeViewData.FindIndex(current);

					TreeViewControl.VirtualItemCollection.EnsureItem(index);

					current = current.Parent;
				}
			}

			return item != null;
		}
	}
}