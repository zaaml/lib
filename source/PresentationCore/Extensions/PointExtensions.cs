// <copyright file="PointExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
	public static class PointExtensions
	{
		public static OrientedPoint AsOriented(this Point point, Orientation orientation)
		{
			return PointUtils.AsOriented(point, orientation);
		}

		public static double GetPart(this Point point, Orientation orientation)
		{
			return PointUtils.GetCoordinate(point, orientation);
		}

		public static bool IsCloseTo(this Point a, Point b)
		{
			return PointUtils.AreClose(a, b);
		}

		internal static Point LayoutRound(this Point point, RoundingMode roundingMode)
		{
			return PointUtils.LayoutRound(point, roundingMode);
		}

		public static Point Negate(this Point value)
		{
			return PointUtils.Negate(value);
		}

		public static Point Round(this Point point, int digits = 0)
		{
			return PointUtils.Round(point, digits);
		}

		internal static Point Round(this Point point, RoundingMode roundingMode, int digits = 0)
		{
			return PointUtils.Round(point, roundingMode, digits);
		}

		public static Point Scale(this Point point, double scaleX, double scaleY)
		{
			return PointUtils.Scale(point, scaleX, scaleY);
		}

		public static Point Scale(this Point point, double scale)
		{
			return PointUtils.Scale(point, scale);
		}

		public static Point SetCoordinate(this Point point, Orientation orientation, double value)
		{
			return PointUtils.SetCoordinate(point, orientation, value);
		}

		public static Point Truncate(this Point point)
		{
			return PointUtils.Truncate(point);
		}

		public static Point WithOffset(this Point point, Point offset)
		{
			return PointUtils.Offset(point, offset);
		}

		public static Point WithOffset(this Point point, OrientedPoint offset)
		{
			return PointUtils.Offset(point, offset.Point);
		}

		public static Point WithOffset(this Point point, double offsetX, double offsetY)
		{
			return PointUtils.Offset(point, offsetX, offsetY);
		}
	}
}