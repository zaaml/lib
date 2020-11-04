// <copyright file="DelegateNotifyCollectionBatchDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class DelegateNotifyCollectionBatchDispatcher<T> : NotifyCollectionBatchDispatcher<T>
  {
    #region Fields

    private readonly Action<IEnumerable<T>> _onItemsAdded;
    private readonly Action<IEnumerable<T>> _onItemsRemoved;
    private readonly Action _onReset;

    #endregion

    #region Ctors

    public DelegateNotifyCollectionBatchDispatcher(Action<IEnumerable<T>> onItemsAdded, Action<IEnumerable<T>> onItemsRemoved, Action onReset)
    {
      _onItemsAdded = onItemsAdded;
      _onItemsRemoved = onItemsRemoved;
      _onReset = onReset;
    }

    #endregion

    #region  Methods

    protected override void OnItemsAdded(IEnumerable<T> item)
    {
      _onItemsAdded(item);
    }

    protected override void OnItemsRemoved(IEnumerable<T> item)
    {
      _onItemsRemoved(item);
    }

    protected override void OnReset()
    {
      _onReset();
    }

    #endregion
  }
}