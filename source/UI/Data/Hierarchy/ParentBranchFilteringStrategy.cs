// <copyright file="ParentBranchFilteringStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Data.Hierarchy
{
	internal sealed class ParentBranchFilteringStrategy<THierarchy, TNodeCollection, TNode> : FilteringStrategy<THierarchy, TNodeCollection, TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		private ParentBranchFilteringStrategy()
		{
		}

		public static readonly FilteringStrategy<THierarchy, TNodeCollection, TNode> Instance = new ParentBranchFilteringStrategy<THierarchy, TNodeCollection, TNode>();

		public override IEnumerable<TNode> Filter(TNodeCollection collection)
		{
			foreach (var node in collection.SourceCollection)
			{
				if (node.Nodes.FilteredCollection.Count > 0 || node.PassedFilterField)
					yield return node;
			}
		}
	}
}