// <copyright file="ObservableCollectionDispatcherExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.PresentationCore.ObservableCollections
{
	public static class ObservableCollectionDispatcherExtensions
	{
		public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(this ObservableCollection<TElement> observableCollection,
			Action<TElement> onItemAdded, Action<TElement> onItemRemoved)
		{
			return ObservableCollectionDispatcher.Dispatch(observableCollection, onItemAdded, onItemRemoved);
		}

		public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(this DependencyObjectCollectionBase<TElement> dependencyObjectCollection,
			Action<TElement> onItemAdded, Action<TElement> onItemRemoved) where TElement : DependencyObject
		{
			return ObservableCollectionDispatcher.Dispatch(dependencyObjectCollection, onItemAdded, onItemRemoved);
		}
	}
}