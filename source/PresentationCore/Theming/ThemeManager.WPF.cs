// <copyright file="ThemeManager.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls.Primitives;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Theming
{
  public static partial class ThemeManager
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached
      ("Theme", typeof(Theme), typeof(ThemeManager), 
	      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure, OnThemePropertyChanged));

    private static readonly DependencyProperty DefaultStyleKeyProperty = DefaultStyleKeyHelper.DefaultStyleKeyProperty;

    #endregion

    #region  Methods

    private static void ActivateGeneric()
    {
      ProcessSources();
    }

    internal static void EnableTheme(FrameworkElement frameworkElement)
    {
      var actualTheme = ApplicationThemeCore;

      if (ActualBehavior.ActivationMode == ThemeManagerActivationMode.Auto && actualTheme != null)
        SetTheme(frameworkElement, actualTheme);
    }

    public static Theme GetTheme(DependencyObject element)
    {
      return (Theme) element.GetValue(ThemeProperty);
    }

    private static void OnThemeChanged(FrameworkElement fre, Theme theme)
    {
      var themeKey = theme?.GetThemeKey(ThemeKey.GetDefaultType(fre));

      if (themeKey != null)
        fre.SetValue(DefaultStyleKeyProperty, themeKey);
    }

    private static void OnThemePropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
    {
	    if (depObj is FrameworkElement fre) 
		    OnThemeChanged(fre, (Theme) args.NewValue);
    }

    static partial void PlatformActivateApplicationTheme()
    {
      if (ReferenceEquals(ActualTheme, DefaultTheme.Instance) == false)
        ActivateGeneric();
    }

    private static void ProcessSources()
    {
      if (IsAutoActivationEnabled == false)
        return;

      var theme = ApplicationThemeCore;

      foreach (var source in PresentationTreeUtils.EnumerateVisualRoots())
      {
        var freSource = source as FrameworkElement;

        if (freSource == null)
          continue;

        var parent = freSource.Parent;

        var themeTarget = parent is Popup || parent is Window ? parent : freSource;

	      EnsureTheme(themeTarget, theme);
      }

	    foreach (var enumeratePopup in PresentationTreeUtils.EnumeratePopups())
		    EnsureTheme(enumeratePopup, theme);
    }

	  private static void EnsureTheme(DependencyObject frameworkElement, Theme theme)
	  {
	    if (frameworkElement.Dispatcher.CheckAccess())
	    {
	      if (ReferenceEquals(GetTheme(frameworkElement), theme) == false)
	        SetTheme(frameworkElement, theme);
      }
	    else
	    {
				// TODO Implement thread-based theme loading.
	      //frameworkElement.Dispatcher.BeginInvoke(() =>
	      //{
	      //  if (ReferenceEquals(GetTheme(frameworkElement), theme) == false)
	      //    SetTheme(frameworkElement, theme);
	      //});
      }
    }

    public static void SetTheme(DependencyObject element, Theme value)
    {
      element.SetValue(ThemeProperty, value);
    }

    #endregion
  }
}