// <copyright file="ScrollInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.ScrollView
{
	public readonly struct ScrollInfo
	{
		public ScrollInfo(Vector offset, Size viewport, Size extent)
		{
			Offset = offset;
			Extent = extent;
			Viewport = viewport;

			Offset = ClampOffset(offset);
		}

		public Size Extent { get; }

		public Vector Offset { get; }

		public Size Viewport { get; }

		public Size ScrollableSize => new Size
		{
			Height = Math.Max(Extent.Height - Viewport.Height, 0),
			Width = Math.Max(Extent.Width - Viewport.Width, 0),
		};

		public ScrollInfo WithOffset(Vector offset)
		{
			offset = ClampOffset(offset);

			return new ScrollInfo(offset, Viewport, Extent);
		}

		internal ScrollInfo WithExtent(Size extent)
		{
			return new ScrollInfo(Offset, Viewport, extent);
		}

		internal ScrollInfo WithViewport(Size viewport)
		{
			return new ScrollInfo(Offset, viewport, Extent);
		}

		internal ScrollInfo WithOffset(Vector offset, bool expandExtent)
		{
			if (expandExtent == false)
				return WithOffset(offset);

			var extent = Extent;

			if (extent.Width < Viewport.Width + offset.X)
				extent.Width = Viewport.Width + offset.X;

			if (extent.Height < Viewport.Height + offset.Y)
				extent.Height = Viewport.Height + offset.Y;

			return new ScrollInfo(offset, Viewport, extent);
		}

		internal Vector ClampOffset(Vector offset)
		{
			var scrollableSize = ScrollableSize;

			offset.X = offset.X.Clamp(0, scrollableSize.Width);
			offset.Y = offset.Y.Clamp(0, scrollableSize.Height);

			return offset;
		}

		public bool Equals(ScrollInfo other)
		{
			return Extent.Equals(other.Extent) && Viewport.Equals(other.Viewport) && Offset.Equals(other.Offset);
		}

		public override bool Equals(object obj)
		{
			return obj is ScrollInfo other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = EqualityComparer<Size>.Default.GetHashCode(Extent);

				hashCode = (hashCode * 397) ^ EqualityComparer<Size>.Default.GetHashCode(Viewport);
				hashCode = (hashCode * 397) ^ EqualityComparer<Vector>.Default.GetHashCode(Offset);

				return hashCode;
			}
		}
	}
}