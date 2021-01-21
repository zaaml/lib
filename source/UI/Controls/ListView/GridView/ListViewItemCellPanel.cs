// <copyright file="ListViewItemCellPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemCellsPanel
		: GridCellsPanel<ListViewItemCellsPresenter,
			ListViewItemCellsPanel,
			ListViewItemCellCollection,
			ListViewItemCell,
			ListViewItemCellSplitter,
			ListViewItemCellColumnController,
			ListViewItemCellColumn>
	{
		protected override ListViewItemCellSplitter CreateCellSplitter()
		{
			return new ListViewItemCellSplitter();
		}
	}
}