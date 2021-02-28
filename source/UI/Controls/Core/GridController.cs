// <copyright file="GridController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
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

		internal double ActualColumnsWidth { get; private set; }

		protected abstract IEnumerable<GridCellsPresenter> CellsPresenters { get; }

		protected virtual int ColumnCount => Columns.Count;

		private List<GridColumn> Columns { get; } = new();

		protected virtual GridColumnWidthConstraints DefaultColumnWidthConstraints => new GridColumnWidthConstraints(GridColumn.DefaultWidth, GridColumn.DefaultMinWidth, GridColumn.DefaultMaxWidth);

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

		private FlexElementCollection FlexColumns { get; } = new FlexElementCollection();

		private GridCellsPanel LastMeasurePanel { get; set; }

		internal long LayoutVersion { get; private set; }

		private FrameworkElement Owner { get; }

		private void CalculateFinalColumnWidth()
		{
			var starLengthValue = 0.0;
			var fixedValue = 0.0;
			var minResultConstraint = 0.0;
			var useLayoutRounding = Owner.UseLayoutRounding;

			FlexColumns.EnsureCount(ColumnCount);

			for (var i = 0; i < ColumnCount; i++)
			{
				var column = GetColumn(i);

				if (column == null)
					break;

				var columnWidth = column.ActualColumnWidthConstraints;
				var value = columnWidth.Value;
				var minimum = columnWidth.Minimum;
				var maximum = columnWidth.Maximum;

				GridColumnWidthConstraints.CalcConstraints(value, minimum, maximum, column.AutoDesiredWidth, double.NaN, out var valueConstraint, out var minimumConstraint, out _);

				minResultConstraint += minimumConstraint;

				if (value.IsStar)
					starLengthValue += value.Value;
				else
					fixedValue += valueConstraint;
			}

			var starValue = Math.Max(0, FinalWidth - fixedValue - minResultConstraint) / starLengthValue;
			var columnsWidth = 0.0;
			var distributeDeltaCount = 0;

			for (var i = 0; i < ColumnCount; i++)
			{
				var column = GetColumn(i);

				if (column == null)
					break;

				double finalWidth;

				var columnWidth = column.ActualColumnWidthConstraints;
				var value = columnWidth.Value;
				var minimum = columnWidth.Minimum;
				var maximum = columnWidth.Maximum;

				GridColumnWidthConstraints.CalcConstraints(value, minimum, maximum, column.AutoDesiredWidth, starValue, out var valueConstraint, out var minimumConstraint, out var maximumConstraint);

				if (value.IsAbsolute)
					finalWidth = valueConstraint;
				else if (value.IsAuto)
				{
					finalWidth = valueConstraint;
					distributeDeltaCount++;
				}
				else
				{
					finalWidth = valueConstraint;
					distributeDeltaCount++;
				}

				if (useLayoutRounding)
					finalWidth = finalWidth.LayoutRoundX(RoundingMode.MidPointFromZero);

				column.FinalWidth = finalWidth;
				columnsWidth += finalWidth;

				var columnFlexElement = new FlexElement(minimumConstraint, maximumConstraint) {Length = value, StretchDirection = FlexStretchDirection.Both, ActualLength = finalWidth}.WithRounding(useLayoutRounding);

				FlexColumns[i] = columnFlexElement;
			}

			FlexColumns.UseLayoutRounding = useLayoutRounding;

			var deltaWidth = FinalWidth - columnsWidth;

			if (deltaWidth > 0 && distributeDeltaCount > 0)
			{
				FlexDistributor.Equalizer.Distribute(FlexColumns, FinalWidth);

				for (var i = 0; i < ColumnCount; i++)
				{
					var column = GetColumn(i);

					if (column == null)
						break;

					column.FinalWidth = FlexColumns[i].ActualLength;
				}
			}

			ActualColumnsWidth = 0;

			for (var i = 0; i < ColumnCount; i++)
			{
				var column = GetColumn(i);

				if (column == null)
					break;

				ActualColumnsWidth += column.FinalWidth;
			}

			InvalidateMeasuredPanels();
		}

		protected virtual GridColumn CreateColumn()
		{
			return new GridColumn(this);
		}

		public virtual GridColumn GetColumn(int index)
		{
			while (Columns.Count <= index)
				Columns.Add(CreateColumn());

			return Columns[index];
		}

		public GridColumn GetColumn(GridCell gridCell)
		{
			var index = gridCell.Index;

			return index == -1 ? null : GetColumn(index);
		}

		protected virtual GridColumnWidthConstraints GetColumnWidthConstraints(GridColumn gridColumn)
		{
			var defaultConstraints = DefaultColumnWidthConstraints;
			var width = gridColumn.GetValueSource(GridColumn.WidthProperty) == PropertyValueSource.Default ? defaultConstraints.Value : gridColumn.Width;
			var minWidth = gridColumn.GetValueSource(GridColumn.MinWidthProperty) == PropertyValueSource.Default ? defaultConstraints.Minimum : gridColumn.MinWidth;
			var maxWidth = gridColumn.GetValueSource(GridColumn.MaxWidthProperty) == PropertyValueSource.Default ? defaultConstraints.Maximum : gridColumn.MaxWidth;

			return new GridColumnWidthConstraints(width, minWidth, maxWidth);
		}

		internal GridColumnWidthConstraints GetColumnWidthConstraintsInternal(GridColumn gridColumn)
		{
			return GetColumnWidthConstraints(gridColumn);
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

		private void InvalidateMeasuredPanels()
		{
			var panel = LastMeasurePanel;

			while (panel != null)
			{
				panel.InvalidatePanelMeasureInternal();
				
				panel = panel.PrevCellsPanel;
			}
		}

		internal void OnAutoDesiredWidthChanged(GridColumn gridColumn)
		{
			CalculateFinalColumnWidth();
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

		internal void OnColumnWidthConstraintsChanged()
		{
			InvalidateCellsPresentersMeasure();
		}

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			LayoutVersion++;

			var panel = LastMeasurePanel;

			while (panel != null)
			{
				var nextPanel = panel.PrevCellsPanel;

				panel.PrevCellsPanel = null;

				panel = nextPanel;
			}

			LastMeasurePanel = null;
		}

		internal void OnPanelMeasured(GridCellsPanel gridCellsPanel)
		{
			if (gridCellsPanel.MeasureLayoutVersion == LayoutVersion)
				return;

			gridCellsPanel.PrevCellsPanel = LastMeasurePanel;
			gridCellsPanel.MeasureLayoutVersion = LayoutVersion;

			LastMeasurePanel = gridCellsPanel;
		}
	}
}