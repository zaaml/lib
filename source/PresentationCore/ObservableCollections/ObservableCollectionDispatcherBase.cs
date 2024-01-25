// <copyright file="ObservableCollectionDispatcherBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.PresentationCore.ObservableCollections
{
	public abstract class ObservableCollectionDispatcherBase<T>
	{
		private readonly Action<T> _onItemAdded;
		private readonly Action<T> _onItemRemoved;
		private protected readonly CachingObservableCollectionDispatchAdapter<T> Adapter;

		protected ObservableCollectionDispatcherBase(ICollection collectionWrapper, Action<T> onItemAdded, Action<T> onItemRemoved)
		{
			Adapter = new CachingObservableCollectionDispatchAdapter<T>(collectionWrapper, OnItemAdded, OnItemRemoved);
			_onItemAdded = onItemAdded;
			_onItemRemoved = onItemRemoved;
		}

		protected void OnItemAdded(T item)
		{
			_onItemAdded(item);
		}

		protected void OnItemRemoved(T item)
		{
			_onItemRemoved(item);
		}
	}
}