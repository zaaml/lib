// <copyright file="TreeViewItemData.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.TreeView.Data
{
	[DebuggerDisplay("{ToString()}")]
	internal sealed class TreeViewItemData : HierarchyNodeView<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>
	{
		public TreeViewItemData(TreeViewData treeViewData, TreeViewItemData parent) : base(treeViewData, parent)
		{
		}

		public int FlatIndex => TreeViewData.FindIndex(this);

		public TreeViewData TreeViewData => Hierarchy;

		public TreeViewItem TreeViewItem { get; set; }

		protected override TreeViewItemData CreateChildNodeCore(object nodeData)
		{
			var treeNode = new TreeViewItemData(TreeViewData, this)
			{
				Data = nodeData,
			};

			return treeNode;
		}

		protected override void OnIsExpandedChanged()
		{
			base.OnIsExpandedChanged();

			TreeViewItem?.UpdateIsExpandedInternal();
		}

		protected override TreeViewItemDataCollection CreateNodeCollectionCore()
		{
			return new TreeViewItemDataCollection(TreeViewData, this, CreateChildNode);
		}

		public override string ToString()
		{
			return Data?.ToString() ?? "Empty";
		}
	}
}