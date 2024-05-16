// <copyright file="SearchResultCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Editors.Text
{
  public class SearchResultCollection : ICollection, INotifyCollectionChanged
  {
    #region Fields

    private readonly List<object> _innerCollection = new List<object>();

    #endregion

    #region  Methods

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      CollectionChanged?.Invoke(this, e);
    }

    internal void SetResult(IEnumerable<object> searchResult)
    {
      _innerCollection.Clear();
      _innerCollection.AddRange(searchResult);

      OnCollectionChanged(Constants.NotifyCollectionChangedReset);
    }

    #endregion

    #region Interface Implementations

    #region ICollection

    public int Count => _innerCollection.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => null;

    public void CopyTo(Array array, int index)
    {
      throw new NotSupportedException();
    }

    #endregion

    #region IEnumerable

    public IEnumerator GetEnumerator()
    {
      return _innerCollection.GetEnumerator();
    }

    #endregion

    #region INotifyCollectionChanged

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    #endregion

    #endregion
  }
}