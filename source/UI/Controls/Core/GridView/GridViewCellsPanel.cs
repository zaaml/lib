// <copyright file="GridViewCellsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

namespace Zaaml.UI.Controls.Core.GridView
{
	public abstract class GridViewCellsPanel : Panel
	{
		private byte _packedValue;

		protected GridViewCellsPanel()
		{
			IsStructureDirty = true;
		}

		protected abstract bool AllowCellSplitter { get; }

		internal long ArrangeLayoutVersion { get; private set; } = -1;

		protected abstract IReadOnlyList<GridViewCell> CellsCore { get; }

		protected GridViewCellsPresenter CellsPresenterCore => (GridViewCellsPresenter)VisualParent;

		protected virtual GridViewElement FillElement => null;

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

		internal GridViewCellsPanel PrevCellsPanel { get; set; }

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var viewController = CellsPresenterCore.ViewControllerInternal;
			var columnController = CellsPresenterCore.ColumnControllerInternal;

			if (columnController == null)
				return finalSize;

			columnController.OnCellsPanelArrange(this, finalSize);

			try
			{
				IsInArrange = true;

				var isInsideScrollView = CellsPresenterCore.ScrollViewControl == null;
				var horizontalOffset = isInsideScrollView ? 0 : viewController.ScrollInfo.Offset;
				var offset = -horizontalOffset.LayoutRoundX(RoundingMode.MidPointFromZero);

				for (var i = 0; i < Children.Count; i++)
				{
					var child = Children[i];

					if (child is not GridViewCell gridCell)
						break;

					var column = gridCell.GridColumnCoreInternal;
					var columnFinalWidth = column?.FinalWidth ?? gridCell.DesiredSize.Width;
					var cellSize = new Size(columnFinalWidth, finalSize.Height);
					var cellRect = new Rect(new Point(offset, 0), cellSize);

					if (gridCell.DesiredSize.Width.IsGreaterThan(cellSize.Width))
						gridCell.Measure(cellSize);

					gridCell.Arrange(cellRect);

					offset += cellSize.Width;
				}

				var fillWidth = finalSize.Width - columnController.ActualColumnsWidth;

				if (this.GetVisualParent() is GridViewCellsPresenter presenter)
					fillWidth = presenter.ArrangeBounds.Width - (columnController.ActualColumnsWidth - horizontalOffset);

				if (fillWidth.IsGreaterThan(0))
					FillElement?.Arrange(new Rect(new Point(offset, 0), new Size(fillWidth, finalSize.Height)));

				return finalSize;
			}
			finally
			{
				IsInArrange = false;
				ArrangeLayoutVersion = viewController.LayoutVersion;
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
						var child = Children[childIndex] as GridViewCellSplitter;

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

		protected virtual GridViewCellSplitter CreateCellSplitter()
		{
			return new GridViewCellSplitter();
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

		private static GridViewColumnWidthConstraints GetColumnWidth(GridViewColumn column)
		{
			return column?.ActualColumnWidthConstraints ?? GridViewColumnWidthConstraints.Default;
		}

		protected override Geometry GetLayoutClip(Size layoutSlotSize)
		{
			return null;
		}

		internal void InvalidatePanelMeasureInternal()
		{
			var presenter = CellsPresenterCore;
			var templatedParent = presenter.GetTemplatedParent();

			this.InvalidateAncestorsMeasure(templatedParent, true);
		}

		internal void InvalidateStructure()
		{
			if (IsStructureDirty)
				return;

			IsStructureDirty = true;

			InvalidateMeasure();
		}

		protected virtual bool IsValidSplitter(GridViewCellSplitter cellSplitter)
		{
			return cellSplitter != null;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			EnsureStructure();

			var viewController = CellsPresenterCore.ViewControllerInternal;
			var columnController = CellsPresenterCore.ColumnControllerInternal;

			if (columnController == null)
				return new Size();

			var fixedResult = new OrientedSize(Orientation.Horizontal);
			var minResultConstraint = 0.0;
			var starLengthValue = 0.0;

			// Auto
			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is not GridViewCell gridCell)
					break;

				var column = gridCell.GridColumnCoreInternal;
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

				if (child is not GridViewCell gridCell)
					break;

				var column = gridCell.GridColumnCoreInternal;
				var columnWidth = GetColumnWidth(column);
				var value = columnWidth.Value;
				var minimum = columnWidth.Minimum;
				var maximum = columnWidth.Maximum;

				GridViewColumnWidthConstraints.CalcConstraints(value, minimum, maximum, gridCell.AutoDesiredSize.Width, double.NaN, out var valueConstraint, out var minimumConstraint, out var maximumConstraint);

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

				if (child is not GridViewCell gridCell)
					break;

				var column = gridCell.GridColumnCoreInternal;
				var columnWidth = GetColumnWidth(column);
				var value = columnWidth.Value;
				var minimum = columnWidth.Minimum;
				var maximum = columnWidth.Maximum;

				if (value.IsStar == false)
					continue;

				GridViewColumnWidthConstraints.CalcConstraints(value, minimum, maximum, gridCell.AutoDesiredSize.Width, starLength, out var valueConstraint, out _, out _);

				var constraint = new Size(valueConstraint, availableSize.Height);

				gridCell.Measure(constraint);

				starResult = starResult.StackSize(gridCell.DesiredSize);
			}

			var result = new Size(columnController.ActualColumnsWidth, Math.Max(fixedResult.Indirect, starResult.Indirect));

			FillElement?.Measure(new Size(0, result.Height));

			viewController.OnPanelMeasured(this);

			return result;
		}

		internal void OnCellStructurePropertyChanged(GridViewColumn gridColumn)
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

	public abstract class GridViewCellsPanel<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : GridViewCellsPanel
		where TGridColumn : GridViewColumn
		where TGridCellsPresenter : GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		protected override bool AllowCellSplitter => CellsPresenter.AllowCellSplitter;

		protected override IReadOnlyList<GridViewCell> CellsCore => CellsPresenter.Cells;

		protected TGridCellsPresenter CellsPresenter => (TGridCellsPresenter)CellsPresenterCore;
		
		protected GridViewColumnCollection<TGridColumn> Columns { get; set; }
	}
}