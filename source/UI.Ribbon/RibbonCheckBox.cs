// <copyright file="RibbonCheckBox.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonCheckBox : RibbonToggleButton
  {
    #region Ctors

    static RibbonCheckBox()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonCheckBox>();
    }

    public RibbonCheckBox()
    {
      this.OverrideStyleKey<RibbonCheckBox>();
    }

    #endregion
  }
}