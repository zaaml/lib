// <copyright file="TreeAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	internal class TreeAdvisor : ITreeAdvisor
	{
		public IEnumerable GetNodes(object treeNodeData)
		{
			var treeItem = (DataTreeItem)treeNodeData;

			return treeItem.Children;
		}

		public bool IsExpanded(object treeNodeData)
		{
			var treeItem = (DataTreeItem)treeNodeData;

			return treeItem.IsExpanded;
		}
	}
}