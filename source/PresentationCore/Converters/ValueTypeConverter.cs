// <copyright file="ValueTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class ValueTypeConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly ValueTypeConverter Instance = new ValueTypeConverter();

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value?.GetType();
    }

    #endregion
  }
}