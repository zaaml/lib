// <copyright file="DummyList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Collections
{
  internal class DummyList : IList
  {
    #region Ctors

    private DummyList()
    {
    }

    #endregion

    #region Properties

    public static DummyList Instance { get; } = new DummyList();

    #endregion

    #region Interface Implementations

    #region ICollection

    public void CopyTo(Array array, int index)
    {
    }

    public int Count => 0;

    public object SyncRoot { get; } = new object();

    public bool IsSynchronized => false;

    #endregion

    #region IEnumerable

    public IEnumerator GetEnumerator()
    {
      return EnumeratorUtils.EmptyEnumerator;
    }

    #endregion

    #region IList

    public int Add(object value) => -1;

    public bool Contains(object value) => false;

    public void Clear()
    {
    }

    public int IndexOf(object value) => -1;

    public void Insert(int index, object value)
    {
    }

    public void Remove(object value)
    {
    }

    public void RemoveAt(int index)
    {
    }

    public object this[int index]
    {
      get => throw new ArgumentException(nameof(index));
      set => throw new ArgumentException(nameof(index));
    }

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    #endregion

    #endregion
  }
}