// <copyright file="ListViewItemCellColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemCellColumn
		: GridCellColumn<ListViewItemCellsPresenter,
			ListViewItemCellsPanel,
			ListViewItemCellCollection,
			ListViewItemCell,
			ListViewItemCellSplitter,
			ListViewItemCellColumnController,
			ListViewItemCellColumn>
	{
		public ListViewItemCellColumn(ListViewItemCellColumnController controller, int index) : base(controller, index)
		{
		}
	}
}