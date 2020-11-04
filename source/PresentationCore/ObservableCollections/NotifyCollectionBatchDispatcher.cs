// <copyright file="NotifyCollectionBatchDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal abstract class NotifyCollectionBatchDispatcher<T>
  {
    #region  Methods

    protected virtual void OnCollectionChangedCore(NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          OnItemsAdded(e.NewItems.Cast<T>());
          break;
        case NotifyCollectionChangedAction.Remove:
          OnItemsRemoved(e.OldItems.Cast<T>());
          break;
        case NotifyCollectionChangedAction.Replace:
          OnItemsRemoved(e.OldItems.Cast<T>());
          OnItemsAdded(e.NewItems.Cast<T>());
          break;
        case NotifyCollectionChangedAction.Reset:
          OnReset();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    protected abstract void OnItemsAdded(IEnumerable<T> items);
    protected abstract void OnItemsRemoved(IEnumerable<T> items);
    protected abstract void OnReset();

    #endregion
  }
}