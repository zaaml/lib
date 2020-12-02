// <copyright file="DefaultStyleKeyHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core;

#if SILVERLIGHT
using System.Windows.Controls;
#endif

namespace Zaaml.PresentationCore.Theming
{
  internal static class DefaultStyleKeyHelper
  {
    #region Static Fields and Constants

    private static readonly HashSet<Type> OverridenTypes = new HashSet<Type>();

    #endregion

    #region Properties

    public static DependencyProperty DefaultStyleKeyProperty => Dummy.Property;

    #endregion

    #region  Methods

    public static Type GetDefaultStyleKeyType(Type type)
    {
#if SILVERLIGHT
      return typeof(Control).IsAssignableFrom(type) ? (DefaultStyleKeyProperty?.GetMetadata(type)?.DefaultValue as Type ?? typeof(FrameworkElement)) : typeof(FrameworkElement);
#else
      var fce = typeof(FrameworkContentElement).IsAssignableFrom(type) ? (DefaultStyleKeyProperty?.GetMetadata(type)?.DefaultValue as Type ?? typeof(FrameworkContentElement)) : null;

      if (fce != null)
        return fce;

      return typeof(FrameworkElement).IsAssignableFrom(type) ? (DefaultStyleKeyProperty?.GetMetadata(type)?.DefaultValue as Type ?? typeof(FrameworkElement)) : typeof(FrameworkElement);
#endif
    }

    public static bool IsItsOwnDefaultStyleKey(Type type)
    {
      return OverridenTypes.Contains(type);
    }

    // ReSharper disable once UnusedTypeParameter
    public static void OverrideStyleKey<T>()
    {
      EnsureThemeManager();

      var type = typeof(T);

      OverridenTypes.Add(type);

#if SILVERLIGHT
#else
      DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
#endif
    }

    private static void EnsureThemeManager()
    {
      ThemeManager.EnsureApplicationTheme();
    }

    public static void OverrideStyleKey(this FrameworkElement element, Type type)
    {
      EnsureThemeManager();
#if SILVERLIGHT
      element.SetValue(DefaultStyleKeyProperty, type);
#else
      if (ThemeManager.IsDefaultThemeApplied)
        element.SetValue(DefaultStyleKeyProperty, (object)DefaultMasterTheme.Instance.GetThemeKey(type) ?? type);
      else
        element.SetValue(DefaultStyleKeyProperty, type);
#endif
    }

    public static void OverrideStyleKey<T>(this FrameworkElement element)
    {
      EnsureThemeManager();

      element.OverrideStyleKey(typeof(T));
    }

    #endregion

    #region  Nested Types

    [UsedImplicitly]
#if SILVERLIGHT
    private class Dummy : Control
#else
    private class Dummy : FrameworkElement
#endif
    {
      #region Properties

      public static DependencyProperty Property => DefaultStyleKeyProperty;

      #endregion
    }

    #endregion
  }
}