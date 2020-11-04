// <copyright file="DockItemStateChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  public class DockItemStateChangedEventArgs : EventArgs
  {
    #region Fields

    public readonly DockItemState OldDockState;

    #endregion

    #region Ctors

    public DockItemStateChangedEventArgs(DockItemState oldDockState)
    {
      OldDockState = oldDockState;
    }

    #endregion
  }
}