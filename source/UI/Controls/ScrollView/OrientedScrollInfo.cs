// <copyright file="AxisScrollInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.ScrollView
{
	internal struct AxisScrollInfo
	{
		private double _extent;
		private double _offset;
		private double _viewport;

		public AxisScrollInfo(Orientation orientation)
		{
			Orientation = orientation;

			_extent = 0.0;
			_offset = 0.0;
			_viewport = 0.0;
		}

		public AxisScrollInfo(Orientation orientation, double offset, double viewport, double extent)
		{
			Orientation = orientation;

			_extent = extent;
			_offset = offset;
			_viewport = viewport;

			ClampOffset();
		}

		public double Extent
		{
			get => _extent;
			set
			{
				if (DoubleUtils.AreClose(_extent, value))
					return;

				_extent = value;

				ClampOffset();
			}
		}

		public double Offset
		{
			get => _offset;
			set
			{
				if (DoubleUtils.AreClose(_offset, value))
					return;

				_offset = value;

				ClampOffset();
			}
		}

		public Orientation Orientation { get; }

		public double ScrollableSize => Math.Max(0, Extent - Viewport);

		public double Viewport
		{
			get => _viewport;
			set
			{
				if (DoubleUtils.AreClose(_viewport, value))
					return;

				_viewport = value;

				ClampOffset();
			}
		}

		private void ClampOffset()
		{
			_offset = _offset.Clamp(0, ScrollableSize);
		}

		public void ExecuteScrollCommand(ScrollCommandKind command, double smallChange, double wheelChange)
		{
			var commandOrientation = ScrollViewUtils.GetCommandOrientation(command);

			if (commandOrientation != null && commandOrientation != Orientation)
				return;

			var offset = ScrollViewUtils.ExecuteScrollCommand(new Vector(_offset, _offset), command, new Size(_viewport, _viewport), new Size(_extent, _extent), smallChange, wheelChange);

			_offset = Orientation == Orientation.Horizontal ? offset.X : offset.Y;

			ClampOffset();
		}

		public double GetSize(Size size)
		{
			return size.AsOriented(Orientation).Direct;
		}

		public bool IsCloseTo(AxisScrollInfo scrollInfo)
		{
			if (Orientation != scrollInfo.Orientation)
				throw new InvalidOperationException();

			return Offset.IsCloseTo(scrollInfo.Offset) && Extent.IsCloseTo(scrollInfo.Extent) && Viewport.IsCloseTo(scrollInfo.Viewport);
		}
	}
}
