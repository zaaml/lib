// <copyright file="SizeUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
  public static class SizeUtils
  {
    #region  Methods

    public static bool AreClose(Size a, Size b)
    {
      return DoubleUtils.AreClose(a.Width, b.Width, XamlConstants.LayoutComparisonPrecision) &&
             DoubleUtils.AreClose(a.Height, b.Height, XamlConstants.LayoutComparisonPrecision);
    }

    public static OrientedSize AsOriented(Size size, Orientation orientation)
    {
      return new OrientedSize(orientation, size);
    }

    public static Size Ceiling(Size size)
    {
      return new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
    }

    public static Size Clamp(Size value, Size min, Size max)
    {
      return new Size(value.Width.Clamp(min.Width, max.Width), value.Height.Clamp(min.Height, max.Height));
    }

    public static Size ExpandTo(Size size, Size another)
    {
      return new Size(Math.Max(size.Width, another.Width), Math.Max(size.Height, another.Height));
    }

    public static OrientedSize ExpandTo(OrientedSize size, OrientedSize another)
    {
      var clone = size;

      clone.Size = clone.Size.ExpandTo(another.Size);

      return clone;
    }

    public static Size Floor(Size size)
    {
      return new Size(Math.Floor(size.Width), Math.Floor(size.Height));
    }

    public static double GetDirect(Size size, Orientation orientation)
    {
      return OrientedSize.GetDirect(size, orientation);
    }

    public static double GetIndirect(Size size, Orientation orientation)
    {
      return OrientedSize.GetIndirect(size, orientation);
    }

    public static double HalfDirect(OrientedSize orientedSize)
    {
      return orientedSize.Direct / 2.0;
    }

    public static double HalfHeight(Size size)
    {
      return size.Height / 2.0;
    }

    public static double HalfIndirect(OrientedSize orientedSize)
    {
      return orientedSize.Indirect / 2.0;
    }

    public static double HalfWidth(Size size)
    {
      return size.Width / 2.0;
    }

    public static Rect Rect(Size size)
    {
      return new Rect(0, 0, size.Width, size.Height);
    }

    public static Size Round(Size size, int digits = 0)
    {
      var width = Math.Round(size.Width, digits);
      var height = Math.Round(size.Height, digits);

      return new Size(width, height);
    }

	  internal static Size Round(Size size, RoundingMode roundingMode, int digits = 0)
	  {
		  var width = DoubleUtils.Round(size.Width, digits, roundingMode);
		  var height = DoubleUtils.Round(size.Height, digits, roundingMode);

		  return new Size(width, height);
	  }
		
	  internal static Size LayoutRound(Size size, RoundingMode roundingMode)
	  {
		  var width = LayoutUtils.RoundX(size.Width, roundingMode);
		  var height = LayoutUtils.RoundY(size.Height, roundingMode);

		  return new Size(width, height);
	  }

		public static Size Truncate(Size size)
    {
      var width = DoubleUtils.Truncate(size.Width);
      var height = DoubleUtils.Truncate(size.Height);

      return new Size(width, height);
    }

    #endregion
  }
}