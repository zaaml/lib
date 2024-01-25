// <copyright file="TreeGridViewColumnController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewColumnController : GridViewColumnController
	{
		public TreeGridViewColumnController(TreeGridViewController viewController) : base(viewController)
		{
		}

		protected override int ColumnCount => TreeGridView?.Columns.Count ?? 0;

		protected override GridViewColumnWidthConstraints DefaultColumnWidthConstraints => TreeGridView?.DefaultColumnWidthConstraints ?? base.DefaultColumnWidthConstraints;

		private TreeGridView TreeGridView => TreeViewController.TreeViewControl?.View as TreeGridView;

		public TreeGridViewController TreeViewController => (TreeGridViewController)ViewController;

		public override GridViewColumn GetColumn(int index)
		{
			var columns = TreeGridView?.Columns;

			if (columns == null)
				return null;

			return index >= 0 && index < columns.Count && columns.Count > 0 ? columns[index] : null;
		}
	}
}