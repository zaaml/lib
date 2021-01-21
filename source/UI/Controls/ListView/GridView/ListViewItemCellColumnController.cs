// <copyright file="ListViewItemCellColumnController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemCellColumnController
		: GridCellColumnController<ListViewItemCellsPresenter,
			ListViewItemCellsPanel,
			ListViewItemCellCollection,
			ListViewItemCell,
			ListViewItemCellSplitter,
			ListViewItemCellColumnController,
			ListViewItemCellColumn>
	{
		public ListViewItemCellColumnController(ListViewControl listViewControl)
		{
			ListViewControl = listViewControl;
		}

		public ListViewControl ListViewControl { get; }

		protected override ListViewItemCellColumn CreateColumn(int index)
		{
			return new ListViewItemCellColumn(this, index);
		}
	}
}