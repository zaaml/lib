// <copyright file="HierarchyFlatListView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Trees;

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyFlatListView : TreeFlatListView<object>
	{
		public HierarchyFlatListView(HierarchyView hierarchyView)
		{
			HierarchyView = hierarchyView;
		}

		protected override int Count => HierarchyView.VisibleFlatCount;

		public HierarchyView HierarchyView { get; }

		protected override IEnumerator<object> GetEnumerator()
		{
			return HierarchyView.GetDataEnumerator();
		}

		protected override object GetItem(int index)
		{
			return HierarchyView.GetData(index);
		}

		protected override int IndexOf(object value)
		{
			return HierarchyView.FindDataIndex(value);
		}
	}
}