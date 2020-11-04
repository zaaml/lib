// <copyright file="SizeExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  public static class SizeExtensions
  {
    #region  Methods

    public static OrientedSize AsOriented(this Size size, Orientation orientation)
    {
      return SizeUtils.AsOriented(size, orientation);
    }

    public static Size Ceiling(this Size size)
    {
      return SizeUtils.Ceiling(size);
    }

    public static Size Clamp(this Size value, Size min, Size max)
    {
      return SizeUtils.Clamp(value, min, max);
    }

    public static Size ExpandTo(this Size size, Size value)
    {
      return SizeUtils.ExpandTo(size, value);
    }

    public static OrientedSize ExpandTo(this OrientedSize size, OrientedSize value)
    {
      return SizeUtils.ExpandTo(size, value);
    }

    public static Size Floor(this Size size)
    {
      return SizeUtils.Floor(size);
    }

    public static double GetDirect(this Size size, Orientation orientation)
    {
      return SizeUtils.GetDirect(size, orientation);
    }

    public static double GetIndirect(this Size size, Orientation orientation)
    {
      return SizeUtils.GetIndirect(size, orientation);
    }

    public static double HalfDirect(this OrientedSize orientedSize)
    {
      return SizeUtils.HalfDirect(orientedSize);
    }

    public static double HalfHeight(this Size size)
    {
      return SizeUtils.HalfHeight(size);
    }

    public static double HalfIndirect(this OrientedSize orientedSize)
    {
      return SizeUtils.HalfIndirect(orientedSize);
    }

    public static double HalfWidth(this Size size)
    {
      return SizeUtils.HalfWidth(size);
    }

    public static bool IsCloseTo(this Size a, Size b)
    {
      return SizeUtils.AreClose(a, b);
    }

    internal static Size LayoutRound(this Size size, RoundingMode roundingMode)
    {
      return SizeUtils.LayoutRound(size, roundingMode);
    }

    public static Rect Rect(this Size size)
    {
      return SizeUtils.Rect(size);
    }

    public static Size Round(this Size size, int digits = 0)
    {
      return SizeUtils.Round(size, digits);
    }

    internal static Size Round(this Size size, RoundingMode roundingMode, int digits = 0)
    {
      return SizeUtils.Round(size, roundingMode, digits);
    }

    public static Size Truncate(this Size size)
    {
      return SizeUtils.Truncate(size);
    }

    #endregion
  }
}