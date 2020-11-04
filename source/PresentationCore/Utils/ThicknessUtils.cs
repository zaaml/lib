// <copyright file="ThicknessUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
#if !SILVERLIGHT
using System;
using Zaaml.Platform;
#endif

namespace Zaaml.PresentationCore.Utils
{
  internal static class ThicknessUtils
  {
    #region  Methods

    public static Thickness Extend(Thickness thickness, Thickness extent)
    {
      return new Thickness(thickness.Left + extent.Left, thickness.Top + extent.Top, thickness.Right + extent.Right, thickness.Bottom + extent.Bottom);
    }

    public static Thickness Extend(Thickness thickness, double uniformExtent)
    {
      return new Thickness(thickness.Left + uniformExtent, thickness.Top + uniformExtent, thickness.Right + uniformExtent, thickness.Bottom + uniformExtent);
    }

    public static bool AreClose(Thickness first, Thickness second)
    {
      return first.Left.IsCloseTo(second.Left) && first.Top.IsCloseTo(second.Top) && first.Right.IsCloseTo(second.Right) && first.Bottom.IsCloseTo(second.Bottom);
    }

    public static Thickness Scale(Thickness thickness, double scale)
    {
      return new Thickness
      {
        Left = thickness.Left* scale,
        Top = thickness.Top* scale,
        Right = thickness.Right* scale,
        Bottom = thickness.Bottom* scale
      };
    }

    #endregion
  }

  internal static class ThicknessExtensions
  {
    #region  Methods

    public static Thickness GetExtended(this Thickness thickness, Thickness extent)
    {
      return ThicknessUtils.Extend(thickness, extent);
    }

    public static Thickness GetExtended(this Thickness thickness, double uniformExtent)
    {
      return ThicknessUtils.Extend(thickness, uniformExtent);
    }

    public static Thickness GetScaled(this Thickness thickness, double scale)
    {
      return ThicknessUtils.Scale(thickness, scale);
    }

    public static bool IsCloseTo(this Thickness thickness, Thickness other)
    {
      return ThicknessUtils.AreClose(thickness, other);
    }

    #endregion

#if !SILVERLIGHT
    public static MARGINS ToPlatformMargins(this Thickness thickness)
    {
      return new MARGINS
      {
        cxLeftWidth = (int) Math.Ceiling(thickness.Left),
        cxRightWidth = (int) Math.Ceiling(thickness.Right),
        cyTopHeight = (int) Math.Ceiling(thickness.Top),
        cyBottomHeight = (int) Math.Ceiling(thickness.Bottom)
      };
    }

    public static Thickness ToPresentationThickness(this MARGINS margins)
    {
      return new Thickness(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
    }
#endif
  }
}