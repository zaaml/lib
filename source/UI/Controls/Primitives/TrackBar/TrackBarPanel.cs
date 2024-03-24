// <copyright file="TrackBarPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Packed;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
	public sealed class TrackBarPanel : Panel, IStackPanel
	{
		private double _fixedSize;
		private ItemMeasure[] _measure = [];
		private byte _packedValue;
		private TrackBarControl _trackBar;
		private double _variableSize;

		private bool IsInArrange
		{
			get => PackedDefinition.IsInArrange.GetValue(_packedValue);
			set => PackedDefinition.IsInArrange.SetValue(ref _packedValue, value);
		}

		private Orientation Orientation => TrackBar?.Orientation ?? Orientation.Horizontal;

		internal double PixelRatio { get; set; }

		internal TrackBarControl TrackBar
		{
			get => _trackBar;
			set
			{
				_trackBar = value;

				InvalidateMeasure();
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			try
			{
				IsInArrange = true;

				var trackBar = TrackBar;

				if (trackBar == null)
					return finalSize;

				var orientation = Orientation;
				var offset = new OrientedPoint(orientation);
				var finalOriented = finalSize.AsOriented(orientation);
				var prevSize = 0.0;
				var items = trackBar.ItemCollection;
				var itemsCount = items.Count;
				bool? prevIsRange = null;

				ArrayUtils.EnsureArrayLength(ref _measure, itemsCount, true);

				var variableSize = finalOriented.Direct - _fixedSize;

				if (variableSize.IsCloseTo(_variableSize) == false)
					MeasureVariable(variableSize);

				for (var index = 0; index < itemsCount; index++)
				{
					var item = items[index];

					var size = item.DesiredSize.AsOriented(orientation);
					var itemMeasure = _measure[index];
					var isItemRange = item is TrackBarRangeItem;

					if (prevIsRange != isItemRange)
						offset.Direct += prevSize;

					size.Indirect = finalOriented.Indirect;
					size.Direct = itemMeasure.Size;
					offset.Direct += itemMeasure.Offset;

					if (index == itemsCount - 1 && item is TrackBarRangeItem)
						size.Direct = Math.Max(finalOriented.Direct - offset.Direct, Math.Min(size.Direct, 0));

					item.Arrange(new Rect(offset.Point, size.Size));
					prevSize = size.Direct;
					prevIsRange = isItemRange;
				}

				return finalSize;
			}
			finally
			{
				IsInArrange = false;
			}
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			_variableSize = 0.0;
			_fixedSize = 0.0;

			PixelRatio = 0.0;

			var trackBar = TrackBar;

			if (trackBar == null)
				return XamlConstants.ZeroSize;

			var orientation = trackBar.Orientation;
			var availableOriented = availableSize.AsOriented(orientation);
			var itemConstraint = availableOriented.Clone.ChangeDirect(double.PositiveInfinity).Size;
			var fixedOriented = new OrientedSize(orientation);
			var currentRangeOriented = new OrientedSize(orientation);
			var items = trackBar.ItemCollection;
			var itemsCount = items.Count;

			ArrayUtils.EnsureArrayLength(ref _measure, itemsCount);

			for (var index = 0; index < itemsCount; index++)
			{
				var item = items[index];
				var valueItem = item as TrackBarValueItem;
				var rangeItem = item as TrackBarRangeItem;

				item.Measure(itemConstraint);

				var itemOriented = item.DesiredSize.AsOriented(orientation);

				_measure[index] = new ItemMeasure
				{
					Size = itemOriented.Direct,
					Offset = 0
				};

				if (valueItem != null)
				{
					fixedOriented = fixedOriented.StackSize(itemOriented);
					fixedOriented = fixedOriented.StackSize(currentRangeOriented);
					currentRangeOriented = new OrientedSize(orientation);
				}

				if (rangeItem != null)
				{
					currentRangeOriented.Direct = Math.Max(currentRangeOriented.Direct, itemOriented.Direct);
					currentRangeOriented.Indirect = Math.Max(currentRangeOriented.Indirect, itemOriented.Indirect);
				}
			}

			_fixedSize = fixedOriented.Direct;

			MeasureVariable(availableOriented.Direct - fixedOriented.Direct);

			return fixedOriented.Size;
		}

		private double MeasureRange(int startIndex, int endIndex, Orientation orientation)
		{
			var items = TrackBar.ItemCollection;

			if (startIndex == -1 && endIndex == 0)
				return PixelRatio * (((TrackBarValueItem)items[endIndex]).Value - TrackBar.Minimum);

			if (endIndex == -1 && startIndex == items.Count - 1)
				return PixelRatio * (TrackBar.Maximum - ((TrackBarValueItem)items[startIndex]).Value);

			var actualStart = startIndex < 0 ? 0 : startIndex + 1;
			var actualEnd = endIndex < 0 ? items.Count : endIndex;
			var contentCount = actualEnd - actualStart;

			var minimum = startIndex == -1 ? TrackBar.Minimum : ((TrackBarValueItem)items[startIndex]).Value;
			var maximum = endIndex == -1 ? TrackBar.Maximum : ((TrackBarValueItem)items[endIndex]).Value;
			var range = Math.Max(0, maximum - minimum);

			var length = PixelRatio * range;

			if (contentCount <= 0)
				return length;

			var rangeOriented = new OrientedSize(orientation);

			for (var i = actualStart; i < actualEnd; i++)
			{
				var itemOriented = items[i].DesiredSize.AsOriented(orientation);

				rangeOriented.Direct = Math.Max(rangeOriented.Direct, itemOriented.Direct);
				rangeOriented.Indirect = Math.Max(rangeOriented.Indirect, itemOriented.Indirect);
			}

			rangeOriented.Direct += length;

			for (var i = actualStart; i < actualEnd; i++)
			{
				items[i].Measure(rangeOriented.Size);
				_measure[i] = new ItemMeasure { Size = rangeOriented.Direct };
			}

			return 0;
		}

		private void MeasureVariable(double variableSize)
		{
			if (variableSize < 0 || variableSize.IsPositiveInfinity() || variableSize.IsNaN())
				return;

			_variableSize = variableSize;

			var trackBar = TrackBar;
			var range = trackBar.Maximum - trackBar.Minimum;

			if (range.IsZero())
				return;

			var orientation = trackBar.Orientation;
			var items = trackBar.ItemCollection;
			var itemsCount = items.Count;
			var prevValueIndex = -1;

			PixelRatio = variableSize / range;

			for (var i = 0; i < itemsCount; i++)
			{
				var valueItem = items[i] as TrackBarValueItem;

				if (valueItem == null)
					continue;

				_measure[i] = new ItemMeasure
				{
					Size = valueItem.DesiredSize.AsOriented(orientation).Direct,
					Offset = MeasureRange(prevValueIndex, i, orientation)
				};

				prevValueIndex = i;
			}

			MeasureRange(prevValueIndex, -1, orientation);
		}

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			if (IsInArrange)
				return;

			base.OnChildDesiredSizeChanged(child);
		}

		Orientation IOrientedPanel.Orientation => Orientation;

		private struct ItemMeasure
		{
			public double Offset;
			public double Size;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsInArrange;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsInArrange = allocator.AllocateBoolItem();
			}
		}
	}
}