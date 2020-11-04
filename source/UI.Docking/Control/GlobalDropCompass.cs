// <copyright file="GlobalDropCompass.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
  public class GlobalDropCompass : DropCompass
  {
    #region Ctors

    static GlobalDropCompass()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<GlobalDropCompass>();
    }

    public GlobalDropCompass()
    {
      this.OverrideStyleKey<GlobalDropCompass>();
    }

    #endregion
  }
}