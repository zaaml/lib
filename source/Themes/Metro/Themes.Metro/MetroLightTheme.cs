// <copyright file="MetroUILightTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Theming
{
  public sealed class MetroLightTheme : SkinnedTheme
  {
    #region Static Fields and Constants

    private static readonly Lazy<MetroLightTheme> LazyInstance = new Lazy<MetroLightTheme>(() => new MetroLightTheme());

    #endregion

    #region Ctors

    private MetroLightTheme()
    {
    }

    #endregion

    #region Properties

    protected override Theme BaseThemeCore => MetroTheme.Instance;

    protected override string SkinName => "Light";

    public override string Name => "Metro Light";

    public static MetroLightTheme Instance => LazyInstance.Value;

    #endregion
  }
}