// <copyright file="WrapConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
  public abstract class WrapConverter : BaseValueConverter
  {
    #region Fields

    private readonly IValueConverter _converter;

    #endregion

    #region Ctors

    protected WrapConverter(IValueConverter converter)
    {
      _converter = converter;
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return _converter != null ? _converter.ConvertBack(ConvertBackOverride(value, targetType, parameter, culture), targetType, parameter, culture) : ConvertBackOverride(value, targetType, parameter, culture);
    }


    protected abstract object ConvertBackOverride(object value, Type targetType, object parameter, CultureInfo culture);

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return _converter != null ? _converter.Convert(ConvertOverride(value, targetType, parameter, culture), targetType, parameter, culture) : ConvertOverride(value, targetType, parameter, culture);
    }

    protected abstract object ConvertOverride(object value, Type targetType, object parameter, CultureInfo culture);

    #endregion
  }
}