//  <copyright file="CanCloseTabItemEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

namespace Zaaml.UI.Controls.TabView
{
  public class CanCloseTabViewItemEventArgs : TabViewItemEventArgs
  {
    #region Ctors

    public CanCloseTabViewItemEventArgs(TabViewItem tabViewItem)
      : base(tabViewItem)
    {
    }

    #endregion

    #region Properties

    public bool CanClose { get; set; }

    #endregion
  }
}