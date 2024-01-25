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
		private IDisposable _disposer;

		public NotifyCollectionEventDispatcher(INotifyCollectionChanged notifier, Action<T> onItemAdded, Action<T> onItemRemoved, Action onReset)
			: base(onItemAdded, onItemRemoved, onReset)
		{
			_disposer = DelegateDisposable.Create(() => notifier.CollectionChanged += OnCollectionChangedHandler, () => notifier.CollectionChanged -= OnCollectionChangedHandler);
		}

		private void OnCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChangedCore(e);
		}

		public void Dispose()
		{
			_disposer = _disposer.DisposeExchange();
		}
	}
}