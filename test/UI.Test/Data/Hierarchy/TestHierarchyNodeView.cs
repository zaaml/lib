// <copyright file="TestHierarchyNodeView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;
using Zaaml.UI.Controls.TreeView;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	[DebuggerDisplay("{ToString()}")]
	internal sealed class TestHierarchyNodeView : HierarchyNodeView<TestHierarchyView, TestHierarchyNodeViewCollection, TestHierarchyNodeView>
	{
		public TestHierarchyNodeView(TestHierarchyView tree, TestHierarchyNodeView parent) : base(tree, parent)
		{
		}

		public int FlatIndex => Tree.FindIndex(this);

		public TestHierarchyView Tree => Hierarchy;

		public TreeViewItem TreeViewItem { get; set; }

		protected override TestHierarchyNodeView CreateChildNodeCore(object nodeData)
		{
			var treeNode = new TestHierarchyNodeView(Tree, this)
			{
				Data = nodeData,
			};

			return treeNode;
		}

		protected override TestHierarchyNodeViewCollection CreateNodeCollectionCore()
		{
			return new TestHierarchyNodeViewCollection(Tree, this, CreateChildNode);
		}

		public override string ToString()
		{
			return Data?.ToString() ?? "Empty";
		}
	}
}