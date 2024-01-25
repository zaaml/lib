// <copyright file="GridViewColumnController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

namespace Zaaml.UI.Controls.Core.GridView
{
	public abstract class GridViewColumnController
	{
		private double _finalWidth;


		protected GridViewColumnController(GridViewController viewController)
		{
			ViewController = viewController;
		}

		internal double ActualColumnsWidth { get; private set; }

		private IEnumerable<GridViewCellsPresenter> CellsPresenters => ViewController.CellsPresentersInternal;

		protected virtual int ColumnCount => Columns.Count;

		private List<GridViewColumn> Columns { get; } = new();

		protected virtual GridViewColumnWidthConstraints DefaultColumnWidthConstraints => new(GridViewColumn.DefaultWidth, GridViewColumn.DefaultMinWidth, GridViewColumn.DefaultMaxWidth);

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

		private FlexElementCollection FlexColumns { get; } = new();

		private GridViewCellsPresenter HeaderCellsPresenter => ViewController.HeaderCellsPresenterInternal;

		public GridViewController ViewController { get; }

		private void CalculateFinalColumnWidth()
		{
			var starLengthValue = 0.0;
			var fixedValue = 0.0;
			var minResultConstraint = 0.0;
			var useLayoutRounding = ViewController.Owner.UseLayoutRounding;

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

				GridViewColumnWidthConstraints.CalcConstraints(value, minimum, maximum, column.AutoDesiredWidth, double.NaN, out var valueConstraint, out var minimumConstraint, out _);

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

				GridViewColumnWidthConstraints.CalcConstraints(value, minimum, maximum, column.AutoDesiredWidth, starValue, out var valueConstraint, out var minimumConstraint, out var maximumConstraint);

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

				var columnFlexElement = new FlexElement(minimumConstraint, maximumConstraint) { Length = value, StretchDirection = FlexStretchDirection.Both, ActualLength = finalWidth }.WithRounding(useLayoutRounding);

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

			ViewController.InvalidateMeasuredPanels();
		}

		protected virtual GridViewColumn CreateColumn()
		{
			return new GridViewColumn(this);
		}

		public virtual GridViewColumn GetColumn(int index)
		{
			if (index < 0)
				return null;

			while (Columns.Count <= index)
				Columns.Add(CreateColumn());

			return Columns[index];
		}

		protected virtual GridViewColumnWidthConstraints GetColumnWidthConstraints(GridViewColumn gridColumn)
		{
			var defaultConstraints = DefaultColumnWidthConstraints;
			var width = gridColumn.GetValueSource(GridViewColumn.WidthProperty) == PropertyValueSource.Default ? defaultConstraints.Value : gridColumn.Width;
			var minWidth = gridColumn.GetValueSource(GridViewColumn.MinWidthProperty) == PropertyValueSource.Default ? defaultConstraints.Minimum : gridColumn.MinWidth;
			var maxWidth = gridColumn.GetValueSource(GridViewColumn.MaxWidthProperty) == PropertyValueSource.Default ? defaultConstraints.Maximum : gridColumn.MaxWidth;

			return new GridViewColumnWidthConstraints(width, minWidth, maxWidth);
		}

		internal GridViewColumnWidthConstraints GetColumnWidthConstraintsInternal(GridViewColumn gridColumn)
		{
			return GetColumnWidthConstraints(gridColumn);
		}

		private void InvalidateCellsPresentersArrange()
		{
			ViewController.InvalidateCellsPresentersArrange();
		}

		internal void OnAutoDesiredWidthChanged(GridViewColumn gridColumn)
		{
			CalculateFinalColumnWidth();
		}

		internal void OnCellsPanelArrange(GridViewCellsPanel panel, Size finalSize)
		{
			var layoutVersion = ViewController.LayoutVersion;

			if (panel.ArrangeLayoutVersion < layoutVersion)
			{
				if (FinalWidthLayoutVersion < layoutVersion)
				{
					FinalWidth = finalSize.Width.RoundMidPointFromZero();
					FinalWidthLayoutVersion = layoutVersion;
				}
				else
					FinalWidth = Math.Min(FinalWidth, finalSize.Width).RoundMidPointFromZero();
			}
		}

		internal void OnCellStructurePropertyChanged(GridViewColumn gridColumn)
		{
			HeaderCellsPresenter?.CellsPanelInternal?.OnCellStructurePropertyChanged(gridColumn);

			foreach (var cellsPresenter in CellsPresenters)
				cellsPresenter.CellsPanelInternal?.OnCellStructurePropertyChanged(gridColumn);
		}

		internal void OnColumnWidthConstraintsChanged()
		{
			ViewController.InvalidateCellsPresentersMeasure();
		}
	}
}