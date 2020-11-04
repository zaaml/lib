// <copyright file="BaseValueConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
  public abstract class BaseValueConverter : IValueConverter
  {
    #region  Methods

    protected abstract object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture);

    protected abstract object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture);

    #endregion

    #region Interface Implementations

    #region IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertCore(value, targetType, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertBackCore(value, targetType, parameter, culture);
    }

    #endregion

    #endregion
  }
}