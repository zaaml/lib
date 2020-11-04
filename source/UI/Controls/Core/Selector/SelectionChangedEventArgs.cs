// <copyright file="SelectionChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core
{
  public sealed class SelectionChangedEventArgs<T> : EventArgs
  {
    #region Fields

    public readonly Selection<T> NewSelection;
    public readonly Selection<T> OldSelection;

    #endregion

    #region Ctors

    public SelectionChangedEventArgs(Selection<T> oldSelection, Selection<T> newSelection)
    {
      OldSelection = oldSelection;
      NewSelection = newSelection;
    }

    #endregion
  }
}