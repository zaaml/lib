// <copyright file="ScrollOffsetChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.ScrollView
{
  public sealed class OffsetChangedEventArgs : EventArgs
  {
    #region Ctors

    public OffsetChangedEventArgs(Vector oldOffset, Vector newOffset)
    {
      OldOffset = oldOffset;
      NewOffset = newOffset;
    }

    #endregion

    #region Properties

    public Vector NewOffset { get; }
    public Vector OldOffset { get; }

    #endregion
  }
}