// <copyright file="TreeViewSelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class TreeViewSelectorController : SelectorController<TreeViewControl, TreeViewItem>
	{
		public TreeViewSelectorController(TreeViewControl treeViewControl) : base(treeViewControl, new TreeViewSelectorAdvisor(treeViewControl))
		{
		}
	}
}