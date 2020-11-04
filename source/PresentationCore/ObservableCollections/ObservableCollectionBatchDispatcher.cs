// <copyright file="ObservableCollectionBatchDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class ObservableCollectionBatchDispatcher<T> : ObservableCollectionBatchDispatcherBase<T>, IDisposable
  {
    #region Fields

    private NotifyCollectionEventBatchDispatcher<T> _eventDispatcher;

    #endregion

    #region Ctors

    public ObservableCollectionBatchDispatcher(ObservableCollectionWrapper<T> collectionWrapper, Action<IEnumerable<T>> onItemsAdded,
      Action<IEnumerable<T>> onItemsRemoved)
      : base(collectionWrapper, onItemsAdded, onItemsRemoved)
    {
      _eventDispatcher = new NotifyCollectionEventBatchDispatcher<T>(collectionWrapper, Adapter.OnItemsAdded, Adapter.OnItemsRemoved, Adapter.OnReset);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      _eventDispatcher.Dispose();
      _eventDispatcher = null;
    }

    #endregion

    #endregion
  }
}