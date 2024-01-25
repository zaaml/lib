// <copyright file="ListGridViewColumnController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewColumnController : GridViewColumnController
	{
		public ListGridViewColumnController(ListGridViewController viewController) : base(viewController)
		{
		}

		protected override int ColumnCount => ListGridView?.Columns.Count ?? 0;

		protected override GridViewColumnWidthConstraints DefaultColumnWidthConstraints => ListGridView?.DefaultColumnWidthConstraints ?? base.DefaultColumnWidthConstraints;

		private ListGridView ListGridView => ListViewController.ListViewControl?.View as ListGridView;

		public ListGridViewController ListViewController => (ListGridViewController)ViewController;

		public override GridViewColumn GetColumn(int index)
		{
			var columns = ListGridView?.Columns;

			if (columns == null)
				return null;

			return index >= 0 && index < columns.Count && columns.Count > 0 ? columns[index] : null;
		}
	}
}