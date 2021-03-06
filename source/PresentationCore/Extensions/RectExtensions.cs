// <copyright file="RectExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  public static class RectExtensions
  {
    #region  Methods

    internal static Rect Align(this Rect hostBox, Rect clientBox, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
    {
      return RectUtils.CalcAlignBox(hostBox, clientBox, horizontalAlignment, verticalAlignment);
    }

    public static bool Contains(this Rect rect, Rect another)
    {
      return RectUtils.Contains(rect, another);
    }

    public static Point GetBottomLeft(this Rect rect)
    {
      return RectUtils.GetBottomLeft(rect);
    }

    public static Point GetBottomRight(this Rect rect)
    {
      return RectUtils.GetBottomRight(rect);
    }

    public static Point GetCenter(this Rect rect)
    {
      return RectUtils.GetCenter(rect);
    }

    public static Rect GetInflated(this Rect rect, double width, double height)
    {
      return RectUtils.Inflate(rect, width, height);
    }

    public static Rect GetInflated(this Rect rect, Thickness thickness)
    {
      return RectUtils.Inflate(rect, thickness);
    }

    public static double GetMaxPart(this Rect rect, Orientation orientation)
    {
      return RectUtils.GetMaxPart(rect, orientation);
    }

    public static double GetMinPart(this Rect rect, Orientation orientation)
    {
      return RectUtils.GetMinPart(rect, orientation);
    }

    internal static Point GetPoint(this Rect rect, RectPoint rectPoint)
    {
      return RectUtils.GetPoint(rect, rectPoint);
    }

    public static Range<double> GetRange(this Rect rect, Orientation orientation)
    {
      return RectUtils.GetRange(rect, orientation);
    }

    public static double GetSize(this Rect rect, Orientation orientation)
    {
      return RectUtils.GetSize(rect, orientation);
    }

    public static Point GetTopLeft(this Rect rect)
    {
      return RectUtils.GetTopLeft(rect);
    }

    public static Point GetTopRight(this Rect rect)
    {
      return RectUtils.GetTopRight(rect);
    }

    public static double HalfHeight(this Rect rect)
    {
      return RectUtils.HalfHeight(rect);
    }

    public static double HalfWidth(this Rect rect)
    {
      return RectUtils.HalfWidth(rect);
    }

    public static Rect IntersectionWith(this Rect rect, Rect another)
    {
      return RectUtils.Intersect(rect, another);
    }

    public static bool IntersectsWith(this Rect rect, Rect other)
    {
      return RectUtils.IntersectsWith(rect, other);
    }

    public static bool IsCloseTo(this Rect a, Rect b)
    {
      return RectUtils.AreClose(a, b);
    }

    internal static Rect LayoutRound(this Rect rect, RoundingMode roundingMode)
    {
      return RectUtils.LayoutRound(rect, roundingMode);
    }

    public static Rect Offset(this Rect rect, double offsetX, double offsetY)
    {
      return RectUtils.Offset(rect, offsetX, offsetY);
    }

    public static Rect Offset(this Rect rect, Point offset)
    {
      return RectUtils.Offset(rect, offset);
    }

    public static Rect Offset(this Rect rect, OrientedPoint offset)
    {
      return RectUtils.Offset(rect, offset);
    }

    public static Rect Round(this Rect rect, int digits = 0)
    {
      return RectUtils.Round(rect, digits);
    }

    internal static Rect Round(this Rect rect, RoundingMode roundingMode, int digits = 0)
    {
      return RectUtils.Round(rect, roundingMode, digits);
    }

    internal static Rect WithTopLeft(this Rect rect, Point topLeft)
    {
      return RectUtils.SetTopLeft(rect, topLeft);
    }

    public static Size Size(this Rect rect)
    {
      return RectUtils.Size(rect);
    }

    public static double Square(this Rect rect)
    {
      return RectUtils.Square(rect);
    }

    public static Rect Truncate(this Rect rect)
    {
      return RectUtils.Truncate(rect);
    }

    internal static Rect WithHeight(this Rect rect, double height)
    {
	    rect.Height = height;

	    return rect;
    }

    internal static Rect WithWidth(this Rect rect, double width)
    {
	    rect.Width = width;

	    return rect;
    }

    #endregion
  }
}