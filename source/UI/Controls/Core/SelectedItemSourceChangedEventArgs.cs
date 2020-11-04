// <copyright file="SelectedItemSourceChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core
{
  public sealed class SelectedItemSourceChangedEventArgs : EventArgs
  {
    #region Fields

    public readonly object NewItemSource;
    public readonly object OldItemSource;

    #endregion

    #region Ctors

    public SelectedItemSourceChangedEventArgs(object newValue, object oldValue)
    {
      NewItemSource = newValue;
      OldItemSource = oldValue;
    }

    #endregion
  }
}