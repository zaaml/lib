// <copyright file="TreeViewControlAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class TreeViewControlAdvisor : ITreeViewAdvisor
	{
		private readonly ITreeViewAdvisor _actualAdvisor;

		public TreeViewControlAdvisor(ITreeViewAdvisor actualAdvisor)
		{
			_actualAdvisor = actualAdvisor ?? DummyTreeAdvisor.Instance;
		}

		public IEnumerable GetNodes(object treeNodeData)
		{
			if (treeNodeData is TreeViewItem treeViewItem)
				return treeViewItem.SourceCollection ?? treeViewItem.Items;

			return _actualAdvisor.GetNodes(treeNodeData);
		}

		public bool IsExpanded(object treeNodeData)
		{
			var treeViewItem = treeNodeData as TreeViewItem;

			return treeViewItem?.IsExpanded ?? _actualAdvisor.IsExpanded(treeNodeData);
		}
	}
}