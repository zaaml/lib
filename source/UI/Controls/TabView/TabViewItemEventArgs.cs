//  <copyright file="TabItemEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System;

namespace Zaaml.UI.Controls.TabView
{
  public class TabViewItemEventArgs : EventArgs
  {
    #region Fields

    public readonly TabViewItem TabViewItem;

    #endregion

    #region Ctors

    public TabViewItemEventArgs(TabViewItem tabViewItem)
    {
      TabViewItem = tabViewItem;
    }

    #endregion
  }
}