// <copyright file="DefaultTreeViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Linq;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class DefaultTreeViewItemGenerator : TreeViewItemGeneratorBase, IDelegatedGenerator<TreeViewItem>
	{
		protected override void AttachItem(TreeViewItem item, object itemSource)
		{
			Implementation.AttachItem(item, itemSource);
		}

		protected override TreeViewItem CreateItem(object itemSource)
		{
			return Implementation.CreateItem(itemSource);
		}

		protected override void DetachItem(TreeViewItem item, object itemSource)
		{
			Implementation.DetachItem(item, itemSource);
		}

		protected override void DisposeItem(TreeViewItem item, object itemSource)
		{
			Implementation.DisposeItem(item, itemSource);
		}

		protected override IEnumerable GetTreeNodes(object treeNodeData)
		{
			return Enumerable.Empty<object>();
		}

		protected override bool IsExpanded(object treeNodeData)
		{
			return false;
		}

		public IItemGenerator<TreeViewItem> Implementation { get; set; }
	}
}