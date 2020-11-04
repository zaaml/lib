//  <copyright file="ToolBarButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>


using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ToolBar
{
  public class ToolBarButton : ToolBarButtonBase
  {
    #region Ctors

    static ToolBarButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ToolBarButton>();
    }

    public ToolBarButton()
    {
      this.OverrideStyleKey<ToolBarButton>();
    }

    #endregion
  }
}