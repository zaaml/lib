// <copyright file="ThemeManagerResourceDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{
  public sealed class ThemeManagerResourceDictionary : ResourceDictionaryBase
  {
    #region Ctors

    public ThemeManagerResourceDictionary()
    {
      ThemeStyleBinder.Instance.AttachResourceDictionary(this);
    }

    #endregion
  }
}