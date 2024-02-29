// <copyright file="HierarchyView.Cursor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core.Trees;

namespace Zaaml.UI.Data.Hierarchy
{
	internal abstract partial class HierarchyView<THierarchy, TNodeCollection, TNode>
	{
		public int FindDataIndex(object data)
		{
			return Cursor.FindDataIndex(data);
		}

		private class NodeCursor : TreeFlatCursor<TNode>
		{
			public NodeCursor(THierarchy hierarchy)
			{
				Hierarchy = hierarchy;
			}

			private THierarchy Hierarchy { get; }

			public int FindDataIndex(object data)
			{
				return IndexOf(n => ReferenceEquals(n.Data, data));
			}

			protected override int CalcFlatCount()
			{
				return Hierarchy.VisibleFlatCount;
			}

			protected override IReadOnlyList<TNode> GetNodeChildren(TNode node)
			{
				return node == null ? Hierarchy.Nodes : node.Nodes;
			}

			protected override bool IsExpanded(TNode node)
			{
				return node.IsExpanded;
			}
		}
	}
}