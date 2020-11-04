// <copyright file="DockItemIndexProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DockItemIndexProvider
  {
    #region Fields

    private int _index;

    #endregion

    #region Properties

    public int NewIndex => _index++;

    #endregion

    #region  Methods

    internal void OnDockItemIndexChanged(int? newVal)
    {
      SyncDockItemIndex(newVal);
    }

    internal void OnDockItemIndexChanged(DependencyObject d, int? newVal)
    {
      SyncDockItemIndex(newVal);
    }

    internal void SyncDockItemIndex(int? newVal)
    {
      if (newVal != null && newVal >= _index)
        _index = newVal.Value + 1;
    }

    #endregion
  }
}