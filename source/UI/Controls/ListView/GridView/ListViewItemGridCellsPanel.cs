// <copyright file="ListViewItemGridCellsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemGridCellsPanel
		: GridCellsPanel<ListViewItemGridCellsPresenter,
			ListViewItemGridCellsPanel,
			ListViewItemGridCellCollection,
			ListViewItemGridCell>
	{
		protected override GridElement FillElement { get; } = new ListViewItemGridCellElement();
	}
}