// <copyright file="DefaultThemeManagerBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Theming
{
  public class DefaultThemeManagerBehavior : ThemeManagerBehavior
  {
    #region Static Fields and Constants

    public static readonly ThemeManagerBehavior Instance = new DefaultThemeManagerBehavior();

    #endregion

    #region Ctors

    public DefaultThemeManagerBehavior()
    {
      ActivationMode = ThemeManagerActivationMode.Auto;
    }

    #endregion

    #region  Methods

    public override bool EnableThemeStyle(Type elementType, Type styleType)
    {
      return elementType == styleType;
    }

    internal override bool ExcludeResource(Uri uri)
    {
      return false;
    }

    #endregion
  }
}