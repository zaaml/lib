// <copyright file="FilteringStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Data.Hierarchy
{
	internal abstract class FilteringStrategy<THierarchy, TNodeCollection, TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		public abstract IEnumerable<TNode> Filter(TNodeCollection collection);

		public virtual bool CanHideParent => false;
	}
}