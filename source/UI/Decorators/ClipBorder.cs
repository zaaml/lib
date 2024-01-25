// <copyright file="ClipBorder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// Original License and source

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// https://github.com/MahApps

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Decorators
{
	public sealed class ClipBorder : Decorator
	{
		public static readonly DependencyProperty BorderThicknessProperty
			= DependencyProperty.Register(nameof(BorderThickness),
				typeof(Thickness),
				typeof(ClipBorder),
				new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
				OnValidateThickness);

		public static readonly DependencyProperty PaddingProperty
			= DependencyProperty.Register(nameof(Padding),
				typeof(Thickness),
				typeof(ClipBorder),
				new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
				OnValidateThickness);

		public static readonly DependencyProperty CornerRadiusProperty
			= DependencyProperty.Register(nameof(CornerRadius),
				typeof(CornerRadius),
				typeof(ClipBorder),
				new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
				OnValidateCornerRadius);

		public static readonly DependencyProperty BorderBrushProperty
			= DependencyProperty.Register(nameof(BorderBrush),
				typeof(Brush),
				typeof(ClipBorder),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

		public static readonly DependencyProperty BackgroundProperty
			= DependencyProperty.Register(nameof(Background),
				typeof(Brush),
				typeof(ClipBorder),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

		public static readonly DependencyProperty OptimizeClipRenderingProperty
			= DependencyProperty.Register(nameof(OptimizeClipRendering),
				typeof(bool),
				typeof(ClipBorder),
				new FrameworkPropertyMetadata(BooleanBoxes.False, FrameworkPropertyMetadataOptions.AffectsRender));

		private StreamGeometry _backgroundGeometryCache;
		private StreamGeometry _borderGeometryCache;

		public Brush Background
		{
			get => (Brush)GetValue(BackgroundProperty);
			set => SetValue(BackgroundProperty, value);
		}

		public Brush BorderBrush
		{
			get => (Brush)GetValue(BorderBrushProperty);
			set => SetValue(BorderBrushProperty, value);
		}

		public Thickness BorderThickness
		{
			get => (Thickness)GetValue(BorderThicknessProperty);
			set => SetValue(BorderThicknessProperty, value);
		}

		public CornerRadius CornerRadius
		{
			get => (CornerRadius)GetValue(CornerRadiusProperty);
			set => SetValue(CornerRadiusProperty, value);
		}

		public bool OptimizeClipRendering
		{
			get => (bool)GetValue(OptimizeClipRenderingProperty);
			set => SetValue(OptimizeClipRenderingProperty, value.Box());
		}

		public Thickness Padding
		{
			get => (Thickness)GetValue(PaddingProperty);
			set => SetValue(PaddingProperty, value);
		}

		internal static Geometry BuildRoundRectangleGeometry(Rect rect, CornerRadius cornerRadius)
		{
			var outerBorderInfo = new BorderInfo(cornerRadius, new Thickness(), new Thickness(), true);
			var borderGeometry = new StreamGeometry();

			using (var ctx = borderGeometry.Open())
				GenerateGeometry(ctx, rect, outerBorderInfo);

			borderGeometry.Freeze();

			return borderGeometry;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var borders = BorderThickness;
			var boundRect = new Rect(finalSize);
			var innerRect = RectUtils.Deflate(boundRect, borders);
			var corners = CornerRadius;
			var padding = Padding;
			var childRect = RectUtils.Deflate(innerRect, padding);

			if (!boundRect.Width.IsZero() && !boundRect.Height.IsZero())
			{
				var outerBorderInfo = new BorderInfo(corners, borders, new Thickness(), true);
				var borderGeometry = new StreamGeometry();

				using (var ctx = borderGeometry.Open())
					GenerateGeometry(ctx, boundRect, outerBorderInfo);

				borderGeometry.Freeze();

				_borderGeometryCache = borderGeometry;
			}
			else
				_borderGeometryCache = null;

			if (!innerRect.Width.IsZero() && !innerRect.Height.IsZero())
			{
				var innerBorderInfo = new BorderInfo(corners, borders, new Thickness(), false);
				var backgroundGeometry = new StreamGeometry();

				using (var ctx = backgroundGeometry.Open())
					GenerateGeometry(ctx, innerRect, innerBorderInfo);

				backgroundGeometry.Freeze();
				_backgroundGeometryCache = backgroundGeometry;
			}
			else
				_backgroundGeometryCache = null;

			var child = Child;

			if (child != null)
			{
				child.Arrange(childRect);

				var clipGeometry = new StreamGeometry();
				var childBorderInfo = new BorderInfo(corners, borders, padding, false);

				using (var ctx = clipGeometry.Open())
					GenerateGeometry(ctx, new Rect(0, 0, childRect.Width, childRect.Height), childBorderInfo);

				clipGeometry.Freeze();
				child.Clip = clipGeometry;
			}

			return finalSize;
		}

		private static void GenerateGeometry(StreamGeometryContext ctx, Rect rect, BorderInfo borderInfo)
		{
			var leftTop = new Point(borderInfo.LeftTop, 0);
			var rightTop = new Point(rect.Width - borderInfo.RightTop, 0);
			var topRight = new Point(rect.Width, borderInfo.TopRight);
			var bottomRight = new Point(rect.Width, rect.Height - borderInfo.BottomRight);
			var rightBottom = new Point(rect.Width - borderInfo.RightBottom, rect.Height);
			var leftBottom = new Point(borderInfo.LeftBottom, rect.Height);
			var bottomLeft = new Point(0, rect.Height - borderInfo.BottomLeft);
			var topLeft = new Point(0, borderInfo.TopLeft);

			if (leftTop.X > rightTop.X)
			{
				var v = borderInfo.LeftTop / (borderInfo.LeftTop + borderInfo.RightTop) * rect.Width;

				leftTop.X = v;
				rightTop.X = v;
			}

			if (topRight.Y > bottomRight.Y)
			{
				var v = borderInfo.TopRight / (borderInfo.TopRight + borderInfo.BottomRight) * rect.Height;

				topRight.Y = v;
				bottomRight.Y = v;
			}

			if (leftBottom.X > rightBottom.X)
			{
				var v = borderInfo.LeftBottom / (borderInfo.LeftBottom + borderInfo.RightBottom) * rect.Width;

				rightBottom.X = v;
				leftBottom.X = v;
			}

			if (topLeft.Y > bottomLeft.Y)
			{
				var v = borderInfo.TopLeft / (borderInfo.TopLeft + borderInfo.BottomLeft) * rect.Height;

				bottomLeft.Y = v;
				topLeft.Y = v;
			}

			var offsetX = rect.TopLeft.X;
			var offsetY = rect.TopLeft.Y;
			var offset = new Vector(offsetX, offsetY);

			leftTop += offset;
			rightTop += offset;
			topRight += offset;
			bottomRight += offset;
			rightBottom += offset;
			leftBottom += offset;
			bottomLeft += offset;
			topLeft += offset;

			ctx.BeginFigure(leftTop, true, true);

			ctx.LineTo(rightTop, true, false);

			var radiusX = rect.TopRight.X - rightTop.X;
			var radiusY = topRight.Y - rect.TopRight.Y;

			if (!radiusX.IsZero() || !radiusY.IsZero())
				ctx.ArcTo(topRight, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);

			ctx.LineTo(bottomRight, true, false);

			radiusX = rect.BottomRight.X - rightBottom.X;
			radiusY = rect.BottomRight.Y - bottomRight.Y;

			if (!radiusX.IsZero() || !radiusY.IsZero())
				ctx.ArcTo(rightBottom, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);

			ctx.LineTo(leftBottom, true, false);

			radiusX = leftBottom.X - rect.BottomLeft.X;
			radiusY = rect.BottomLeft.Y - bottomLeft.Y;

			if (!radiusX.IsZero() || !radiusY.IsZero())
				ctx.ArcTo(bottomLeft, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);

			ctx.LineTo(topLeft, true, false);

			radiusX = leftTop.X - rect.TopLeft.X;
			radiusY = topLeft.Y - rect.TopLeft.Y;

			if (!radiusX.IsZero() || !radiusY.IsZero())
				ctx.ArcTo(leftTop, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var child = Child;
			var desiredSize = new Size();
			var borders = BorderThickness;

			var borderSize = borders.Collapse();
			var paddingSize = Padding.Collapse();

			if (child != null)
			{
				var combined = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);
				var childConstraint = new Size(Math.Max(0.0, constraint.Width - combined.Width),
					Math.Max(0.0, constraint.Height - combined.Height));

				child.Measure(childConstraint);

				var childSize = child.DesiredSize;

				desiredSize.Width = childSize.Width + combined.Width;
				desiredSize.Height = childSize.Height + combined.Height;
			}
			else
				desiredSize = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);

			return desiredSize;
		}

		protected override void OnRender(DrawingContext dc)
		{
			var borders = BorderThickness;
			var borderBrush = BorderBrush;
			var bgBrush = Background;
			var borderGeometry = _borderGeometryCache;
			var backgroundGeometry = _backgroundGeometryCache;
			var optimizeClipRendering = OptimizeClipRendering;

			if (optimizeClipRendering)
			{
				dc.DrawGeometry(borderBrush, null, borderGeometry);

				return;
			}

			if (borderBrush != null && !borders.IsZero() && bgBrush != null)
			{
				if (borderBrush.IsEqualTo(bgBrush))
					dc.DrawGeometry(borderBrush, null, borderGeometry);
				else if (borderBrush.IsOpaque() && bgBrush.IsOpaque())
				{
					dc.DrawGeometry(borderBrush, null, borderGeometry);
					dc.DrawGeometry(bgBrush, null, backgroundGeometry);
				}
				else if (borderBrush.IsOpaque())
				{
					if (borderGeometry == null || backgroundGeometry == null)
						return;

					var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
					var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
					var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath,
						GeometryCombineMode.Exclude, null);

					dc.DrawGeometry(bgBrush, null, borderGeometry);
					dc.DrawGeometry(borderBrush, null, borderOutlineGeometry);
				}
				else
				{
					if (borderGeometry == null || backgroundGeometry == null)
						return;

					var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
					var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
					var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath,
						GeometryCombineMode.Exclude, null);

					dc.DrawGeometry(borderBrush, null, borderOutlineGeometry);
					dc.DrawGeometry(bgBrush, null, backgroundGeometry);
				}

				return;
			}

			if (borderBrush != null && !borders.IsZero())
			{
				if (borderGeometry != null && backgroundGeometry != null)
				{
					var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
					var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
					var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath,
						GeometryCombineMode.Exclude, null);

					dc.DrawGeometry(borderBrush, null, borderOutlineGeometry);
				}
				else
					dc.DrawGeometry(borderBrush, null, borderGeometry);
			}

			if (bgBrush != null)
				dc.DrawGeometry(bgBrush, null, backgroundGeometry);
		}

		private static bool OnValidateCornerRadius(object value)
		{
			var cr = (CornerRadius)(value ?? default(CornerRadius));

			return cr.IsValid(false, false, false, false);
		}

		private static bool OnValidateThickness(object value)
		{
			var th = (Thickness)(value ?? default(Thickness));

			return th.IsValid(false, false, false, false);
		}

		private readonly struct BorderInfo
		{
			public readonly double LeftTop;
			public readonly double TopLeft;
			public readonly double TopRight;
			public readonly double RightTop;
			public readonly double RightBottom;
			public readonly double BottomRight;
			public readonly double BottomLeft;
			public readonly double LeftBottom;

			internal BorderInfo(CornerRadius corners, Thickness borders, Thickness padding, bool isOuterBorder)
			{
				var left = 0.5 * borders.Left + padding.Left;
				var top = 0.5 * borders.Top + padding.Top;
				var right = 0.5 * borders.Right + padding.Right;
				var bottom = 0.5 * borders.Bottom + padding.Bottom;

				if (isOuterBorder)
				{
					if (corners.TopLeft.IsZero())
						LeftTop = TopLeft = 0.0;
					else
					{
						LeftTop = corners.TopLeft + left;
						TopLeft = corners.TopLeft + top;
					}

					if (corners.TopRight.IsZero())
						TopRight = RightTop = 0.0;
					else
					{
						TopRight = corners.TopRight + top;
						RightTop = corners.TopRight + right;
					}

					if (corners.BottomRight.IsZero())
						RightBottom = BottomRight = 0.0;
					else
					{
						RightBottom = corners.BottomRight + right;
						BottomRight = corners.BottomRight + bottom;
					}

					if (corners.BottomLeft.IsZero())
						BottomLeft = LeftBottom = 0.0;
					else
					{
						BottomLeft = corners.BottomLeft + bottom;
						LeftBottom = corners.BottomLeft + left;
					}
				}
				else
				{
					LeftTop = Math.Max(0.0, corners.TopLeft - left);
					TopLeft = Math.Max(0.0, corners.TopLeft - top);
					TopRight = Math.Max(0.0, corners.TopRight - top);
					RightTop = Math.Max(0.0, corners.TopRight - right);
					RightBottom = Math.Max(0.0, corners.BottomRight - right);
					BottomRight = Math.Max(0.0, corners.BottomRight - bottom);
					BottomLeft = Math.Max(0.0, corners.BottomLeft - bottom);
					LeftBottom = Math.Max(0.0, corners.BottomLeft - left);
				}
			}
		}
	}
}