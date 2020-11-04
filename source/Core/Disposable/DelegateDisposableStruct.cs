// <copyright file="DelegateDisposableStruct.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Threading;

namespace Zaaml.Core.Disposable
{
  public struct DelegateDisposableStruct : IDisposable
  {
    #region Static Fields and Constants

    public static readonly DelegateDisposableStruct Disposed = new DelegateDisposableStruct(null);

    #endregion

    #region Fields

    private Action _dispose;

    #endregion

    #region Ctors

    public DelegateDisposableStruct(Action dispose)
    {
      _dispose = dispose;
    }

    public DelegateDisposableStruct(Action init, Action dispose)
    {
      init();
      _dispose = dispose;
    }

    #endregion

    #region Properties

    public bool IsDisposed => _dispose == null;

    #endregion

    #region  Methods

    [DebuggerStepThrough]
    public static DelegateDisposableStruct Create(Action dispose)
    {
      return new DelegateDisposableStruct(dispose);
    }

    [DebuggerStepThrough]
    public static DelegateDisposableStruct Create(Action init, Action dispose)
    {
      return new DelegateDisposableStruct(init, dispose);
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct DisposeExchange(DelegateDisposableStruct newDisposable)
    {
      DisposeImpl();
      return newDisposable;
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct DisposeExchange(IDisposable newDisposable)
    {
      DisposeImpl();
      return Create(newDisposable.Dispose);
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct DisposeExchange(Func<IDisposable> disposableFactory)
    {
      DisposeImpl();
      return Create(disposableFactory().Dispose);
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct DisposeExchange(Func<DelegateDisposableStruct> disposableFactory)
    {
      DisposeImpl();
      return disposableFactory();
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct DisposeExchange()
    {
      DisposeImpl();
      return Disposed;
    }

    [DebuggerStepThrough]
    void IDisposable.Dispose()
    {
      DisposeImpl();
    }

    private void DisposeImpl()
    {
      Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }

    [DebuggerStepThrough]
    public static DelegateDisposableStruct<T> Create<T>(T value, Action<T> dispose)
    {
      return new DelegateDisposableStruct<T>(value, dispose);
    }

    [DebuggerStepThrough]
    public static DelegateDisposableStruct<T> Create<T>(T value, Action<T> init, Action<T> dispose)
    {
      return new DelegateDisposableStruct<T>(value, init, dispose);
    }

    #endregion
  }
}