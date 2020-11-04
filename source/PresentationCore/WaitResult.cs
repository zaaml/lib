// <copyright file="WaitResult.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace Zaaml.PresentationCore
{
  internal sealed class WaitResult<T>
  {
    #region Fields

    private readonly TimeSpan _timeOut;
    private readonly ManualResetEvent _waitHandle;
    private T _result;

    #endregion

    #region Ctors

    public WaitResult(TimeSpan timeOut)
    {
      _timeOut = timeOut;
      _waitHandle = new ManualResetEvent(false);
    }

    public WaitResult() : this(TimeSpan.MaxValue)
    {
    }

    #endregion

    #region Properties

    public T Result
    {
      get
      {
        if (_waitHandle.WaitOne(_timeOut) == false)
          throw new TimeoutException();

        return _result;
      }
      set
      {
        _result = value;
        _waitHandle.Set();
      }
    }

    #endregion
  }
}