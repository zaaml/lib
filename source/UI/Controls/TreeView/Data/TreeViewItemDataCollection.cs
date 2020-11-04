// <copyright file="TreeViewItemDataCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Controls.TreeView.Data
{
	internal sealed class TreeViewItemDataCollection : HierarchyNodeViewCollection<TreeViewData, TreeViewItemDataCollection, TreeViewItemData>
	{
		public TreeViewItemDataCollection(TreeViewData treeViewData, TreeViewItemData parentViewItemData, Func<object, TreeViewItemData> treeViewItemDadaFactory) : base(treeViewData, parentViewItemData, treeViewItemDadaFactory)
		{
		}
	}
}