// <copyright file="XamlConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class XamlConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new XamlConverter();

    #endregion

    #region Ctors

    private XamlConverter()
    {
    }

    #endregion

    #region  Methods

    public static object Convert(object value, Type targetType)
    {
      return value.XamlConvert(targetType);
    }

    public static T Convert<T>(object value)
    {
      return (T) value.XamlConvert(typeof(T));
    }

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Convert(value, targetType);
    }
		
    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Convert(value, targetType);
    }

    public static T ConvertString<T>(string value)
    {
      return (T) value.XamlConvert(typeof(T));
    }

    #endregion
  }
}