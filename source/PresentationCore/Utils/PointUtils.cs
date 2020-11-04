// <copyright file="PointUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
  public static class PointUtils
  {
    #region  Methods

    public static Point AddVector(Point point, Vector vector)
    {
      return new Point(point.X + vector.X, point.Y + vector.Y);
    }

    public static bool AreClose(Point a, Point b)
    {
      return DoubleUtils.AreClose(a.X, b.X, XamlConstants.LayoutComparisonPrecision) && DoubleUtils.AreClose(a.Y, b.Y, XamlConstants.LayoutComparisonPrecision);
    }

    public static OrientedPoint AsOriented(Point point, Orientation orientation)
    {
      return new OrientedPoint(orientation, point);
    }

    public static Rect CalcBounds(IEnumerable<Point> points)
    {
      var minx = double.MaxValue;
      var maxx = double.MinValue;
      var miny = double.MaxValue;
      var maxy = double.MinValue;
			var any = false;

      foreach (var point in points)
      {
        any = true;

        minx = Math.Min(minx, point.X);
        maxx = Math.Max(maxx, point.X);
        miny = Math.Min(miny, point.Y);
        maxy = Math.Max(maxy, point.Y);
      }

      return any ? new Rect(minx, miny, maxx - minx, maxy - miny) : Rect.Empty;
    }

    public static double GetCoordinate(Point point, Orientation orientation)
    {
      return orientation.IsHorizontal() ? point.X : point.Y;
    }

    public static Point Negate(Point value)
    {
      return new Point(-value.X, -value.Y);
    }

    public static Point Offset(Point point, Point offset)
    {
      return Offset(point, offset.X, offset.Y);
    }

    public static Point Offset(Point point, double offsetX, double offsetY)
    {
      point.X += offsetX;
      point.Y += offsetY;

      return point;
    }

    public static Point Round(Point point, int digits = 0)
    {
      point.X = Math.Round(point.X, digits);
      point.Y = Math.Round(point.Y, digits);

      return point;
    }

    internal static Point Round(Point point, RoundingMode roundingMode, int digits = 0)
    {
      point.X = Math.Round(point.X, digits);
      point.Y = Math.Round(point.Y, digits);

      return point;
    }

	  internal static Point LayoutRound(Point point, RoundingMode roundingMode)
	  {
		  point.X = LayoutUtils.RoundX(point.X, roundingMode);
		  point.Y = LayoutUtils.RoundY(point.Y, roundingMode);

		  return point;
	  }

		public static Point Truncate(Point point)
    {
      point.X = DoubleUtils.Truncate(point.X);
      point.Y = DoubleUtils.Truncate(point.Y);

      return point;
    }

    public static Point Scale(Point point, double scaleX, double scaleY)
    {
      return new Point(point.X*scaleX, point.Y*scaleY);
    }

    public static Point Scale(Point point, double scale)
    {
      return new Point(point.X*scale, point.Y*scale);
    }

    public static Point SetCoordinate(Point point, Orientation orientation, double value)
    {
      if (orientation == Orientation.Horizontal)
        point.X = value;
      else
        point.Y = value;
      return point;
    }

    public static Vector SubtractPoints(Point first, Point second)
    {
      return new Vector(first.X - second.X, first.Y - second.Y);
    }

    #endregion
  }
}