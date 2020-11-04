// <copyright file="DropItemEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DropItemEventArgs : EventArgs
  {
    #region Ctors

    public DropItemEventArgs(DockItem sourceDockItem, DockItem targetDockItem, DropGuideAction action)
    {
      SourceDockItem = sourceDockItem;
      TargetDockItem = targetDockItem;
      Action = action;
    }

    #endregion

    #region Properties

    public DropGuideAction Action { get; }

    public DockItem SourceDockItem { get; }

    public DockItem TargetDockItem { get; }

    #endregion
  }
}