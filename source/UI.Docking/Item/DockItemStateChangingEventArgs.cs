// <copyright file="DockItemStateChangingEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  public class DockItemStateChangingEventArgs : EventArgs
  {
    #region Fields

    public readonly DockItemState NewDockState;
    public bool CancelValue;

    #endregion

    #region Ctors

    public DockItemStateChangingEventArgs(DockItemState newDockState)
    {
      NewDockState = newDockState;
    }

    #endregion
  }
}