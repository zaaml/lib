// <copyright file="NotifyCollectionEventBatchDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class NotifyCollectionEventBatchDispatcher<T> : DelegateNotifyCollectionBatchDispatcher<T>
  {
    #region Fields

    private IDisposable _disposer;

    #endregion

    #region Ctors

    public NotifyCollectionEventBatchDispatcher(INotifyCollectionChanged notifier, Action<IEnumerable<T>> onItemsAdded, Action<IEnumerable<T>> onItemsRemoved,
      Action onReset)
      : base(onItemsAdded, onItemsRemoved, onReset)
    {
      _disposer = DelegateDisposable.Create(() => notifier.CollectionChanged += OnCollectionChangedHandler, () => notifier.CollectionChanged -= OnCollectionChangedHandler);
    }

    #endregion

    #region  Methods

    public void Dispose()
    {
      _disposer = _disposer.DisposeExchange();
    }

    private void OnCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChangedCore(e);
    }

    #endregion
  }
}