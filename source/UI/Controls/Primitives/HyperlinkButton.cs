// <copyright file="HyperlinkButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
  public class HyperlinkButton : Button
  {
    #region Ctors

    static HyperlinkButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<HyperlinkButton>();
    }

    public HyperlinkButton()
    {
      this.OverrideStyleKey<HyperlinkButton>();
    }

    #endregion
  }
}