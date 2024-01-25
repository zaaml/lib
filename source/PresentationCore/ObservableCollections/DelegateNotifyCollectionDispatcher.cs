// <copyright file="DelegateNotifyCollectionDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.ObservableCollections
{
	internal class DelegateNotifyCollectionDispatcher<T> : NotifyCollectionDispatcher<T>
	{
		private readonly Action<T> _onItemAdded;
		private readonly Action<T> _onItemRemoved;
		private readonly Action _onReset;

		public DelegateNotifyCollectionDispatcher(Action<T> onItemAdded, Action<T> onItemRemoved, Action onReset)
		{
			_onItemAdded = onItemAdded;
			_onItemRemoved = onItemRemoved;
			_onReset = onReset;
		}

		protected override void OnItemAdded(T item)
		{
			_onItemAdded(item);
		}

		protected override void OnItemRemoved(T item)
		{
			_onItemRemoved(item);
		}

		protected override void OnReset()
		{
			_onReset();
		}
	}
}