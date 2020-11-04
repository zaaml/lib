// <copyright file="ExplicitNotifyCollectionDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class ExplicitNotifyCollectionDispatcher<T> : DelegateNotifyCollectionDispatcher<T>
  {
    #region Ctors

    public ExplicitNotifyCollectionDispatcher(Action<T> onItemAdded, Action<T> onItemRemoved, Action onReset)
      : base(onItemAdded, onItemRemoved, onReset)
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