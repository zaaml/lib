// <copyright file="DefaultThemeGeneric.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
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