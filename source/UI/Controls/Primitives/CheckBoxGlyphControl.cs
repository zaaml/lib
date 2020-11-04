// <copyright file="CheckBoxGlyphControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
  public sealed class CheckBoxGlyphControl : ToggleGlyphControl
  {
    #region Ctors

    static CheckBoxGlyphControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<CheckBoxGlyphControl>();
    }

    public CheckBoxGlyphControl()
    {
      this.OverrideStyleKey<CheckBoxGlyphControl>();
    }

    #endregion
  }
}