// <copyright file="CachingObservableCollectionDispatchAdapter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class CachingObservableCollectionDispatchAdapter<T>
  {
    #region Fields

    private readonly List<T> _collectionCache = new List<T>();
    private readonly Action<T> _onItemAdded;
    private readonly Action<T> _onItemRemoved;
    private readonly ICollection _sourceCollection;

    #endregion

    #region Ctors

    public CachingObservableCollectionDispatchAdapter(ICollection sourceCollection, Action<T> onItemAdded, Action<T> onItemRemoved)
    {
      _sourceCollection = sourceCollection;
      _onItemAdded = onItemAdded;
      _onItemRemoved = onItemRemoved;
    }

    #endregion

    #region  Methods

    public void OnItemAdded(T item)
    {
      _collectionCache.Add(item);
      _onItemAdded(item);
    }

    public void OnItemRemoved(T item)
    {
      _collectionCache.Remove(item);
      _onItemRemoved(item);
    }

    public void OnReset()
    {
      foreach (var item in _collectionCache)
        _onItemRemoved(item);

      _collectionCache.Clear();

      foreach (var item in _sourceCollection.Cast<T>())
        OnItemAdded(item);
    }

    #endregion
  }
}