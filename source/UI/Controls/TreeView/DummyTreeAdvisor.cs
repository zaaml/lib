// <copyright file="DummyTreeAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Linq;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class DummyTreeAdvisor : ITreeViewAdvisor
	{
		public static readonly DummyTreeAdvisor Instance = new DummyTreeAdvisor();

		private DummyTreeAdvisor()
		{
		}

		public IEnumerable GetNodes(object treeNodeData)
		{
			return Enumerable.Empty<object>();
		}

		public bool IsExpanded(object treeNodeData)
		{
			return false;
		}
	}
}