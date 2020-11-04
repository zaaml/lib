// <copyright file="ObservableCollectionDispatcher.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class ObservableCollectionDispatcher<T> : ObservableCollectionDispatcherBase<T>, IDisposable
  {
    #region Fields

    private NotifyCollectionEventDispatcher<T> _eventDispatcher;

    #endregion

    #region Ctors

    public ObservableCollectionDispatcher(ObservableCollectionWrapper<T> collectionWrapper, Action<T> onItemAdded, Action<T> onItemRemoved)
      : base(collectionWrapper, onItemAdded, onItemRemoved)
    {
      _eventDispatcher = new NotifyCollectionEventDispatcher<T>(collectionWrapper, Adapter.OnItemAdded, Adapter.OnItemRemoved, Adapter.OnReset);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      _eventDispatcher.Dispose();
      _eventDispatcher = null;
    }

    #endregion

    #endregion
  }

  internal static class ObservableCollectionDispatcher
  {
    #region  Methods

    public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(ObservableCollection<TElement> observableCollection,
      Action<TElement> onItemAdded,
      Action<TElement> onItemRemoved)
    {
      return new ObservableCollectionDispatcher<TElement>(observableCollection.ToWrapper(), onItemAdded, onItemRemoved);
    }

#if SILVERLIGHT
    public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(DependencyObjectCollection<TElement> observableCollection,
      Action<TElement> onItemAdded,
      Action<TElement> onItemRemoved) where TElement : DependencyObject
    {
      return new ObservableCollectionDispatcher<TElement>(observableCollection.ToWrapper(), onItemAdded, onItemRemoved);
    }
#endif

    public static ObservableCollectionDispatcher<TElement> Dispatch<TElement>(DependencyObjectCollectionBase<TElement> observableCollection,
      Action<TElement> onItemAdded,
      Action<TElement> onItemRemoved) where TElement : DependencyObject
    {
      return new ObservableCollectionDispatcher<TElement>(observableCollection.ToWrapper(), onItemAdded, onItemRemoved);
    }

    public static ObservableCollectionBatchDispatcher<TElement> Dispatch<TElement>(ObservableCollection<TElement> observableCollection,
      Action<IEnumerable<TElement>> onItemsAdded,
      Action<IEnumerable<TElement>> onItemsRemoved)
    {
      return new ObservableCollectionBatchDispatcher<TElement>(observableCollection.ToWrapper(), onItemsAdded, onItemsRemoved);
    }

#if SILVERLIGHT
    public static ObservableCollectionBatchDispatcher<TElement> Dispatch<TElement>(DependencyObjectCollection<TElement> observableCollection,
      Action<IEnumerable<TElement>> onItemsAdded,
      Action<IEnumerable<TElement>> onItemsRemoved) where TElement : DependencyObject
    {
      return new ObservableCollectionBatchDispatcher<TElement>(observableCollection.ToWrapper(), onItemsAdded, onItemsRemoved);
    }
#endif

    public static ObservableCollectionBatchDispatcher<TElement> Dispatch<TElement>(DependencyObjectCollectionBase<TElement> observableCollection,
      Action<IEnumerable<TElement>> onItemsAdded,
      Action<IEnumerable<TElement>> onItemsRemoved) where TElement : DependencyObject
    {
      return new ObservableCollectionBatchDispatcher<TElement>(observableCollection.ToWrapper(), onItemsAdded, onItemsRemoved);
    }

    #endregion
  }
}