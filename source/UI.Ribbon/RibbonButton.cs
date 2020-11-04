// <copyright file="RibbonButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonButton : RibbonButtonBase
  {
    #region Ctors

    static RibbonButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonButton>();
    }

    public RibbonButton()
    {
      this.OverrideStyleKey<RibbonButton>();
    }

    #endregion
  }
}