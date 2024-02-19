// <copyright file="TestHierarchyNodeViewCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Data.Hierarchy;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	internal sealed class TestHierarchyNodeViewCollection
		: HierarchyNodeViewCollection<TestHierarchyView, TestHierarchyNodeViewCollection, TestHierarchyNodeView>
	{
		public TestHierarchyNodeViewCollection(TestHierarchyView hierarchy, TestHierarchyNodeView parentNode, Func<object, TestHierarchyNodeView> nodeFactory) : base(hierarchy, parentNode, nodeFactory)
		{
		}
	}
}