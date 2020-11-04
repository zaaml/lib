// <copyright file="RibbonSplitButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonSplitButton : RibbonSplitButtonBase
  {
    #region Ctors

    static RibbonSplitButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonSplitButton>();
    }

    public RibbonSplitButton()
    {
      this.OverrideStyleKey<RibbonSplitButton>();
    }

    #endregion
  }
}