// <copyright file="NotifyCollectionEventDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class NotifyCollectionEventDispatcher<T> : DelegateNotifyCollectionDispatcher<T>, IDisposable
  {
    #region Fields

    private IDisposable _disposer;

    #endregion

    #region Ctors

    public NotifyCollectionEventDispatcher(INotifyCollectionChanged notifier, Action<T> onItemAdded, Action<T> onItemRemoved, Action onReset)
      : base(onItemAdded, onItemRemoved, onReset)
    {
      _disposer = DelegateDisposable.Create(() => notifier.CollectionChanged += OnCollectionChangedHandler, () => notifier.CollectionChanged -= OnCollectionChangedHandler);
    }

    #endregion

    #region  Methods

    private void OnCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChangedCore(e);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      _disposer = _disposer.DisposeExchange();
    }

    #endregion

    #endregion
  }
}