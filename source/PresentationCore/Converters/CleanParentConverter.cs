// <copyright file="CleanParentConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  internal sealed class CleanParentConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new CleanParentConverter();

    #endregion

    #region Ctors

    private CleanParentConverter()
    {
    }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var freValue = value as FrameworkElement;
      if (freValue == null)
        return value;

      freValue.DetachFromLogicalParent();

      return value;
    }

    #endregion
  }
}