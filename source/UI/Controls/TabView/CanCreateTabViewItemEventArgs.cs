//  <copyright file="CanCreateTabItemEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System;

namespace Zaaml.UI.Controls.TabView
{
  public class CanCreateTabViewItemEventArgs : EventArgs
  {
    #region Properties

    public bool CanCreate { get; set; }

    #endregion
  }
}