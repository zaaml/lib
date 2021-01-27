// <copyright file="TreeGridViewColumnCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewColumnCollection : InheritanceContextDependencyObjectCollection<TreeGridViewColumn>
	{
		internal TreeGridViewColumnCollection(TreeGridView treeGridView)
		{
			TreeGridView = treeGridView;
		}

		public TreeGridView TreeGridView { get; }

		protected override void OnItemAdded(TreeGridViewColumn column)
		{
			base.OnItemAdded(column);

			if (column.TreeGridView != null)
				throw new InvalidOperationException();

			column.TreeGridView = TreeGridView;
		}

		protected override void OnItemRemoved(TreeGridViewColumn column)
		{
			if (ReferenceEquals(column.TreeGridView, TreeGridView) == false)
				throw new InvalidOperationException();

			column.TreeGridView = null;

			base.OnItemRemoved(column);
		}
	}
}