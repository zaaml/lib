// <copyright file="GridViewCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core.GridView
{
	public abstract class GridViewCellCollection<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : DependencyObjectCollectionBase<TGridCell>, IReadOnlyList<GridViewCell>
		where TGridColumn : GridViewColumn
		where TGridCellsPresenter : GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		protected GridViewCellCollection(TGridCellsPresenter cellsPresenter)
		{
			CellsPresenter = cellsPresenter;
		}

		public TGridCellsPresenter CellsPresenter { get; }

		private void InvalidateStructure()
		{
			CellsPresenter.CellsPanelInternal?.InvalidateStructure();
		}

		protected override void OnItemAdded(TGridCell cell)
		{
			base.OnItemAdded(cell);

			cell.CellsPresenterInternal = CellsPresenter;

			var index = 0;

			foreach (var gridCell in this)
				gridCell.CellIndex = index++;

			InvalidateStructure();
		}

		protected override void OnItemRemoved(TGridCell cell)
		{
			var index = 0;

			foreach (var gridCell in this)
				gridCell.CellIndex = index++;

			cell.CellIndex = -1;
			cell.CellsPresenterInternal = null;

			base.OnItemRemoved(cell);

			InvalidateStructure();
		}

		IEnumerator<GridViewCell> IEnumerable<GridViewCell>.GetEnumerator()
		{
			return GetEnumerator();
		}

		GridViewCell IReadOnlyList<GridViewCell>.this[int index] => this[index];
	}
}