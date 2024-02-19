// <copyright file="TestHierarchyView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	internal sealed class TestHierarchyView : HierarchyView<TestHierarchyView, TestHierarchyNodeViewCollection, TestHierarchyNodeView>
	{
		public TestHierarchyView(ITreeAdvisor treeAdvisor)
		{
			TreeAdvisor = treeAdvisor;
		}

		protected override IHierarchyViewFilter<TestHierarchyView, TestHierarchyNodeViewCollection, TestHierarchyNodeView> FilterCore => default;

		protected override FilteringStrategy<TestHierarchyView, TestHierarchyNodeViewCollection, TestHierarchyNodeView> FilteringStrategy => throw new NotSupportedException();

		internal override bool ShouldExpand => IsFilteredInternal;

		public ITreeAdvisor TreeAdvisor { get; }

		protected override TestHierarchyNodeView CreateChildCore(object data)
		{
			return new TestHierarchyNodeView(this, null)
			{
				Data = data
			};
		}

		protected override TestHierarchyNodeViewCollection CreateNodeCollectionCore()
		{
			return new TestHierarchyNodeViewCollection(this, null, CreateChild);
		}

		protected override IEnumerable GetDataNodesCore(TestHierarchyNodeView node)
		{
			return TreeAdvisor.GetNodes(node.Data);
		}

		protected override bool IsDataExpandedCore(TestHierarchyNodeView node)
		{
			return TreeAdvisor.IsExpanded(node.Data);
		}

		public void RefreshFilter()
		{
			RefreshFilterCore();
		}
	}
}