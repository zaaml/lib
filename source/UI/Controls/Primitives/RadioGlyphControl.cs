// <copyright file="RadioGlyphControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
  public sealed class RadioGlyphControl : ToggleGlyphControl
  {
    #region Ctors

    static RadioGlyphControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RadioGlyphControl>();
    }

    public RadioGlyphControl()
    {
      this.OverrideStyleKey<RadioGlyphControl>();
    }

    #endregion
  }
}