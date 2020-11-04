// <copyright file="RecursiveFilteringStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Data.Hierarchy
{
	internal sealed class RecursiveFilteringStrategy<THierarchy, TNodeCollection, TNode> : FilteringStrategy<THierarchy, TNodeCollection, TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		public static readonly FilteringStrategy<THierarchy, TNodeCollection, TNode> Instance = new RecursiveFilteringStrategy<THierarchy, TNodeCollection, TNode>();

		private RecursiveFilteringStrategy()
		{
		}

		public override IEnumerable<TNode> Filter(TNodeCollection collection)
		{
			foreach (var node in collection.SourceCollection)
			{
				if (node.PassedFilterField)
					yield return node;
			}
		}
	}
}