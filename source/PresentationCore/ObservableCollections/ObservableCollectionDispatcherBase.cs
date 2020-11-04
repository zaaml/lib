// <copyright file="ObservableCollectionDispatcherBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal abstract class ObservableCollectionDispatcherBase<T>
  {
    #region Fields

    private readonly Action<T> _onItemAdded;
    private readonly Action<T> _onItemRemoved;
    protected readonly CachingObservableCollectionDispatchAdapter<T> Adapter;

    #endregion

    #region Ctors

    protected ObservableCollectionDispatcherBase(ICollection collectionWrapper, Action<T> onItemAdded, Action<T> onItemRemoved)
    {
      Adapter = new CachingObservableCollectionDispatchAdapter<T>(collectionWrapper, OnItemAdded, OnItemRemoved);
      _onItemAdded = onItemAdded;
      _onItemRemoved = onItemRemoved;
    }

    #endregion

    #region  Methods

    protected void OnItemAdded(T item)
    {
      _onItemAdded(item);
    }

    protected void OnItemRemoved(T item)
    {
      _onItemRemoved(item);
    }

    #endregion
  }
}