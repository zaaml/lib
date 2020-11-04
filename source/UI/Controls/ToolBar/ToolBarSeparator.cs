//  <copyright file="ToolBarSeparator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>


using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ToolBar
{
  public sealed class ToolBarSeparator : ToolBarItem
  {
    #region Ctors

    static ToolBarSeparator()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ToolBarSeparator>();
    }

    public ToolBarSeparator()
    {
      this.OverrideStyleKey<ToolBarSeparator>();
    }

    #endregion
  }
}