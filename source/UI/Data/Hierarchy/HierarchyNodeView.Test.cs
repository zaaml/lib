// <copyright file="HierarchyNodeView.Test.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if TEST
using System;
using System.Linq;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;

namespace Zaaml.UI.Data.Hierarchy
{
	internal abstract partial class HierarchyNodeView<THierarchy, TNodeCollection, TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		#region  Methods

		internal void Verify()
		{
			var treeFlatCountAdvisor = new DelegateTreeEnumeratorAdvisor<TNode>(n => n.Nodes?.GetEnumerator() ?? Enumerable.Empty<TNode>().GetEnumerator());
			var treeVisibleFlatCountAdvisor = new DelegateTreeEnumeratorAdvisor<TNode>(n => n.IsExpanded ? n.Nodes.GetEnumerator() : Enumerable.Empty<TNode>().GetEnumerator());

			var flatCount = Nodes != null ? TreeEnumerator.GetEnumerator(Nodes, treeFlatCountAdvisor).Enumerate().Count() : 0;
			var visibleCount = IsExpanded ? TreeEnumerator.GetEnumerator(Nodes, treeVisibleFlatCountAdvisor).Enumerate().Count() : 0;

			if (FlatCount != flatCount)
				throw new Exception(nameof(FlatCount));

			if (VisibleFlatCount != visibleCount)
				throw new Exception(nameof(VisibleFlatCount));
		}

		#endregion
	}
}
#endif