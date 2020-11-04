// <copyright file="BoolConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using Zaaml.Core.Utils;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class BoolConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new BoolConverter();

    #endregion

    #region Ctors

    private BoolConverter()
    {
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("BoolConverter can only be used OneWay.");
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return targetType == typeof(bool) ? BoolUtils.ImplicitConvertFrom(value) : value;
    }

    #endregion
  }
}