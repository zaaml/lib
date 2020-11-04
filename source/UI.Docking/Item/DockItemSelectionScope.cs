// <copyright file="DockItemSelectionScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DockItemSelectionScope
  {
    #region Fields

    private DockItem _selectedItem;
    private int _suspendSelection;
    public event EventHandler SelectedItemChanged;

    #endregion

    #region Properties

    public DockItem SelectedItem
    {
      get => _selectedItem;
      set
      {
        if (_suspendSelection > 0)
          return;

        if (ReferenceEquals(_selectedItem, value))
          return;

        try
        {
          Suspend();

          _selectedItem?.SetIsSelected(false, false);

          _selectedItem = value;

          _selectedItem?.SetIsSelected(true, false);

          SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
          Resume();
        }
      }
    }

    #endregion

    #region  Methods

    public void Resume()
    {
      if (_suspendSelection > 0)
        _suspendSelection--;
    }

    public void Suspend()
    {
      _suspendSelection++;
    }

    #endregion
  }
}