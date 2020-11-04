// <copyright file="NodesFilteringStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Data.Hierarchy
{
	internal sealed class NodesFilteringStrategy<THierarchy, TNodeCollection, TNode> : FilteringStrategy<THierarchy, TNodeCollection, TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		public static readonly FilteringStrategy<THierarchy, TNodeCollection, TNode> Instance = new NodesFilteringStrategy<THierarchy, TNodeCollection, TNode>();

		private NodesFilteringStrategy()
		{
		}

		public override bool CanHideParent => true;

		public override IEnumerable<TNode> Filter(TNodeCollection collection)
		{
			foreach (var node in collection.SourceCollection)
			{
				if (node.PassedFilterField)
					yield return node;
				else if (node.Nodes != null)
				{
					foreach (var child in node.Nodes.FilteredCollection)
					{
						yield return child;
					}
				}
			}
		}
	}
}