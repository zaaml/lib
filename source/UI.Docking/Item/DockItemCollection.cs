// <copyright file="DockItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Docking
{
  [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
  [DebuggerTypeProxy(typeof(DockItemCollectionDebugView))]
  public class DockItemCollection : DependencyObjectCollectionBase<DockItem>
  {
    #region Fields

    private readonly Dictionary<string, DockItem> _dictionary = new Dictionary<string, DockItem>(StringComparer.Ordinal);
    private readonly bool _locking;
    private readonly Action<DockItem> _onItemAdded;
    private readonly Action<DockItem> _onItemRemoved;

    #endregion

    #region Ctors

    internal DockItemCollection(Action<DockItem> onItemAdded, Action<DockItem> onItemRemoved, bool locking = false)
    {
      _onItemAdded = onItemAdded;
      _onItemRemoved = onItemRemoved;
      _locking = locking;
    }

    #endregion

    #region Properties

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal string DebuggerDisplay
    {
      get { return $"[{string.Join(",", this.Select(w => w.DebuggerDisplay))}]"; }
    }

    internal DockItem this[string name] => name != null ? _dictionary[name] : null;

    #endregion

    #region  Methods

    protected override void OnItemAdded(DockItem item)
    {
      base.OnItemAdded(item);

      VerifyName(item);

      if (item.ItemCollections.Add(this) == false)
        throw new InvalidOperationException();

      if (string.IsNullOrEmpty(item.Name) == false)
        _dictionary[item.Name] = item;

      _onItemAdded(item);

      if (_locking)
        item.Lock();
    }

    internal void OnItemNameChanged(DockItem dockItem, string prevName)
    {
      if (prevName != null)
        _dictionary.Remove(prevName);

      if (dockItem.Name != null)
        _dictionary[dockItem.Name] = dockItem;
    }

    protected override void OnItemRemoved(DockItem item)
    {
      base.OnItemRemoved(item);

      if (item.Name != null)
        _dictionary.Remove(item.Name);

      if (item.ItemCollections.Remove(this) == false)
        throw new InvalidOperationException();

      _onItemRemoved.Invoke(item);

      if (_locking)
        item.Unlock();
    }

    internal void Replace(DockItem oldItem, DockItem newItem)
    {
      var index = IndexOf(oldItem);

      if (index == -1)
        return;

      this[index] = newItem;
    }

    internal bool TryGetDockItem(string key, out DockItem value)
    {
      if (key != null)
        return _dictionary.TryGetValue(key, out value);

      value = null;

      return false;
    }

    private void VerifyName(DockItem dockItem)
    {
      if (string.IsNullOrEmpty(dockItem.Name))
        return;

      if (_dictionary.ContainsKey(dockItem.Name))
        throw new InvalidOperationException("DockItem with the same Name already exists in this collection");
    }

    #endregion
  }
}