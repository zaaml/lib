// <copyright file="RadioGlyph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
  public class RadioGlyph : ToggleGlyphBase
  {
    #region Ctors

    static RadioGlyph()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RadioGlyph>();
    }

    public RadioGlyph()
    {
      this.OverrideStyleKey<RadioGlyph>();
    }

    #endregion
  }
}