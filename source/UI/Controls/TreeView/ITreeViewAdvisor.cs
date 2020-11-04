// <copyright file="ITreeAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.UI.Controls.TreeView
{
	internal interface ITreeViewAdvisor
	{
		IEnumerable GetNodes(object treeNodeData);

		bool IsExpanded(object treeNodeData);
	}
}