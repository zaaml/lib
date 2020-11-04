// <copyright file="DragItemEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DragItemEventArgs : EventArgs
  {
    #region Ctors

    public DragItemEventArgs(DockItem dockItem)
    {
      DockItem = dockItem;
    }

    #endregion

    #region Properties

    public DockItem DockItem { get; }

    #endregion
  }
}