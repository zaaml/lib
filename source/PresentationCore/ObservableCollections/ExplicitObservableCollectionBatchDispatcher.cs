// <copyright file="ExplicitObservableCollectionBatchDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class ExplicitObservableCollectionBatchDispatcher<T> : ObservableCollectionBatchDispatcherBase<T>
  {
    #region Fields

    private readonly ExplicitNotifyCollectionBatchDispatcher<T> _dispatcher;

    #endregion

    #region Ctors

    public ExplicitObservableCollectionBatchDispatcher(ICollection sourceCollection, Action<IEnumerable<T>> onItemsAdded, Action<IEnumerable<T>> onItemsRemoved)
      : base(sourceCollection, onItemsAdded, onItemsRemoved)
    {
      _dispatcher = new ExplicitNotifyCollectionBatchDispatcher<T>(Adapter.OnItemsAdded, Adapter.OnItemsRemoved, Adapter.OnReset);
    }

    #endregion

    #region  Methods

    public void DispatchCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      _dispatcher.OnCollectionChanged(e);
    }

    #endregion
  }
}