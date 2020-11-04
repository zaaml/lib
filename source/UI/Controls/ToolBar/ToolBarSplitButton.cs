// <copyright file="ToolBarSplitButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ToolBar
{
  public class ToolBarSplitButton : ToolBarSplitButtonBase
  {
    #region Ctors

    static ToolBarSplitButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ToolBarSplitButton>();
    }

    public ToolBarSplitButton()
    {
      this.OverrideStyleKey<ToolBarSplitButton>();
    }

    #endregion
  }
}