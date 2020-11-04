// <copyright file="StyleResourceDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{
  internal sealed class StyleResourceDictionary : ResourceDictionaryBase
  {
    #region Ctors

    public StyleResourceDictionary(StyleBase style)
    {
      Style = style;
    }

    #endregion

    #region Properties

    public StyleBase Style { get; }

    #endregion
  }
}