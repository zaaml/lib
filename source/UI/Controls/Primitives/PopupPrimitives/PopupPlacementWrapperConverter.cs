// <copyright file="PopupPlacementWrapperConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal class PopupPlacementWrapperConverter : IValueConverter
  {
    #region Ctors

    private PopupPlacementWrapperConverter()
    {
    }

    #endregion

    #region Properties

    public static IValueConverter Instance { get; } = new PopupPlacementWrapperConverter();

    #endregion

    #region Interface Implementations

    #region IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value != null ? new PopupPlacementWrapper((PopupPlacement) value) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

    #endregion
  }
}
