// <copyright file="SkinDictionary.IDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;

namespace Zaaml.PresentationCore.Theming
{
  public partial class SkinDictionary : IDictionary
  {
    #region Properties

    private IDictionary DictionaryInterface => Dictionary;

    #endregion

    #region Interface Implementations

    #region ICollection

    void ICollection.CopyTo(Array array, int index)
    {
      DictionaryInterface.CopyTo(array, index);
    }

    int ICollection.Count => DictionaryInterface.Count;

    object ICollection.SyncRoot => DictionaryInterface.SyncRoot;

    bool ICollection.IsSynchronized => DictionaryInterface.IsSynchronized;

    #endregion

    #region IDictionary

    bool IDictionary.Contains(object key)
    {
      return ContainsKeyCore((string) key);
    }

    void IDictionary.Add(object key, object value)
    {
      AddCore((string) key, value);
    }

    void IDictionary.Clear()
    {
      ClearCore();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return DictionaryInterface.GetEnumerator();
    }

    void IDictionary.Remove(object key)
    {
      RemoveCore((string) key);
    }

    object IDictionary.this[object key]
    {
      get => GetCore((string) key);
      set => SetCore((string) key, value);
    }

    ICollection IDictionary.Keys => DictionaryInterface.Keys;

    ICollection IDictionary.Values => DictionaryInterface.Values;

    bool IDictionary.IsReadOnly => DictionaryInterface.IsReadOnly;

    bool IDictionary.IsFixedSize => DictionaryInterface.IsFixedSize;

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) DictionaryInterface).GetEnumerator();
    }

    #endregion

    #endregion
  }
}