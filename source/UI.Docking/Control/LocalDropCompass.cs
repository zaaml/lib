// <copyright file="LocalDropCompass.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
  public class LocalDropCompass : DropCompass
  {
    #region Ctors

    static LocalDropCompass()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<LocalDropCompass>();
    }

    public LocalDropCompass()
    {
      this.OverrideStyleKey<LocalDropCompass>();
    }

    #endregion
  }
}