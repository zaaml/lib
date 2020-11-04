// <copyright file="NegateConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class NegateConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly NegateConverter Default = new NegateConverter();

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Convert(value, targetType, parameter, culture);
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;

      if (value is double)
        return Negate((double) value);

      if (value is int)
        return Negate((int) value);

      if (value is bool)
        return Negate((bool) value);

      if (value is long)
        return Negate((long) value);

      if (value is IConvertible)
        return Negate((IConvertible) value, culture);

      if (value is TimeSpan)
        return Negate((TimeSpan) value);

      if (value is Point)
        return Negate((Point) value);

      if (value is Thickness)
        return Negate((Thickness) value);

      throw new ArgumentException("Cannot negate " + value.GetType() + ".", nameof(value));
    }

    private static TimeSpan Negate(TimeSpan value)
    {
      return value.Negate();
    }

    private static Point Negate(Point value)
    {
      return value.Negate();
    }

    private static Thickness Negate(Thickness value)
    {
      return value.Negate();
    }

    private static bool Negate(bool value)
    {
      return !value;
    }

    private static int Negate(int value)
    {
      return -value;
    }

    private static long Negate(long value)
    {
      return -value;
    }

    private static double Negate(double value)
    {
      return -value;
    }

    private static object Negate(IConvertible value, IFormatProvider formatProvider)
    {
      var inputType = value.GetTypeCode();

      var input = value.ToDecimal(formatProvider);
      var output = decimal.Negate(input);

      return System.Convert.ChangeType(output, inputType, formatProvider);
    }

    #endregion
  }
}