// <copyright file="DefaultMasterTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Zaaml.PresentationCore.Theming
{
  internal class DefaultMasterTheme : Theme
  {
    #region Ctors

    private DefaultMasterTheme()
    {
    }

    #endregion

    #region Properties

    public static DefaultMasterTheme Instance { get; } = new DefaultMasterTheme();

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

    protected override void RegisterGenericDictionaryCore(GenericResourceDictionary genericDictionary)
    {
      base.RegisterGenericDictionaryCore(genericDictionary);

      genericDictionary.MergedDictionaries.Add(CreateThemeResourceDictionary());
    }
  }

  internal sealed class DefaultTheme : SkinnedTheme
  {
    #region Static Fields and Constants

    private static readonly Lazy<DefaultTheme> LazyInstance = new Lazy<DefaultTheme>(() => new DefaultTheme());

    #endregion

    #region Ctors

    static DefaultTheme()
    {
      RuntimeHelpers.RunClassConstructor(typeof(DefaultMasterTheme).TypeHandle);
    }

    private DefaultTheme()
    {
    }

    #endregion

    #region Properties

    protected override Theme BaseThemeCore => DefaultMasterTheme.Instance;

    public static DefaultTheme Instance => LazyInstance.Value;

    public override string Name => "Metro Office";

    protected override string SkinName => "Office";

    #endregion
  }
}