// <copyright file="ExplicitNotifyCollectionBatchDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class ExplicitNotifyCollectionBatchDispatcher<T> : DelegateNotifyCollectionBatchDispatcher<T>
  {
    #region Ctors

    public ExplicitNotifyCollectionBatchDispatcher(Action<IEnumerable<T>> onItemsAdded, Action<IEnumerable<T>> onItemsRemoved, Action onReset)
      : base(onItemsAdded, onItemsRemoved, onReset)
    {
    }

    #endregion

    #region  Methods

    public void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChangedCore(e);
    }

    #endregion
  }
}