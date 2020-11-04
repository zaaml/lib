// <copyright file="ExplicitObservableCollectionDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class ExplicitObservableCollectionDispatcher<T> : ObservableCollectionDispatcherBase<T>
  {
    #region Fields

    private readonly ExplicitNotifyCollectionDispatcher<T> _dispatcher;

    #endregion

    #region Ctors

    public ExplicitObservableCollectionDispatcher(ICollection sourceCollection, Action<T> onItemAdded, Action<T> onItemRemoved)
      : base(sourceCollection, onItemAdded, onItemRemoved)
    {
      _dispatcher = new ExplicitNotifyCollectionDispatcher<T>(Adapter.OnItemAdded, Adapter.OnItemRemoved, Adapter.OnReset);
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