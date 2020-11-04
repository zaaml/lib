// <copyright file="GridCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn> : DependencyObjectCollectionBase<TGridCell>
		where TGridCellPresenter : GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPanel : GridCellPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellCollection : GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCell : GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellSplitter : GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumnController : GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumn : GridCellColumn<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
	{
		protected GridCellCollection(TGridCellPresenter cellPresenter)
		{
			CellPresenter = cellPresenter;
		}

		public TGridCellPresenter CellPresenter { get; }
	}
}