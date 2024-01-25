// <copyright file="ListGridViewCellBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public abstract class ListGridViewCellBase<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		: GridViewCell<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPresenter : GridViewCellsPresenter<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		internal ListGridViewCellBase()
		{
		}

		protected abstract ListGridView GridView { get; }

		protected abstract GridViewLines GridViewLines { get; }

		protected virtual Thickness GetBorderThickness(GridViewLines gridViewLines)
		{
			var vt = gridViewLines.ShowVertical() ? 1 : 0;
			var ht = gridViewLines.ShowHorizontal() ? 1 : 0;

			return BorderThickness = new Thickness(vt, ht, vt, ht);
		}

		private void InvalidateCellMeasure()
		{
			var listViewItem = (CellsPresenterInternal as ListGridViewCellsPresenter)?.ListViewItem;

			if (listViewItem != null)
				this.InvalidateAncestorsMeasure(listViewItem, true);
			else
				InvalidateMeasure();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			UpdateBorders();

			return base.MeasureOverride(availableSize);
		}

		private void UpdateBorders()
		{
			var lines = GridViewLines;
			var currentBorderThickness = BorderThickness;
			var expectedBorderThickness = GetBorderThickness(lines);

			if (currentBorderThickness != expectedBorderThickness)
				BorderThickness = expectedBorderThickness;

			Margin = new Thickness(lines.ShowVertical() ? -1 : 0, 0, 0, 0);
		}

		protected override void UpdateStructure()
		{
			InvalidateCellMeasure();
		}
	}
}