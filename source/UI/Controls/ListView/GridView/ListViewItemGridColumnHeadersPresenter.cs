// <copyright file="ListViewItemGridColumnHeadersPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemGridColumnHeadersPresenter
		: ListViewItemGridElementPresenter<ListViewItemGridColumnHeadersPresenter,
			ListViewItemGridColumnHeadersPanel,
			ListViewItemGridColumnHeaderCollection,
			ListViewItemGridColumnHeader>
	{
		public ListViewItemGridColumnHeadersPresenter()
		{
			AllowCellSplitter = true;
		}

		protected override ListViewItemGridColumnHeaderCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override void CreateCells(ListGridViewColumnCollection columns)
		{
			for (var index = 0; index < columns.Count; index++)
				Cells.Add(new ListViewItemGridColumnHeader());
		}

		protected override void DestroyCells()
		{
			Cells.Clear();
		}
	}
}