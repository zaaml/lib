// <copyright file="MetroUIDarkTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Theming
{
  public sealed class MetroDarkTheme : SkinnedTheme
  {
    #region Static Fields and Constants

    private static readonly Lazy<MetroDarkTheme> LazyInstance = new Lazy<MetroDarkTheme>(() => new MetroDarkTheme());

    #endregion

    #region Ctors

    private MetroDarkTheme()
    {
    }

    #endregion

    #region Properties

    protected override Theme BaseThemeCore => MetroTheme.Instance;

    protected override string SkinName => "Dark";

    public override string Name => "Metro Dark";

    public static MetroDarkTheme Instance => LazyInstance.Value;

    #endregion
  }
}