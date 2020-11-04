// <copyright file="ThemeManagerStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{
  public abstract class ThemeManagerStrategy
  {
    #region  Methods

    public abstract bool EnableThemeStyle(ThemeStyle style, bool explicitStyle);

    #endregion
  }
}