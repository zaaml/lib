// <copyright file="TargetNullValueConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class TargetNullValueConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly TargetNullValueConverter Instance = new TargetNullValueConverter();

    #endregion

    #region Ctors

    private TargetNullValueConverter()
    {
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return targetType.IsValueType && (value == null || value.IsDependencyPropertyUnsetValue() || value.IsUnset()) ? RuntimeUtils.CreateDefaultValue(targetType) : value;
    }

    #endregion
  }
}