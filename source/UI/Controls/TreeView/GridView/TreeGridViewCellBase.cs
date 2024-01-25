// <copyright file="TreeGridViewCellBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeGridViewCellBase<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		: GridViewCell<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPresenter : GridViewCellsPresenter<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		internal TreeGridViewCellBase()
		{
		}

		protected abstract TreeGridView GridView { get; }

		protected abstract GridViewLines GridViewLines { get; }

		protected virtual Thickness GetBorderThickness(GridViewLines gridViewLines)
		{
			var vt = gridViewLines.ShowVertical() ? 1 : 0;
			var ht = gridViewLines.ShowHorizontal() ? 1 : 0;

			return BorderThickness = new Thickness(vt, ht, vt, ht);
		}

		private void InvalidateCellMeasure()
		{
			var treeViewItem = (CellsPresenterInternal as TreeGridViewCellsPresenter)?.TreeViewItem;

			if (treeViewItem != null)
				this.InvalidateAncestorsMeasure(treeViewItem, true);
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