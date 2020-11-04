// <copyright file="AppWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

#if !SILVERLIGHT

namespace Zaaml.UI.Windows
{
  public class AppWindow : WindowBase
  {
    #region Ctors

    static AppWindow()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<AppWindow>();
    }

    public AppWindow()
    {
      this.OverrideStyleKey<AppWindow>();
    }

    #endregion
  }
}
#endif