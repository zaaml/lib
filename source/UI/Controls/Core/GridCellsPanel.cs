// <copyright file="GridCellsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Panels;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellsPanel : Panel
	{
		private double _horizontalOffset;
		private byte _packedValue;

		protected GridCellsPanel()
		{
			IsStructureDirty = true;
		}

		protected abstract bool AllowCellSplitter { get; }

		internal long ArrangeLayoutVersion { get; private set; } = -1;

		protected abstract IReadOnlyList<GridCell> CellsCore { get; }

		internal GridCellsPresenter CellsPresenterInternal => (GridCellsPresenter) VisualParent;

		protected virtual GridElement FillElement => null;

		internal double HorizontalOffset
		{
			get => _horizontalOffset;
			set
			{
				if (_horizontalOffset.IsCloseTo(value))
					return;

				_horizontalOffset = value;

				InvalidateArrange();
			}
		}

		private bool IsInArrange
		{
			get => PackedDefinition.IsInArrange.GetValue(_packedValue);
			set => PackedDefinition.IsInArrange.SetValue(ref _packedValue, value);
		}

		private bool IsStructureDirty
		{
			get => PackedDefinition.IsStructureDirty.GetValue(_packedValue);
			set => PackedDefinition.IsStructureDirty.SetValue(ref _packedValue, value);
		}

		internal long MeasureLayoutVersion { get; set; } = -1;

		internal GridCellsPanel PrevCellsPanel { get; set; }

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var controller = CellsPresenterInternal.ControllerInternal;

			controller.OnCellsPanelArrange(this, finalSize);

			try
			{
				IsInArrange = true;

				var offset = -_horizontalOffset.LayoutRoundX(RoundingMode.MidPointFromZero);

				for (var i = 0; i < Children.Count; i++)
				{
					var child = Children[i];

					if (child is not GridCell gridCell || gridCell.ControllerInternal == null)
						break;

					var column = gridCell.ColumnInternal;
					var columnFinalWidth = column?.FinalWidth ?? gridCell.DesiredSize.Width;
					var cellSize = new Size(columnFinalWidth, finalSize.Height);
					var cellRect = new Rect(new Point(offset, 0), cellSize);

					if (gridCell.DesiredSize.Width.IsGreaterThan(cellSize.Width))
						gridCell.Measure(cellSize);

					gridCell.Arrange(cellRect);

					offset += cellSize.Width;
				}

				var fillWidth = finalSize.Width - controller.ActualColumnsWidth;

				if (this.GetVisualParent() is GridCellsPresenter presenter)
					fillWidth = presenter.ArrangeBounds.Width - (controller.ActualColumnsWidth - _horizontalOffset);

				if (fillWidth.IsGreaterThan(0))
					FillElement?.Arrange(new Rect(new Point(offset, 0), new Size(fillWidth, finalSize.Height)));

				return finalSize;
			}
			finally
			{
				IsInArrange = false;
				ArrangeLayoutVersion = controller.LayoutVersion;
			}
		}

		protected virtual bool CheckStructure()
		{
			var allowSplitter = AllowCellSplitter;
			var cellCount = CellsCore.Count;
			var childIndex = 0;

			for (var index = 0; index < cellCount; index++)
			{
				var cell = CellsCore[index];

				if (childIndex < Children.Count)
				{
					var child = Children[childIndex];

					if (ReferenceEquals(child, cell))
					{
						childIndex++;

						continue;
					}
				}

				return false;
			}

			if (allowSplitter)
			{
				for (var index = 0; index < cellCount - 1; index++)
				{
					if (childIndex < Children.Count)
					{
						var child = Children[childIndex] as GridCellSplitter;

						if (IsValidSplitter(child))
						{
							childIndex++;

							continue;
						}
					}

					return false;
				}
			}

			var fillElement = FillElement;

			if (fillElement != null)
			{
				if (childIndex < Children.Count)
				{
					var child = Children[childIndex];

					if (ReferenceEquals(fillElement, child) == false)
						return false;
				}
				else
					return false;
			}

			if (childIndex != Children.Count)
				return false;

			return true;
		}

		protected virtual GridCellSplitter CreateCellSplitter()
		{
			return new GridCellSplitter();
		}

		private void EnsureStructure()
		{
			if (IsStructureDirty == false)
				return;

			try
			{
				if (CheckStructure())
					return;

				Children.Clear();

				var allowSplitter = AllowCellSplitter;
				var cellCount = CellsCore.Count;

				for (var index = 0; index < cellCount; index++)
				{
					var cell = CellsCore[index];

					cell.UpdateStructureInternal(true);

					Children.Add(cell);
				}

				if (allowSplitter)
				{
					for (var index = 0; index < cellCount - 1; index++)
						Children.Add(CreateCellSplitter());
				}

				var fillElement = FillElement;

				if (fillElement != null)
					Children.Add(fillElement);
			}
			finally
			{
				IsStructureDirty = false;
			}
		}

		private static GridColumnWidthConstraints GetColumnWidth(GridColumn column)
		{
			return column?.ActualColumnWidthConstraints ?? GridColumnWidthConstraints.Default;
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			return null;
		}

		internal void InvalidateStructure()
		{
			if (IsStructureDirty)
				return;

			IsStructureDirty = true;

			InvalidateMeasure();
		}

		internal void InvalidatePanelMeasureInternal()
		{
			var presenter = CellsPresenterInternal;
			var templatedParent = presenter.GetTemplatedParent();

			this.InvalidateAncestorsMeasure(templatedParent, true);
		}

		protected virtual bool IsValidSplitter(GridCellSplitter cellSplitter)
		{
			return cellSplitter != null;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			EnsureStructure();

			var controller = CellsPresenterInternal.ControllerInternal;
			var fixedResult = new OrientedSize(Orientation.Horizontal);
			var minResultConstraint = 0.0;
			var starLengthValue = 0.0;

			// Auto
			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is not GridCell gridCell)
					break;

				var column = gridCell.ColumnInternal;
				var columnWidth = GetColumnWidth(column);
				var value = columnWidth.Value;
				var minimum = columnWidth.Minimum;
				var maximum = columnWidth.Maximum;

				if (!minimum.IsAuto && !value.IsAuto && !maximum.IsAuto)
					continue;

				var constraint = new Size(double.PositiveInfinity, availableSize.Height);

				gridCell.Measure(constraint);

				if (column != null)
				{
					column.AutoDesiredWidth = Math.Max(column.AutoDesiredWidth, gridCell.DesiredSize.Width);
					gridCell.AutoDesiredSize = new Size(column.AutoDesiredWidth, gridCell.DesiredSize.Height);
				}
				else
					gridCell.AutoDesiredSize = gridCell.DesiredSize;
			}

			// Fixed
			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is not GridCell gridCell)
					break;

				var column = gridCell.ColumnInternal;
				var columnWidth = GetColumnWidth(column);
				var value = columnWidth.Value;
				var minimum = columnWidth.Minimum;
				var maximum = columnWidth.Maximum;

				GridColumnWidthConstraints.CalcConstraints(value, minimum, maximum, gridCell.AutoDesiredSize.Width, double.NaN, out var valueConstraint, out var minimumConstraint, out var maximumConstraint);

				var constraint = new Size(valueConstraint, availableSize.Height);

				if (value.IsStar)
				{
					starLengthValue += columnWidth.Value.Value;
					minResultConstraint += minimumConstraint;

					continue;
				}

				gridCell.Measure(constraint);

				var cellSize = gridCell.DesiredSize;

				cellSize.Width = value.IsAbsolute ? valueConstraint : cellSize.Width.Clamp(minimumConstraint, maximumConstraint);

				fixedResult = fixedResult.StackSize(cellSize);
			}

			var starAvailable = Math.Max(0, availableSize.Width - fixedResult.Width - minResultConstraint);
			var starLength = starAvailable / starLengthValue;
			var starResult = new OrientedSize(Orientation.Horizontal);

			// Star
			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is not GridCell gridCell)
					break;

				var column = gridCell.ColumnInternal;
				var columnWidth = GetColumnWidth(column);
				var value = columnWidth.Value;
				var minimum = columnWidth.Minimum;
				var maximum = columnWidth.Maximum;

				if (value.IsStar == false)
					continue;

				GridColumnWidthConstraints.CalcConstraints(value, minimum, maximum, gridCell.AutoDesiredSize.Width, starLength, out var valueConstraint, out _, out _);

				var constraint = new Size(valueConstraint, availableSize.Height);

				gridCell.Measure(constraint);

				starResult = starResult.StackSize(gridCell.DesiredSize);
			}

			var result = new Size(controller.ActualColumnsWidth, Math.Max(fixedResult.Indirect, starResult.Indirect));

			FillElement?.Measure(new Size(0, result.Height));

			controller.OnPanelMeasured(this);

			return result;
		}

		internal void OnCellStructurePropertyChanged(GridColumn gridColumn)
		{
			InvalidateStructure();
		}

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			if (IsInArrange)
				return;

			base.OnChildDesiredSizeChanged(child);
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsInArrange;
			public static readonly PackedBoolItemDefinition IsStructureDirty;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsInArrange = allocator.AllocateBoolItem();
				IsStructureDirty = allocator.AllocateBoolItem();
			}
		}
	}

	public abstract class GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : GridCellsPanel
		where TGridCellsPresenter : GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		protected override bool AllowCellSplitter => CellPresenter.AllowCellSplitter;

		private TGridCellsPresenter CellPresenter => (TGridCellsPresenter) CellsPresenterInternal;

		protected override IReadOnlyList<GridCell> CellsCore => CellPresenter.Cells;
	}
}