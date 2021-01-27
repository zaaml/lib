// <copyright file="GridCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : DependencyObjectCollectionBase<TGridCell>, IReadOnlyList<GridCell>
		where TGridCellsPresenter : GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		protected GridCellCollection(TGridCellsPresenter cellsPresenter)
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

			UpdateIndices();
			InvalidateStructure();
		}

		protected override void OnItemRemoved(TGridCell cell)
		{
			cell.Index = -1;
			cell.CellsPresenterInternal = null;

			base.OnItemRemoved(cell);

			UpdateIndices();
			InvalidateStructure();
		}

		private void UpdateIndices()
		{
			for (var i = 0; i < Count; i++)
				this[i].Index = i;
		}

		IEnumerator<GridCell> IEnumerable<GridCell>.GetEnumerator()
		{
			return GetEnumerator();
		}

		GridCell IReadOnlyList<GridCell>.this[int index] => this[index];
	}
}