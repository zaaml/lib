// <copyright file="GenericResourceDictionary.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Theming
{
  public abstract class GenericResourceDictionary : ResourceDictionary
  {
    #region Ctors

    protected GenericResourceDictionary(Type themeType)
    {
      Theme.RegisterGenericDictionary(themeType, this);
    }

    #endregion
  }

  public abstract class GenericResourceDictionary<TTheme> : GenericResourceDictionary where TTheme : Theme
  {
    #region Ctors

    protected GenericResourceDictionary() : base(typeof(TTheme))
    {
    }

    #endregion
  }
}