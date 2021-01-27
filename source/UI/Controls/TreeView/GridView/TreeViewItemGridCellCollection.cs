// <copyright file="TreeViewItemGridCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemGridCellCollection
		: GridCellCollection<TreeViewItemGridCellsPresenter,
			TreeViewItemGridCellsPanel,
			TreeViewItemGridCellCollection,
			TreeViewItemGridCell>
	{
		public TreeViewItemGridCellCollection(TreeViewItemGridCellsPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}