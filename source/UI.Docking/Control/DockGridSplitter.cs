// <copyright file="DockGridSplitter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
  public class DockGridSplitter : GridSplitter
  {
    #region Ctors

    static DockGridSplitter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DockGridSplitter>();
    }

    public DockGridSplitter()
    {
      this.OverrideStyleKey<DockGridSplitter>();
    }

    #endregion
  }
}