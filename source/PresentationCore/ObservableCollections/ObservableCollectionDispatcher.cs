// <copyright file="ObservableCollectionDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.PresentationCore.ObservableCollections
{
	public class ObservableCollectionDispatcher<T> : ObservableCollectionDispatcherBase<T>, IDisposable
	{
		private NotifyCollectionEventDispatcher<T> _eventDispatcher;

		internal ObservableCollectionDispatcher(ObservableCollectionWrapper<T> collectionWrapper, Action<T> onItemAdded, Action<T> onItemRemoved)
			: base(collectionWrapper, onItemAdded, onItemRemoved)
		{
			_eventDispatcher = new NotifyCollectionEventDispatcher<T>(collectionWrapper, Adapter.OnItemAdded, Adapter.OnItemRemoved, Adapter.OnReset);
		}

		public void Dispose()
		{
			_eventDispatcher.Dispose();
			_eventDispatcher = null;
		}
	}

	public static class ObservableCollectionDispatcher
	{
		public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(ObservableCollection<TElement> observableCollection,
			Action<TElement> onItemAdded,
			Action<TElement> onItemRemoved)
		{
			return new ObservableCollectionDispatcher<TElement>(observableCollection.ToWrapper(), onItemAdded, onItemRemoved);
		}

		public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(DependencyObjectCollectionBase<TElement> observableCollection,
			Action<TElement> onItemAdded,
			Action<TElement> onItemRemoved) where TElement : DependencyObject
		{
			return new ObservableCollectionDispatcher<TElement>(observableCollection.ToWrapper(), onItemAdded, onItemRemoved);
		}
	}
}