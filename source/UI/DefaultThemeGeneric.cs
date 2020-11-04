// <copyright file="DefaultThemeGeneric.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using DefaultMasterTheme = Zaaml.PresentationCore.Theming.DefaultMasterTheme;

// ReSharper disable once CheckNamespace
namespace Zaaml.Theming
{
  public sealed class DefaultThemeGeneric : GenericResourceDictionary
  {
    #region Ctors

    public DefaultThemeGeneric() : base(typeof(DefaultMasterTheme))
    {
    }

    #endregion
  }
}