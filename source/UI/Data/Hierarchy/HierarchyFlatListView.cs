// <copyright file="HierarchyFlatListView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Trees;

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyFlatListView<TNode> : TreeFlatListView<TNode> where TNode : class
	{
		public HierarchyFlatListView(HierarchyView<TNode> hierarchyView)
		{
			HierarchyView = hierarchyView;
		}

		protected override TreeFlatCursor<TNode> CursorCore => HierarchyView.CursorCore;

		private HierarchyView<TNode> HierarchyView { get; }
	}
}