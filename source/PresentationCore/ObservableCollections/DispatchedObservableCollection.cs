// <copyright file="DispatchedObservableCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zaaml.Core;

namespace Zaaml.PresentationCore.ObservableCollections
{
  public class DispatchedObservableCollection<T> : ObservableCollection<T>
  {
    #region Fields

    [UsedImplicitly] private readonly ObservableCollectionDispatcher<T> _dispatcher;

    #endregion

    #region Ctors

    public DispatchedObservableCollection()
    {
      _dispatcher = this.Dispatch(OnItemAdded, OnItemRemoved);
    }

    public DispatchedObservableCollection(IEnumerable<T> collection)
      : base(collection)
    {
      _dispatcher = this.Dispatch(OnItemAdded, OnItemRemoved);
    }

    public DispatchedObservableCollection(List<T> list)
      : base(list)
    {
      _dispatcher = this.Dispatch(OnItemAdded, OnItemRemoved);
    }

    #endregion

    #region  Methods

    protected virtual void OnItemAdded(T obj)
    {
    }

    protected virtual void OnItemRemoved(T item)
    {
    }

    #endregion
  }
}