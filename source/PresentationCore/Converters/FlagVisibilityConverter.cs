// <copyright file="FlagVisibilityConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class FlagVisibilityConverter : FlagConverterBase
  {
    #region Static Fields and Constants

    public static readonly IValueConverter EnabledToVisibleInstance = new FlagVisibilityConverter(true);
    public static readonly IValueConverter EnabledToCollapsedInstance = new FlagVisibilityConverter(false);

    #endregion

    #region Fields

    private readonly bool _enabledToVisible;

    #endregion

    #region Ctors

    private FlagVisibilityConverter(bool enabledToVisible)
    {
      _enabledToVisible = enabledToVisible;
    }

    #endregion

    #region  Methods

    protected override object GetValue(bool value)
    {
      if (value)
        return _enabledToVisible ? Visibility.Visible : Visibility.Collapsed;

      return _enabledToVisible ? Visibility.Collapsed : Visibility.Visible;
    }

    #endregion
  }
}