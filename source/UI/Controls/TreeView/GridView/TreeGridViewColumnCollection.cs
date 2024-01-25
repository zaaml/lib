// <copyright file="TreeGridViewColumnCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewColumnCollection : GridViewColumnCollection<TreeGridViewColumn>
	{
		internal TreeGridViewColumnCollection(TreeGridView treeGridView)
		{
			TreeGridView = treeGridView;
		}

		public TreeGridView TreeGridView { get; }

		protected override void OnItemAdded(TreeGridViewColumn column)
		{
			base.OnItemAdded(column);

			if (column.View != null)
				throw new InvalidOperationException();

			column.View = TreeGridView;
		}

		protected override void OnItemRemoved(TreeGridViewColumn column)
		{
			if (ReferenceEquals(column.View, TreeGridView) == false)
				throw new InvalidOperationException();

			column.View = null;

			base.OnItemRemoved(column);
		}
	}
}