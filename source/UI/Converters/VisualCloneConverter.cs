// <copyright file="VisualCloneConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using Zaaml.PresentationCore.Converters;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Converters
{
  internal sealed class VisualCloneConverter : BaseValueConverter
  {
    #region Ctors

    private VisualCloneConverter()
    {
    }

    #endregion

    #region Properties

    public static VisualCloneConverter Instance { get; } = new VisualCloneConverter();

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var uie = value as FrameworkElement;
      if (uie == null)
        return value;

      var rectangle = new VisualCloneControl
      {
        Source = uie
      };

      return rectangle;
    }

    #endregion
  }
}