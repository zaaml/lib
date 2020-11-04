// <copyright file="SkinResourceConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class SkinResourceConverter : IValueConverter
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new SkinResourceConverter();

    #endregion

    #region Ctors

    private SkinResourceConverter()
    {
    }

    #endregion

    #region  Methods

    private object GetSkinValue(object value, object parameter)
    {
      var skin = value as SkinBase;

      if (skin == null)
        return null;

      if (parameter is ThemeResourceKey)
        return skin.GetValueInternal((ThemeResourceKey) parameter);

      var valuePath = parameter as string;

      return valuePath != null ? skin.GetValueInternal(valuePath) : null;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return TargetNullValueConverter.Instance.Convert(GetSkinValue(value, parameter), targetType, parameter, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}