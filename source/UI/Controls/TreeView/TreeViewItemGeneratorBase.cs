// <copyright file="TreeViewItemGeneratorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeViewItemGeneratorBase : ItemGenerator<TreeViewItem>, ITreeViewAdvisor
	{
		protected abstract IEnumerable GetTreeNodes(object treeNodeData);

		protected abstract bool IsExpanded(object treeNodeData);

		IEnumerable ITreeViewAdvisor.GetNodes(object treeNodeData)
		{
			return GetTreeNodes(treeNodeData);
		}

		bool ITreeViewAdvisor.IsExpanded(object treeNodeData)
		{
			return IsExpanded(treeNodeData);
		}
	}
}