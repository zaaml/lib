// <copyright file="SingleItemEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core
{
  internal sealed class SingleItemEnumerator<T> : IEnumerator<T>
  {
    #region Fields

    private readonly T _item;
    private int _index = -1;

    #endregion

    #region Ctors

    public SingleItemEnumerator(T item)
    {
      _item = item;
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    void IDisposable.Dispose()
    {
    }

    #endregion

    #region IEnumerator

    public bool MoveNext()
    {
      if (_index != -1) return false;

      _index = 0;

      return true;
    }

    public void Reset()
    {
      _index = -1;
    }

    object IEnumerator.Current => Current;

    #endregion

    #region IEnumerator<T>

    public T Current => _index == 0 ? _item : default(T);

    #endregion

    #endregion
  }

  internal sealed class SingleItemEnumerator : IEnumerator
  {
    #region Fields

    private readonly object _item;
    private int _index = -1;

    #endregion

    #region Ctors

    public SingleItemEnumerator(object item)
    {
      _item = item;
    }

    #endregion

    #region Properties

    public object Current => _index == 0 ? _item : null;

    #endregion

    #region Interface Implementations

    #region IEnumerator

    public bool MoveNext()
    {
      if (_index != -1) return false;

      _index = 0;

      return true;
    }

    public void Reset()
    {
      _index = -1;
    }

    object IEnumerator.Current => Current;

    #endregion

    #endregion
  }
}