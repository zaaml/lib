// <copyright file="DisposableList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Disposable
{
	internal sealed class DisposableList : IDisposable, IEnumerable
  {
    #region Fields

    private readonly List<IDisposable> _disposeList = new List<IDisposable>();

    #endregion

    #region Ctors

    public DisposableList()
    {
    }

    public DisposableList(IEnumerable<IDisposable> disposables)
    {
      _disposeList.AddRange(disposables);
    }

    public DisposableList(params IDisposable[] disposables)
    {
      _disposeList.AddRange(disposables);
    }

    #endregion

    #region  Methods

    public void Add(IDisposable disposable)
    {
      if (_disposeList.Contains(disposable) == false)
        _disposeList.Add(disposable);
    }

    public void Remove(IDisposable disposable)
    {
      _disposeList.Remove(disposable);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      foreach (var disposable in _disposeList)
        disposable.Dispose();

      _disposeList.Clear();
    }

    #endregion

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _disposeList.GetEnumerator();
    }

    #endregion

    #endregion
  }
}