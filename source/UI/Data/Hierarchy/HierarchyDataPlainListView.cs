// <copyright file="HierarchyDataPlainListView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyDataPlainListView
	{
		#region Ctors

		public HierarchyDataPlainListView(HierarchyView hierarchyView)
		{
			HierarchyView = hierarchyView;
		}

		#endregion

		#region Properties

		public HierarchyView HierarchyView { get; }

		#endregion
	}
}