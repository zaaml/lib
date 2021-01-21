// <copyright file="ListViewItemCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemCellCollection
		: GridCellCollection<ListViewItemCellsPresenter,
			ListViewItemCellsPanel,
			ListViewItemCellCollection,
			ListViewItemCell,
			ListViewItemCellSplitter,
			ListViewItemCellColumnController,
			ListViewItemCellColumn>
	{
		public ListViewItemCellCollection(ListViewItemCellsPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}