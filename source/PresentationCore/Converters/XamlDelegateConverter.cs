// <copyright file="XamlDelegateConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  internal class XamlDelegateConverter : BaseValueConverter
  {
    #region Fields

    private readonly IValueConverter _converter;

    #endregion

    #region Ctors

    public XamlDelegateConverter(IValueConverter converter)
    {
      _converter = converter;
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return _converter.ConvertBack(value, targetType, parameter, culture).XamlConvert(targetType);
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return _converter.Convert(value, targetType, parameter, culture).XamlConvert(targetType);
    }

    internal static IValueConverter WrapConverter(IValueConverter valueConverter)
    {
      return valueConverter != null ? new XamlDelegateConverter(valueConverter) : XamlConverter.Instance;
    }

    #endregion
  }
}