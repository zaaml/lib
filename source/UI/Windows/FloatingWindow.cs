// <copyright file="FloatingWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
  // ReSharper disable once PartialTypeWithSinglePart
  public partial class FloatingWindow : WindowBase
  {
    #region Ctors

    static FloatingWindow()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<FloatingWindow>();
    }

    public FloatingWindow()
    {
      this.OverrideStyleKey<FloatingWindow>();
    }

    #endregion
  }
}