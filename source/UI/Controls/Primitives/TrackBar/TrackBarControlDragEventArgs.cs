// <copyright file="TrackBarControlDragEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
  public class TrackBarControlDragEventArgs : EventArgs
  {
    #region Fields

    public readonly TrackBarItem Item;

    #endregion

    #region Ctors

    internal TrackBarControlDragEventArgs(TrackBarItem item)
    {
      Item = item;
    }

    #endregion
  }
}