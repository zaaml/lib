// <copyright file="ObservableCollectionProxy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.ObservableCollections
{
  public class ObservableCollectionProxy<T> : IReadOnlyCollection<T>, IDisposable
  {
    #region Fields

    private readonly List<T> _innerCollection = new List<T>();
    private readonly Action<T> _onItemAdded;
    private readonly Action<T> _onItemRemoved;
    private IDisposable _dispatcher;

    #endregion

    #region Ctors

    public ObservableCollectionProxy(ObservableCollection<T> observableCollection, Action<T> onItemAdded, Action<T> onItemRemoved)
    {
      _onItemAdded = onItemAdded;
      _onItemRemoved = onItemRemoved;
      _dispatcher = new ObservableCollectionDispatcher<T>(observableCollection.ToWrapper(), OnItemAdded, OnItemRemoved);
    }

    #endregion

    #region  Methods

    private void OnItemAdded(T item)
    {
      _innerCollection.Add(item);
      _onItemAdded(item);
    }

    private void OnItemRemoved(T item)
    {
      _innerCollection.Remove(item);
      _onItemRemoved(item);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      _dispatcher = _dispatcher.DisposeExchange();
    }

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion

    #region IEnumerable<T>

    public IEnumerator<T> GetEnumerator()
    {
      return _innerCollection.GetEnumerator();
    }

    #endregion

    #region IReadOnlyCollection<T>

    public T this[int index] => _innerCollection[index];

    public int Count => _innerCollection.Count;

    #endregion

    #endregion
  }
}