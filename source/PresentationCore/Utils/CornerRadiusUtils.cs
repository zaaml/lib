// <copyright file="CornerRadiusUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Utils
{
  public static class CornerRadiusUtils
  {
    #region  Methods

    public static CornerRadius Coerce(CornerRadius cornerRadius, Rect rect)
    {
      var shortestDimension = Math.Min(rect.Width, rect.Height);
      var halfDimension = shortestDimension/2;

      cornerRadius.TopLeft = Math.Min(cornerRadius.TopLeft, halfDimension);
      cornerRadius.TopRight = Math.Min(cornerRadius.TopRight, halfDimension);
      cornerRadius.BottomLeft = Math.Min(cornerRadius.BottomLeft, halfDimension);
      cornerRadius.BottomRight = Math.Min(cornerRadius.BottomRight, halfDimension);

      return cornerRadius;
    }

    public static bool IsUniform(CornerRadius cornerRadius)
    {
      return cornerRadius.TopLeft.IsCloseTo(cornerRadius.TopRight) &&
             cornerRadius.BottomLeft.IsCloseTo(cornerRadius.BottomRight) &&
             cornerRadius.TopLeft.IsCloseTo(cornerRadius.BottomRight);
    }

    #endregion
  }

  public static class CornerRadiusExtensions
  {
    #region  Methods

    public static CornerRadius Coerce(this CornerRadius cornerRadius, Rect rect)
    {
      return CornerRadiusUtils.Coerce(cornerRadius, rect);
    }

    public static bool IsUniform(this CornerRadius cornerRadius)
    {
      return CornerRadiusUtils.IsUniform(cornerRadius);
    }

    #endregion
  }
}