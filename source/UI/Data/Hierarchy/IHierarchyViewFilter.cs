// <copyright file="IHierarchyViewFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Data.Hierarchy
{
	internal interface IHierarchyViewFilter<THierarchy, TNodeCollection, in TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		bool IsEnabled { get; }

		bool Pass(TNode node);
	}
}