// <copyright file="CheckGlyph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
  public class CheckGlyph : ToggleGlyphBase
  {
    #region Ctors

    static CheckGlyph()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<CheckGlyph>();
    }

    public CheckGlyph()
    {
      this.OverrideStyleKey<CheckGlyph>();
    }

    #endregion
  }
}