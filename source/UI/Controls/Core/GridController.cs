// <copyright file="GridController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridController
	{
		private double _finalWidth;

		protected GridController(FrameworkElement owner)
		{
			Owner = owner;
			Owner.LayoutUpdated += OnLayoutUpdated;
		}

		protected abstract IEnumerable<GridCellsPresenter> CellsPresenters { get; }

		protected virtual int ColumnCount => Columns.Count;

		private List<GridColumn> Columns { get; } = new();

		internal double FinalWidth
		{
			get => _finalWidth;
			private set
			{
				if (_finalWidth.IsCloseTo(value))
					return;

				_finalWidth = value;

				CalculateFinalColumnWidth();
				InvalidateCellsPresentersArrange();
			}
		}

		private long FinalWidthLayoutVersion { get; set; } = -1;

		internal long LayoutVersion { get; private set; }

		private FrameworkElement Owner { get; }

		private void CalculateFinalColumnWidth()
		{
			var starLengthValue = 0.0;
			var fixedValue = 0.0;

			for (var i = 0; i < ColumnCount; i++)
			{
				var column = GetColumn(i);

				if (column == null)
					break;

				var columnWidth = column.Width;

				if (columnWidth.IsStar)
					starLengthValue += columnWidth.Value;
				else if (columnWidth.IsAuto)
					fixedValue += column.AutoDesiredWidth;
				else
					fixedValue += columnWidth.Value;
			}

			var starValue = Math.Max(0, FinalWidth - fixedValue) / starLengthValue;
			var columnsWidth = 0.0;
			var distributeDeltaCount = 0;

			for (var i = 0; i < ColumnCount; i++)
			{
				var column = GetColumn(i);

				if (column == null)
					break;

				double finalWidth;

				if (column.Width.IsAbsolute)
					finalWidth = column.Width.Value;
				else if (column.Width.IsAuto)
				{
					finalWidth = column.AutoDesiredWidth;
					distributeDeltaCount++;
				}
				else
				{
					finalWidth = column.Width.Value * starValue;
					distributeDeltaCount++;
				}

				finalWidth = finalWidth.RoundToZero();

				column.FinalWidth = finalWidth;
				columnsWidth += finalWidth;
			}

			var deltaWidth = FinalWidth - columnsWidth;

			if (deltaWidth > 0 && distributeDeltaCount > 0)
			{
				var distributeValue = (deltaWidth / distributeDeltaCount).RoundToZero();
				var distributeWidth = 0.0;

				GridColumn lastColumn = null;

				for (var i = 0; i < ColumnCount; i++)
				{
					var column = GetColumn(i);

					if (column == null)
						break;

					if (column.Width.IsAbsolute)
						continue;

					column.FinalWidth += distributeValue;
					distributeWidth += distributeValue;

					lastColumn = column;
				}

				deltaWidth = (deltaWidth - distributeWidth).RoundToZero();

				if (lastColumn != null && deltaWidth.IsGreaterThan(0))
					lastColumn.FinalWidth += deltaWidth;
			}
		}

		protected virtual GridColumn CreateColumn()
		{
			return new GridColumn(this);
		}

		public virtual GridColumn GetColumn(int index)
		{
			while (Columns.Count <= index)
			{
				var column = CreateColumn();

				column.Width = new FlexLength(1.0, FlexLengthUnitType.Star);

				Columns.Add(column);
			}

			return Columns[index];
		}

		public GridColumn GetColumn(GridCell gridCell)
		{
			var index = gridCell.Index;

			return index == -1 ? null : GetColumn(index);
		}

		public FlexLength GetFlexWidth(int index)
		{
			return GetColumn(index)?.Width ?? FlexLength.Auto;
		}

		private void InvalidateCellsPresentersArrange()
		{
			foreach (var cellsPresenter in CellsPresenters)
				cellsPresenter.CellsPanelInternal?.InvalidateArrange();
		}

		private void InvalidateCellsPresentersMeasure()
		{
			foreach (var cellsPresenter in CellsPresenters)
				cellsPresenter.CellsPanelInternal?.InvalidateMeasure();
		}

		internal void OnCellsPanelArrange(GridCellsPanel panel, Size finalSize)
		{
			if (panel.ArrangeLayoutVersion < LayoutVersion)
			{
				if (FinalWidthLayoutVersion < LayoutVersion)
				{
					FinalWidth = finalSize.Width.RoundMidPointFromZero();
					FinalWidthLayoutVersion = LayoutVersion;
				}
				else
					FinalWidth = Math.Min(FinalWidth, finalSize.Width).RoundMidPointFromZero();
			}
		}

		internal void OnCellStructurePropertyChanged(GridColumn gridColumn)
		{
			foreach (var cellsPresenter in CellsPresenters)
				cellsPresenter.CellsPanelInternal?.OnCellStructurePropertyChanged(gridColumn);
		}

		internal void OnColumnWidthChanged(GridColumn gridColumn)
		{
			InvalidateCellsPresentersMeasure();
		}

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			LayoutVersion++;
		}
	}
}