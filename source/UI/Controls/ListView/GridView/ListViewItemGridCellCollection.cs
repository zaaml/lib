// <copyright file="ListViewItemGridCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemGridCellCollection
		: GridCellCollection<ListViewItemGridCellsPresenter,
			ListViewItemGridCellsPanel,
			ListViewItemGridCellCollection,
			ListViewItemGridCell>
	{
		public ListViewItemGridCellCollection(ListViewItemGridCellsPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}