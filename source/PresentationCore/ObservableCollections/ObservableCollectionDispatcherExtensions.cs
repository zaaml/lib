// <copyright file="ObservableCollectionDispatcherExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal static class ObservableCollectionDispatcherExtensions
  {
    #region  Methods

    public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(this ObservableCollection<TElement> observableCollection,
      Action<TElement> onItemAdded, Action<TElement> onItemRemoved)
    {
      return ObservableCollectionDispatcher.Dispatch(observableCollection, onItemAdded, onItemRemoved);
    }

#if SILVERLIGHT
    public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(this DependencyObjectCollection<TElement> dependencyObjectCollection,
      Action<TElement> onItemAdded, Action<TElement> onItemRemoved) where TElement : DependencyObject
    {
      return ObservableCollectionDispatcher.Dispatch(dependencyObjectCollection, onItemAdded, onItemRemoved);
    }
#endif

    public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(this DependencyObjectCollectionBase<TElement> dependencyObjectCollection,
      Action<TElement> onItemAdded, Action<TElement> onItemRemoved) where TElement : DependencyObject
    {
      return ObservableCollectionDispatcher.Dispatch(dependencyObjectCollection, onItemAdded, onItemRemoved);
    }

    public static ObservableCollectionBatchDispatcher<TElement> Dispatch<TElement>(this ObservableCollection<TElement> observableCollection,
      Action<IEnumerable<TElement>> onItemsAdded, Action<IEnumerable<TElement>> onItemsRemoved)
    {
      return ObservableCollectionDispatcher.Dispatch(observableCollection, onItemsAdded, onItemsRemoved);
    }

#if SILVERLIGHT
    public static ObservableCollectionBatchDispatcher<TElement> Dispatch<TElement>(this DependencyObjectCollection<TElement> dependencyObjectCollection,
      Action<IEnumerable<TElement>> onItemsAdded, Action<IEnumerable<TElement>> onItemsRemoved) where TElement : DependencyObject
    {
      return ObservableCollectionDispatcher.Dispatch(dependencyObjectCollection, onItemsAdded, onItemsRemoved);
    }
#endif

    public static ObservableCollectionBatchDispatcher<TElement> Dispatch<TElement>(this DependencyObjectCollectionBase<TElement> dependencyObjectCollection,
      Action<IEnumerable<TElement>> onItemsAdded, Action<IEnumerable<TElement>> onItemsRemoved) where TElement : DependencyObject
    {
      return ObservableCollectionDispatcher.Dispatch(dependencyObjectCollection, onItemsAdded, onItemsRemoved);
    }

    #endregion
  }
}