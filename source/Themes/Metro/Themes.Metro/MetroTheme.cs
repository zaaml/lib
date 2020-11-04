// <copyright file="MetroUITheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Theming
{
  public sealed class MetroTheme : Theme
  {
    #region Static Fields and Constants

    private static readonly Lazy<MetroTheme> LazyInstance = new Lazy<MetroTheme>(() => new MetroTheme());

    #endregion

    #region Ctors

    private MetroTheme()
    {
    }

    #endregion

    #region Properties

    public static MetroTheme Instance => LazyInstance.Value;

    public override string Name => "Metro";

    #endregion

    #region  Methods

    protected internal override void BindThemeResource(ThemeResourceReference themeResourceReference)
    {
    }

    protected internal override IEnumerable<ThemeResource> EnumerateResources()
    {
      return Enumerable.Empty<ThemeResource>();
    }

    protected internal override ThemeResource GetResource(string key)
    {
      return null;
    }

    #endregion
  }
}