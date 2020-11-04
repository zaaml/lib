// <copyright file="LayoutExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class LayoutExtensions
  {
    #region  Methods

    public static double LayoutRound(this double value, Orientation orientation, RoundingMode roundingMode)
    {
      return orientation == Orientation.Horizontal ? LayoutUtils.RoundX(value, roundingMode) : LayoutUtils.RoundY(value, roundingMode);
    }

    public static double LayoutRoundX(this double value, RoundingMode roundingMode)
    {
      return LayoutUtils.RoundX(value, roundingMode);
    }

    public static double LayoutRoundY(this double value, RoundingMode roundingMode)
    {
      return LayoutUtils.RoundY(value, roundingMode);
    }

    #endregion
  }
}