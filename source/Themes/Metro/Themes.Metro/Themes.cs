// <copyright file="Themes.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Panels.Core;

// ReSharper disable once CheckNamespace
namespace Zaaml.Theming
{
  public static class Themes
  {
    #region Ctors

    static Themes()
    {
#if !SILVERLIGHT
      RuntimeHelpers.RunClassConstructor(typeof(ControlTemplateRoot).TypeHandle);
#endif
    }

    #endregion

    #region Properties

    public static IEnumerable<Theme> Collection
    {
      get
      {
        yield return MetroDark;
        yield return MetroLight;
        yield return MetroOffice;
      }
    }

    public static Theme MetroDark => MetroDarkTheme.Instance;

    public static Theme MetroLight => MetroLightTheme.Instance;

    public static Theme MetroOffice => MetroOfficeTheme.Instance;

    #endregion
  }

  public enum ThemeKind
  {
    MetroDark,
    MetroLight,
    MetroOffice
  }

  public sealed class ThemesExtension : MarkupExtensionBase
  {
    #region Properties

    public ThemeKind Theme { get; set; } = ThemeKind.MetroOffice;

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      switch (Theme)
      {
        case ThemeKind.MetroDark:
          return MetroDarkTheme.Instance;
        case ThemeKind.MetroLight:
          return MetroLightTheme.Instance;
        case ThemeKind.MetroOffice:
          return MetroOfficeTheme.Instance;
        default:
          return DefaultTheme.Instance;
      }
    }

    #endregion
  }
}