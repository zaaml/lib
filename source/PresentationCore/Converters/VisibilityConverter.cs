// <copyright file="VisibilityConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class VisibilityConverter : BaseValueConverter
  {
    private readonly Visibility _falseVisibility;
    private readonly Visibility _trueVisibility;

    private VisibilityConverter(Visibility falseVisibility, Visibility trueVisibility)
    {
      _falseVisibility = falseVisibility;
      _trueVisibility = trueVisibility;
    }

#if !SILVERLIGHT
    public static readonly VisibilityConverter FalseToHiddenVisibility = new VisibilityConverter(Visibility.Hidden, Visibility.Visible);
    public static readonly VisibilityConverter FalseToCollapsedVisibility = new VisibilityConverter(Visibility.Collapsed, Visibility.Visible);
    public static readonly VisibilityConverter TrueToCollapsedVisibility = new VisibilityConverter(Visibility.Visible, Visibility.Collapsed);
#else
    public static readonly VisibilityConverter FalseToCollapsedVisibility = new VisibilityConverter(Visibility.Collapsed, Visibility.Visible);
    public static readonly VisibilityConverter TrueToCollapsedVisibility = new VisibilityConverter(Visibility.Visible, Visibility.Collapsed);
#endif

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || value is bool boolValue && boolValue == false)
        return _falseVisibility;

      return _trueVisibility;
    }

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (Visibility) value != _falseVisibility;
    }
  }
}