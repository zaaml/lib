// <copyright file="ObservableCollectionBatchDispatcherBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal abstract class ObservableCollectionBatchDispatcherBase<T>
  {
    #region Fields

    private readonly Action<IEnumerable<T>> _onItemsAdded;
    private readonly Action<IEnumerable<T>> _onItemsRemoved;
    protected readonly CachingObservableCollectionBatchDispatchAdapter<T> Adapter;

    #endregion

    #region Ctors

    protected ObservableCollectionBatchDispatcherBase(ICollection collectionWrapper, Action<IEnumerable<T>> onItemsAdded, Action<IEnumerable<T>> onItemsRemoved)
    {
      Adapter = new CachingObservableCollectionBatchDispatchAdapter<T>(collectionWrapper, OnItemsAdded, OnItemsRemoved);
      _onItemsAdded = onItemsAdded;
      _onItemsRemoved = onItemsRemoved;
    }

    #endregion

    #region  Methods

    protected void OnItemsAdded(IEnumerable<T> items)
    {
      _onItemsAdded(items);
    }

    protected void OnItemsRemoved(IEnumerable<T> items)
    {
      _onItemsRemoved(items);
    }

    #endregion
  }
}