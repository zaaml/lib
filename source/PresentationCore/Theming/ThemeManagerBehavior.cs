// <copyright file="ThemeManagerStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Theming
{
  public abstract class ThemeManagerBehavior
  {
    #region  Methods

    public ThemeManagerActivationMode ActivationMode { get; set; }

    public abstract bool EnableThemeStyle(Type elementType, Type styleType);

    internal abstract bool ExcludeResource(Uri uri);

    #endregion
  }
}