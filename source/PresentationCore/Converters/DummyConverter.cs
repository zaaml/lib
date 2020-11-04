// <copyright file="DummyConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class DummyConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new DummyConverter();

    #endregion

    #region Ctors

    private DummyConverter()
    {
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }

    #endregion
  }
}