// <copyright file="MetroUIOfficeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Theming
{
  public sealed class MetroOfficeTheme : SkinnedTheme
  {
    #region Static Fields and Constants

    private static readonly Lazy<MetroOfficeTheme> LazyInstance = new Lazy<MetroOfficeTheme>(() => new MetroOfficeTheme());

    #endregion

    #region Ctors

    private MetroOfficeTheme()
    {
    }

    #endregion

    #region Properties

    protected override Theme BaseThemeCore => MetroTheme.Instance;

    public static MetroOfficeTheme Instance => LazyInstance.Value;

    public override string Name => "Metro Office";

    protected override string SkinName => "Office";

    #endregion
  }
}