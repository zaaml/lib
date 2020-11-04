// <copyright file="RoundingConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class RoundingConverter : BaseValueConverter
  {
    #region Properties

    public int Digits { get; set; }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var roundingMode = RoundingMode.MidPointFromZero;

      if (value is double)
      {
        var dblValue = (double) value;

        return DoubleUtils.Round(dblValue, Digits, roundingMode);
      }

      if (value is Point)
      {
        var ptValue = (Point) value;

        return PointUtils.Round(ptValue, roundingMode, Digits);
      }

      if (value is Size)
      {
        var szValue = (Size) value;

        return SizeUtils.Round(szValue, roundingMode, Digits);
      }

      if (value is Rect)
      {
        var rcValue = (Rect) value;

        return RectUtils.Round(rcValue, roundingMode, Digits);
      }

      return value;
    }

    #endregion
  }
}