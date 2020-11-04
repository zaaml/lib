// <copyright file="HierarchyDataPlainListView.ICollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.UI.Data.Hierarchy
{
	internal partial class HierarchyDataPlainListView
	{
		#region Interface Implementations

		#region ICollection

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		int ICollection.Count => HierarchyView.VisibleFlatCount;

		object ICollection.SyncRoot => this;

		bool ICollection.IsSynchronized => false;

		#endregion

		#endregion
	}
}