// <copyright file="SnapSideConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore.Snapping;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class SnapSideConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new SnapSideConverter();

    #endregion

    #region Ctors

    private SnapSideConverter()
    {
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertImpl(value, targetType);
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertImpl(value, targetType);
    }

    private static object ConvertImpl(object value, Type targetType)
    {
      if (value is Dock && targetType == typeof(SnapSide))
        return Snapper.ConvertDock((Dock) value);

      if (value is SnapSide && targetType == typeof(Dock))
        return Snapper.ConvertSnapSide((SnapSide) value);

      return value;
    }

    #endregion
  }
}