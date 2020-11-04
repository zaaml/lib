// <copyright file="DockItemCollectionDebugView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DockItemCollectionDebugView
  {
    #region Fields

    private readonly DockItemCollection _dockItemCollection;

    #endregion

    #region Ctors

    public DockItemCollectionDebugView(DockItemCollection dockItemCollection)
    {
      _dockItemCollection = dockItemCollection;
    }

    #endregion

    #region Properties

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public DockItem[] Items
    {
      get
      {
        var windows = new DockItem[_dockItemCollection.Count];
        for (var i = 0; i < _dockItemCollection.Count; i++)
        {
          var window = _dockItemCollection[i];
          windows[i] = window;
        }

        return windows;
      }
    }

    #endregion
  }
}