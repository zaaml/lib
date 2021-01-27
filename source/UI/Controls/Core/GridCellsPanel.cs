// <copyright file="GridCellsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellsPanel : Panel
	{
		private byte _packedValue;

		protected GridCellsPanel()
		{
			IsStructureDirty = true;
		}

		protected abstract bool AllowCellSplitter { get; }

		internal long ArrangeLayoutVersion { get; private set; } = -1;

		protected abstract IReadOnlyList<GridCell> CellsCore { get; }

		internal GridCellsPresenter CellsPresenterInternal => (GridCellsPresenter) VisualParent;

		protected virtual GridHeaderElement FillElement => null;

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

		internal long MeasureLayoutVersion { get; private set; } = -1;

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var controller = CellsPresenterInternal.ControllerInternal;

			controller.OnCellsPanelArrange(this, finalSize);

			try
			{
				IsInArrange = true;

				var offset = 0.0;

				for (var i = 0; i < Children.Count; i++)
				{
					var child = Children[i];

					if (child is not GridCell gridCell || gridCell.ControllerInternal == null)
						break;

					var column = gridCell.ColumnInternal;
					var cellSize = new Size(column.FinalWidth, finalSize.Height);
					var cellRect = new Rect(new Point(offset, 0), cellSize);

					gridCell.Arrange(cellRect);

					offset += cellSize.Width;
				}

				var fillWidth = finalSize.Width - controller.FinalWidth;

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

		protected virtual GridCellSplitter CreateCellSplitter()
		{
			return new GridCellSplitter();
		}

		protected virtual bool IsValidSplitter(GridCellSplitter cellSplitter)
		{
			return cellSplitter != null;
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

		internal void InvalidateStructure()
		{
			if (IsStructureDirty)
				return;

			IsStructureDirty = true;

			InvalidateMeasure();
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			EnsureStructure();

			var controller = CellsPresenterInternal.ControllerInternal;
			var fixedResult = new OrientedSize(Orientation.Horizontal);
			var starLengthValue = 0.0;

			// Fixed
			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is not GridCell gridCell || gridCell.ControllerInternal == null)
					break;

				var column = gridCell.ColumnInternal;

				if (column.Width.IsStar)
				{
					starLengthValue += column.Width.Value;

					continue;
				}

				var constraint = new Size(column.Width.IsAuto ? double.PositiveInfinity : column.Width.Value, availableSize.Height);

				gridCell.Measure(constraint);

				if (column.Width.IsAuto)
					column.AutoDesiredWidth = Math.Max(column.AutoDesiredWidth, gridCell.DesiredSize.Width);

				fixedResult = fixedResult.StackSize(gridCell.DesiredSize);
			}

			var starAvailable = Math.Max(0, availableSize.Width - fixedResult.Width);
			var starLength = starAvailable / starLengthValue;
			var starResult = new OrientedSize(Orientation.Horizontal);

			// Star
			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is not GridCell gridCell || gridCell.ControllerInternal == null)
					break;

				var column = gridCell.ColumnInternal;

				if (column.Width.IsStar == false)
					continue;

				var constraint = new Size(column.Width.Value * starLength, availableSize.Height);

				gridCell.Measure(constraint);

				starResult = starResult.StackSize(gridCell.DesiredSize);
			}

			var result = new Size(fixedResult.Direct, Math.Max(fixedResult.Indirect, starResult.Indirect));

			FillElement?.Measure(new Size(0, result.Height));

			MeasureLayoutVersion = controller.LayoutVersion;

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