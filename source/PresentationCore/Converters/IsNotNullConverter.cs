// <copyright file="IsNotNullConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class IsNotNullConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new IsNotNullConverter();

    #endregion

    #region Ctors

    private IsNotNullConverter()
    {
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException("IsNotNullConverter can only be used OneWay.");
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value == null ? KnownBoxes.BoolFalse : KnownBoxes.BoolTrue;
    }

    #endregion
  }
}