// <copyright file="NotifyCollectionDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal abstract class NotifyCollectionDispatcher<T>
  {
    #region  Methods

    protected virtual void OnCollectionChangedCore(NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (T item in e.NewItems)
            OnItemAdded(item);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (T item in e.OldItems)
            OnItemRemoved(item);
          break;
        case NotifyCollectionChangedAction.Replace:
          foreach (T item in e.OldItems)
            OnItemRemoved(item);
          foreach (T item in e.NewItems)
            OnItemAdded(item);
          break;
        case NotifyCollectionChangedAction.Reset:
          OnReset();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    protected abstract void OnItemAdded(T item);
    protected abstract void OnItemRemoved(T item);
    protected abstract void OnReset();

    #endregion
  }
}