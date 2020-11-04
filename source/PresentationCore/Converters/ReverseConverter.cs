// <copyright file="ReverseConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class ReverseConverter : BaseValueConverter
  {
    #region Properties

    public IValueConverter DirectConverter { get; set; }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DirectConverter.Convert(value, targetType, parameter, culture);
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DirectConverter.ConvertBack(value, targetType, parameter, culture);
    }

    #endregion
  }
}