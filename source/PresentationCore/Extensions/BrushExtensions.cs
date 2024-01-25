// <copyright file="BrushExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
	public static class BrushExtensions
	{
		public static SolidColorBrush CloneBrush(this SolidColorBrush brush)
		{
			return BrushUtils.CloneBrush(brush);
		}

		public static bool IsOpaque(this Brush brush)
		{
			return (brush as SolidColorBrush)?.Color.A == 0xFF;
		}

		public static bool IsEqualTo(this Brush brush, Brush otherBrush)
		{
			if (brush.GetType() != otherBrush.GetType())
				return false;

			if (ReferenceEquals(brush, otherBrush))
				return true;

			if (brush is SolidColorBrush solidBrushA && otherBrush is SolidColorBrush solidBrushB)
			{
				return solidBrushA.Color == solidBrushB.Color
							 && solidBrushA.Opacity.IsCloseTo(solidBrushB.Opacity);
			}

			if (brush is LinearGradientBrush linGradBrushA && otherBrush is LinearGradientBrush linGradBrushB)
			{
				var result = linGradBrushA.ColorInterpolationMode == linGradBrushB.ColorInterpolationMode
										 && linGradBrushA.EndPoint == linGradBrushB.EndPoint
										 && linGradBrushA.MappingMode == linGradBrushB.MappingMode
										 && linGradBrushA.Opacity.IsCloseTo(linGradBrushB.Opacity)
										 && linGradBrushA.StartPoint == linGradBrushB.StartPoint
										 && linGradBrushA.SpreadMethod == linGradBrushB.SpreadMethod
										 && linGradBrushA.GradientStops.Count == linGradBrushB.GradientStops.Count;
				if (!result)
					return false;

				for (var i = 0; i < linGradBrushA.GradientStops.Count; i++)
				{
					result = linGradBrushA.GradientStops[i].Color == linGradBrushB.GradientStops[i].Color
									 && linGradBrushA.GradientStops[i].Offset.IsCloseTo(linGradBrushB.GradientStops[i].Offset);

					if (!result)
						break;
				}

				return result;
			}

			if (brush is RadialGradientBrush radGradBrushA && otherBrush is RadialGradientBrush radGradBrushB)
			{
				var result = radGradBrushA.ColorInterpolationMode == radGradBrushB.ColorInterpolationMode
										 && radGradBrushA.GradientOrigin == radGradBrushB.GradientOrigin
										 && radGradBrushA.MappingMode == radGradBrushB.MappingMode
										 && radGradBrushA.Opacity.IsCloseTo(radGradBrushB.Opacity)
										 && radGradBrushA.RadiusX.IsCloseTo(radGradBrushB.RadiusX)
										 && radGradBrushA.RadiusY.IsCloseTo(radGradBrushB.RadiusY)
										 && radGradBrushA.SpreadMethod == radGradBrushB.SpreadMethod
										 && radGradBrushA.GradientStops.Count == radGradBrushB.GradientStops.Count;

				if (!result)
					return false;

				for (var i = 0; i < radGradBrushA.GradientStops.Count; i++)
				{
					result = radGradBrushA.GradientStops[i].Color == radGradBrushB.GradientStops[i].Color
									 && radGradBrushA.GradientStops[i].Offset.IsCloseTo(radGradBrushB.GradientStops[i].Offset);

					if (!result)
						break;
				}

				return result;
			}

			if (brush is ImageBrush imgBrushA && otherBrush is ImageBrush imgBrushB)
			{
				var result = imgBrushA.AlignmentX == imgBrushB.AlignmentX
										 && imgBrushA.AlignmentY == imgBrushB.AlignmentY
										 && imgBrushA.Opacity.IsCloseTo(imgBrushB.Opacity)
										 && imgBrushA.Stretch == imgBrushB.Stretch
										 && imgBrushA.TileMode == imgBrushB.TileMode
										 && imgBrushA.Viewbox == imgBrushB.Viewbox
										 && imgBrushA.ViewboxUnits == imgBrushB.ViewboxUnits
										 && imgBrushA.Viewport == imgBrushB.Viewport
										 && imgBrushA.ViewportUnits == imgBrushB.ViewportUnits
										 && imgBrushA.ImageSource == imgBrushB.ImageSource;

				return result;
			}

			return false;
		}
	}
}