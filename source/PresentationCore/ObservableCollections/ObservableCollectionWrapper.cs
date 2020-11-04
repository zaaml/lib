// <copyright file="ObservableCollectionWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal class ObservableCollectionWrapper<T> : ICollection, INotifyCollectionChanged
  {
    #region Fields

    private readonly ICollection _collection;

    #endregion

    #region Ctors

    public ObservableCollectionWrapper(ObservableCollection<T> observableCollection)
    {
      _collection = observableCollection;
      observableCollection.CollectionChanged += (sender, args) => OnCollectionChanged(args);
    }

    public ObservableCollectionWrapper(ICollection collection, INotifyCollectionChanged notifier)
    {
      _collection = collection;
      notifier.CollectionChanged += (sender, args) => OnCollectionChanged(args);
    }

    #endregion

    #region  Methods

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      CollectionChanged?.Invoke(this, e);
    }

    #endregion

    #region Interface Implementations

    #region ICollection

    int ICollection.Count => _collection.Count;

    bool ICollection.IsSynchronized => _collection.IsSynchronized;

    object ICollection.SyncRoot => _collection.SyncRoot;

    void ICollection.CopyTo(Array array, int index)
    {
      _collection.CopyTo(array, index);
    }

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _collection.GetEnumerator();
    }

    #endregion

    #region INotifyCollectionChanged

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    #endregion

    #endregion
  }
}