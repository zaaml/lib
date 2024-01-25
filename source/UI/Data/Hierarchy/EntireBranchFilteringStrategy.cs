// <copyright file="EntireBranchFilteringStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Data.Hierarchy
{
	internal sealed class EntireBranchFilteringStrategy<THierarchy, TNodeCollection, TNode> : FilteringStrategy<THierarchy, TNodeCollection, TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		public static readonly FilteringStrategy<THierarchy, TNodeCollection, TNode> Instance = new EntireBranchFilteringStrategy<THierarchy, TNodeCollection, TNode>();

		private EntireBranchFilteringStrategy()
		{
		}

		public override IEnumerable<TNode> Filter(TNodeCollection collection)
		{
			foreach (var node in collection.SourceCollection)
			{
				if (node.Nodes.FilteredCollectionInternal.Count > 0 || node.PassedFilterField || (node.Parent?.PassedFilterField ?? false))
					yield return node;
			}
		}
	}
}