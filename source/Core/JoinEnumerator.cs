// <copyright file="JoinEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core
{
  internal struct JoinEnumerator<T> : IEnumerator<T>
  {
    #region Fields

    private readonly IEnumerator<T>[] _enumerators;
    private int _currentEnumeratorIndex;

    #endregion

    #region Ctors

    public JoinEnumerator(IEnumerator<T>[] enumerators)
    {
      _currentEnumeratorIndex = -1;
      _enumerators = enumerators;
    }

    #endregion

    #region Properties

    private IEnumerator<T> CurrentEnumerator => _currentEnumeratorIndex >= 0 && _currentEnumeratorIndex < _enumerators.Length ? _enumerators[_currentEnumeratorIndex] : null;

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
      while (_currentEnumeratorIndex < _enumerators.Length)
      {
        _currentEnumeratorIndex++;

        if (CurrentEnumerator?.MoveNext() == true)
          return true;
      }

      return false;
    }

    public void Reset()
    {
      do
      {
        CurrentEnumerator?.Reset();
        _currentEnumeratorIndex--;
      } while (_currentEnumeratorIndex != -1);
    }

    object IEnumerator.Current => Current;

    #endregion

    #region IEnumerator<T>

    public T Current
    {
      get
      {
        var currentEnumerator = CurrentEnumerator;
        return currentEnumerator != null ? currentEnumerator.Current : default(T);
      }
    }

    #endregion

    #endregion
  }

  internal struct JoinEnumerator : IEnumerator
  {
    #region Fields

    private readonly IEnumerator[] _enumerators;
    private int _currentEnumeratorIndex;

    #endregion

    #region Ctors

    public JoinEnumerator(IEnumerator[] enumerators)
    {
      _currentEnumeratorIndex = -1;
      _enumerators = enumerators;
    }

    #endregion

    #region Properties

    public object Current => CurrentEnumerator?.Current;

    private IEnumerator CurrentEnumerator => _currentEnumeratorIndex >= 0 && _currentEnumeratorIndex < _enumerators.Length ? _enumerators[_currentEnumeratorIndex] : null;

    #endregion

    #region Interface Implementations

    #region IEnumerator

    public bool MoveNext()
    {
      while (_currentEnumeratorIndex < _enumerators.Length)
      {
        if (CurrentEnumerator?.MoveNext() == true)
          return true;

        _currentEnumeratorIndex++;
      }

      return false;
    }

    public void Reset()
    {
      while (_currentEnumeratorIndex != -1)
      {
        CurrentEnumerator?.Reset();
        _currentEnumeratorIndex--;
      }
    }

    object IEnumerator.Current => Current;

    #endregion

    #endregion
  }
}