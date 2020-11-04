// <copyright file="RectUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
using Range = Zaaml.Core.Range;

namespace Zaaml.PresentationCore.Utils
{
  internal enum RectPoint
  {
    TopLeft,
    TopRight,
    BottomRight,
    BottomLeft,
    Center
  }

  internal enum RectSide
  {
		Left,
		Top,
		Right,
		Bottom
  }

  internal enum HorizontalPoint
  {
    Right,
    Left
  }

  internal enum VerticalPoint
  {
    Top,
    Bottom
  }

  public static class RectUtils
  {
    #region  Methods

    public static bool AreClose(Rect a, Rect b)
    {
      return a.GetTopLeft().IsCloseTo(b.GetTopLeft()) && a.Size().IsCloseTo(b.Size());
    }

    internal static Rect CalcAlignBox(Rect hostBox, Rect clientBox, Alignment alignment)
    {
      return CalcAlignBox(hostBox, clientBox, alignment.Horizontal, alignment.Vertical);
    }

    internal static Rect CalcAlignBox(Rect hostBox, Rect clientBox, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
    {
      if (hostBox.IsEmpty)
        return clientBox;

      var left = 0.0;
      var top = 0.0;
      var width = clientBox.Width;
      var height = clientBox.Height;
      var hostBoxCenter = hostBox.GetCenter();

      switch (horizontalAlignment)
      {
        case HorizontalAlignment.Left:
          left = hostBox.Left;
          break;
        case HorizontalAlignment.Center:
          left = hostBoxCenter.X - clientBox.Width / 2;
          break;
        case HorizontalAlignment.Right:
          left = hostBox.Right - clientBox.Width;
          break;
        case HorizontalAlignment.Stretch:
          left = hostBox.Left;
          width = hostBox.Width;
          break;
      }

      switch (verticalAlignment)
      {
        case VerticalAlignment.Top:
          top = hostBox.Top;
          break;
        case VerticalAlignment.Center:
          top = hostBoxCenter.Y - clientBox.Height / 2;
          break;
        case VerticalAlignment.Bottom:
          top = hostBox.Bottom - clientBox.Height;
          break;
        case VerticalAlignment.Stretch:
          top = hostBox.Top;
          height = hostBox.Height;
          break;
      }

      return new Rect(left, top, width, height);
    }

    public static Rect CalcBounds(IEnumerable<Rect> rects)
    {
      var minx = double.MaxValue;
      var maxx = double.MinValue;
      var miny = double.MaxValue;
      var maxy = double.MinValue;

      var any = false;

      foreach (var rect in rects)
      {
        any = true;
        minx = Math.Min(minx, rect.Left);
        miny = Math.Min(miny, rect.Top);
        maxx = Math.Max(maxx, rect.Right);
        maxy = Math.Max(maxy, rect.Bottom);
      }

      return any ? new Rect(minx, miny, maxx - minx, maxy - miny) : System.Windows.Rect.Empty;
    }

    public static bool Contains(Rect rect, Rect another)
    {
      return rect.Contains(another.GetTopLeft()) && rect.Contains(another.GetBottomRight());
    }

    internal static RectPoint DiagonalOpposite(this RectPoint rectPoint)
    {
      rectPoint.ToHVPoints(out var horizontal, out var vertical);

      return GetRectPoint(horizontal.Opposite(), vertical.Opposite());
    }

    public static Point GetBottomLeft(Rect rect)
    {
      return new Point(rect.Left, rect.Bottom);
    }

    public static Point GetBottomRight(Rect rect)
    {
      return new Point(rect.Right, rect.Bottom);
    }

    public static Point GetCenter(Rect rect)
    {
      return new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2);
    }

    public static double GetMaxPart(Rect rect, Orientation orientation)
    {
      return orientation.IsVertical() ? rect.Bottom : rect.Right;
    }

    public static double GetMinPart(Rect rect, Orientation orientation)
    {
      return orientation.IsVertical() ? rect.Top : rect.Left;
    }

    internal static Point GetPoint(Rect rect, RectPoint rectPoint)
    {
      switch (rectPoint)
      {
        case RectPoint.TopLeft:
          return rect.GetTopLeft();
        case RectPoint.TopRight:
          return rect.GetTopRight();
        case RectPoint.BottomRight:
          return rect.GetBottomRight();
        case RectPoint.BottomLeft:
          return rect.GetBottomLeft();
        case RectPoint.Center:
          return rect.GetCenter();
      }

      throw new ArgumentException(nameof(rectPoint));
    }

    internal static double GetSideAxisValue(Rect rect, RectSide side)
    {
	    switch (side)
	    {
		    case RectSide.Left:
			    return rect.Left;
		    case RectSide.Top:
			    return rect.Top;
		    case RectSide.Right:
			    return rect.Right;
		    case RectSide.Bottom:
			    return rect.Bottom;
		    default:
			    throw new ArgumentOutOfRangeException(nameof(side), side, null);
	    }
    }

    public static Range<double> GetRange(Rect rect, Orientation orientation)
    {
      return orientation.IsVertical() ? Range.Create(rect.Top, rect.Bottom) : Range.Create(rect.Left, rect.Right);
    }

    internal static RectPoint GetRectPoint(HorizontalPoint horizontal, VerticalPoint vertical)
    {
      if (vertical == VerticalPoint.Top)
      {
	      switch (horizontal)
	      {
		      case HorizontalPoint.Right:
			      return RectPoint.TopRight;
		      case HorizontalPoint.Left:
			      return RectPoint.TopLeft;
	      }
      }

      if (vertical == VerticalPoint.Bottom)
      {
	      switch (horizontal)
	      {
		      case HorizontalPoint.Right:
			      return RectPoint.BottomRight;
		      case HorizontalPoint.Left:
			      return RectPoint.BottomLeft;
	      }
      }

      throw new Exception("Invalid combination");
    }

    public static double GetSize(Rect rect, Orientation orientation)
    {
      return orientation.IsVertical() ? rect.Height : rect.Width;
    }

    public static Point GetTopLeft(Rect rect)
    {
      return new Point(rect.Left, rect.Top);
    }

    public static Point GetTopRight(Rect rect)
    {
      return new Point(rect.Right, rect.Top);
    }

    public static double HalfHeight(Rect rect)
    {
      return rect.Height / 2.0;
    }

    public static double HalfWidth(Rect rect)
    {
      return rect.Width / 2.0;
    }

    internal static RectPoint HorizontalOpposite(this RectPoint rectPoint)
    {
      rectPoint.ToHVPoints(out var horizontal, out var vertical);

      return GetRectPoint(horizontal.Opposite(), vertical);
    }

    public static Rect Inflate(Rect rect, double width, double height)
    {
      rect.X -= width;
      rect.Y -= height;

      rect.Width = Math.Max(0, rect.Width + 2 * width);
      rect.Height = Math.Max(0, rect.Height + 2 * height);

      return rect;
    }

    public static Rect Inflate(Rect rect, Thickness thickness)
    {
      rect.X -= thickness.Left;
      rect.Y -= thickness.Top;

      rect.Width = Math.Max(0, rect.Width + thickness.Left + thickness.Right);
      rect.Height = Math.Max(0, rect.Height + thickness.Top + thickness.Bottom);

      return rect;
    }

    public static Rect Intersect(Rect rect, Rect another)
    {
      rect.Intersect(another);

      return rect;
    }

    public static bool IntersectsWith(Rect rect, Rect other)
    {
      other.Intersect(rect);

      return other.IsEmpty == false;
    }

    internal static Rect LayoutRound(Rect rect, RoundingMode roundingMode)
    {
      return new Rect(rect.GetTopLeft().LayoutRound(roundingMode), rect.Size().LayoutRound(roundingMode));
    }

    public static Rect Offset(Rect rect, double offsetX, double offsetY)
    {
      rect.X += offsetX;
      rect.Y += offsetY;

      return rect;
    }

    public static Rect Offset(Rect rect, Point offset)
    {
      return Offset(rect, offset.X, offset.Y);
    }

    public static Rect Offset(Rect rect, OrientedPoint offset)
    {
      return offset.Orientation == Orientation.Horizontal ? Offset(rect, offset.Direct, offset.Indirect) : Offset(rect, offset.Indirect, offset.Direct);
    }

    internal static HorizontalPoint Opposite(this HorizontalPoint point)
    {
      return point == HorizontalPoint.Left ? HorizontalPoint.Right : HorizontalPoint.Left;
    }

    internal static VerticalPoint Opposite(this VerticalPoint point)
    {
      return point == VerticalPoint.Top ? VerticalPoint.Bottom : VerticalPoint.Top;
    }

    public static Rect Rect(Size size)
    {
      return new Rect(0, 0, size.Width, size.Height);
    }

    public static Rect Round(Rect rect, int digits = 0)
    {
      return new Rect(rect.GetTopLeft().Round(digits), rect.Size().Round(digits));
    }

    internal static Rect Round(Rect rect, RoundingMode roundingMode, int digits = 0)
    {
      return new Rect(rect.GetTopLeft().Round(roundingMode, digits), rect.Size().Round(roundingMode, digits));
    }

    internal static Rect SetTopLeft(Rect rect, Point topLeft)
    {
      rect.X = topLeft.X;
      rect.Y = topLeft.Y;

      return rect;
    }

    public static Size Size(Rect rect)
    {
      return rect.IsEmpty ? XamlConstants.ZeroSize : new Size(rect.Width, rect.Height);
    }

    public static double Square(Rect rect)
    {
      return rect.Width * rect.Height;
    }

    internal static void ToHVPoints(this RectPoint rectPoint, out HorizontalPoint horizontalPoint, out VerticalPoint verticalPoint)
    {
      horizontalPoint = HorizontalPoint.Left;
      verticalPoint = VerticalPoint.Top;

      switch (rectPoint)
      {
        case RectPoint.TopLeft:
          horizontalPoint = HorizontalPoint.Left;
          verticalPoint = VerticalPoint.Top;
          break;
        case RectPoint.TopRight:
          horizontalPoint = HorizontalPoint.Right;
          verticalPoint = VerticalPoint.Top;
          break;
        case RectPoint.BottomRight:
          horizontalPoint = HorizontalPoint.Right;
          verticalPoint = VerticalPoint.Bottom;
          break;
        case RectPoint.BottomLeft:
          horizontalPoint = HorizontalPoint.Left;
          verticalPoint = VerticalPoint.Bottom;
          break;
        case RectPoint.Center:
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(rectPoint));
      }
    }

    public static Rect Truncate(Rect rect)
    {
      return new Rect(rect.GetTopLeft().Truncate(), rect.Size().Truncate());
    }

    internal static RectPoint VerticalOpposite(this RectPoint rectPoint)
    {
      rectPoint.ToHVPoints(out var horizontal, out var vertical);

      return GetRectPoint(horizontal, vertical.Opposite());
    }

    #endregion
  }
}