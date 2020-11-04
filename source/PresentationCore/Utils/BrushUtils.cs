// <copyright file="BrushUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.Utils
{
  public static class BrushUtils
  {
    #region  Methods

    public static SolidColorBrush CloneBrush(SolidColorBrush brush)
    {
      return new SolidColorBrush(brush.Color)
      {
        Opacity = brush.Opacity,
        RelativeTransform = brush.RelativeTransform,
        Transform = brush.Transform
      };
    }

    public static SolidColorBrush ConvertFromColor(Color color)
    {
      return ColorUtils.CreateBrush(color);
    }
		
    public static SolidColorBrush ConvertFromString(string colorString)
    {
      return ConvertFromColor(ColorUtils.ConvertFromString(colorString));
    }

    public static SolidColorBrush ConvertFromString(string colorString, IFormatProvider formatProvider)
    {
      return ConvertFromColor(ColorUtils.ConvertFromString(colorString, formatProvider));
    }

    public static string ConvertToString(SolidColorBrush brush)
    {
      return ColorUtils.ConvertToString(brush.Color);
    }

    public static string ConvertToString(SolidColorBrush brush, IFormatProvider formatProvider)
    {
      return ColorUtils.ConvertToString(brush.Color, formatProvider);
    }

    #endregion
  }
}