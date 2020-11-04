// <copyright file="DelegateEqualityComparer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core
{
  internal class DelegateEqualityComparer<T> : IEqualityComparer<T>
  {
    #region Fields

    private readonly Func<T, T, bool> _comparer;
    private readonly Func<T, int> _hashFunc;

    #endregion

    #region Ctors

    public DelegateEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hashFunc)
    {
      comparer.EnsureNotNull(nameof(comparer));
      hashFunc.EnsureNotNull(nameof(hashFunc));

      _comparer = comparer;
      _hashFunc = hashFunc;
    }

    #endregion

    #region Interface Implementations

    #region IEqualityComparer<T>

    public bool Equals(T x, T y)
    {
      return _comparer(x, y);
    }

    public int GetHashCode(T obj)
    {
      return _hashFunc(obj);
    }

    #endregion

    #endregion
  }
}