// <copyright file="CachingObservableCollectionBatchDispatchAdapter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class CachingObservableCollectionBatchDispatchAdapter<T>
  {
    #region Fields

    private readonly List<T> _collectionCache = new List<T>();
    private readonly Action<IEnumerable<T>> _onItemsAdded;
    private readonly Action<IEnumerable<T>> _onItemsRemoved;
    private readonly ICollection _sourceCollection;

    #endregion

    #region Ctors

    public CachingObservableCollectionBatchDispatchAdapter(ICollection sourceCollection, Action<IEnumerable<T>> onItemsAdded,
      Action<IEnumerable<T>> onItemsRemoved)
    {
      _sourceCollection = sourceCollection;
      _onItemsAdded = onItemsAdded;
      _onItemsRemoved = onItemsRemoved;
    }

    #endregion

    #region  Methods

    public void OnItemsAdded(IEnumerable<T> items)
    {
      _collectionCache.AddRange(items);
      _onItemsAdded(items);
    }

    public void OnItemsRemoved(IEnumerable<T> items)
    {
      _collectionCache.RemoveRange(items);
      _onItemsRemoved(items);
    }

    public void OnReset()
    {
      OnItemsRemoved(_collectionCache);

      _collectionCache.Clear();

      OnItemsAdded(_sourceCollection.Cast<T>());
    }

    #endregion
  }
}