// <copyright file="GridCellPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Packed;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellsPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn> : Panel, IFlexPanel, IFlexPanelEx
		where TGridCellPresenter : GridCellsPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPanel : GridCellsPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellCollection : GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCell : GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellSplitter : GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumnController : GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumn : GridCellColumn<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
	{
		private FlexPanelLayout _layout;
		private byte _packedValue;

		protected GridCellsPanel()
		{
			IsStructureDirty = true;
		}

		internal TGridCellPresenter CellPresenter { get; set; }

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

		internal FlexPanelLayout Layout => _layout ??= CreateLayout();

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			try
			{
				IsInArrange = true;

				return Layout.Arrange(finalSize);
			}
			finally
			{
				IsInArrange = false;
			}
		}

		protected abstract TGridCellSplitter CreateCellSplitter();

		private FlexPanelLayout CreateLayout()
		{
			return new FlexPanelLayout(this);
		}

		private void EnsureStructure()
		{
			if (IsStructureDirty == false)
				return;

			var columnController = CellPresenter.ColumnControllerInternal;

			Children.Clear();

			var allowSplitter = CellPresenter.AllowCellSplitter;
			var gridCells = CellPresenter.Cells;

			if (allowSplitter)
			{
				for (var index = 0; index < gridCells.Count; index++)
				{
					var cell = gridCells[index];

					cell.Column = columnController?.GetColumn(index);

					Children.Add(cell);

					if (index != gridCells.Count - 1)
						Children.Add(CreateCellSplitter());
				}
			}
			else
			{
				for (var index = 0; index < gridCells.Count; index++)
				{
					var cell = gridCells[index];

					cell.Column = columnController?.GetColumn(index);

					Children.Add(cell);
				}
			}

			IsStructureDirty = false;
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

			return Layout.Measure(availableSize);
		}

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			if (IsInArrange)
				return;

			base.OnChildDesiredSizeChanged(child);
		}

		Orientation IOrientedPanel.Orientation => Orientation.Horizontal;

		IFlexDistributor IFlexPanel.Distributor => FlexDistributor.Equalizer;

		bool IFlexPanel.HasHiddenChildren { get; set; }

		double IFlexPanel.Spacing => 0;

		FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

		FlexElement IFlexPanel.GetFlexElement(UIElement child)
		{
			return child switch
			{
				TGridCellSplitter _ => new FlexElement
				{
					StretchDirection = FlexStretchDirection.None,
					Length = new FlexLength(0.0, FlexLengthUnitType.Pixel)
				},
				TGridCell cell => new FlexElement
				{
					StretchDirection = FlexStretchDirection.Both, Length = new FlexLength(CellPresenter.ColumnControllerInternal?.GetFlexWidth(cell.Column.Index) ?? 1.0, FlexLengthUnitType.Star)
				},
				_ => throw new InvalidOperationException("Invalid structure.")
			};
		}

		bool IFlexPanel.GetIsHidden(UIElement child)
		{
			return false;
		}

		void IFlexPanel.SetIsHidden(UIElement child, bool value)
		{
		}

		bool IFlexPanelEx.AllowMeasureInArrange => true;

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
}