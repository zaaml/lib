// <copyright file="SelectedItemChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Behaviors.Selectable
{
  internal class SelectedItemChangedEventArgs : EventArgs
  {
    #region Fields

    public readonly object NewItem;
    public readonly object OldItem;

    #endregion

    #region Ctors

    public SelectedItemChangedEventArgs(object newItem, object oldItem)
    {
      NewItem = newItem;
      OldItem = oldItem;
    }

    #endregion
  }

  internal class SelectedItemChangedEventArgs<T> : EventArgs
  {
    #region Fields

    public readonly T NewItem;
    public readonly T OldItem;

    #endregion

    #region Ctors

    public SelectedItemChangedEventArgs(T newItem, T oldItem)
    {
      NewItem = newItem;
      OldItem = oldItem;
    }

    #endregion
  }
}