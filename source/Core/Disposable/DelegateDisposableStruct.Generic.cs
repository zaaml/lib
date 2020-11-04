// <copyright file="DelegateDisposableStruct.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Disposable
{
  public struct DelegateDisposableStruct<T> : IDisposable
  {
    public readonly T Value;

    #region Static Fields and Constants

    public static readonly DelegateDisposableStruct<T> Disposed = new DelegateDisposableStruct<T>(default(T), null);

    #endregion

    #region Fields

    private readonly Action<T> _dispose;

    #endregion

    #region Ctors

    public DelegateDisposableStruct(Func<T> factory, Action<T> dispose)
    {
      Value = factory();
      _dispose = dispose;
    }

    public DelegateDisposableStruct(T value, Action<T> dispose)
    {
      Value = value;
      _dispose = dispose;
    }

    public DelegateDisposableStruct(T value, Action<T> init, Action<T> dispose)
    {
      Value = value;
      init(Value);
      _dispose = dispose;
    }

    #endregion

    #region Properties

    public bool IsDisposed => _dispose == null;

    #endregion

    #region  Methods

    [DebuggerStepThrough]
    public static DelegateDisposableStruct<T> Create(T value, Action<T> dispose)
    {
      return new DelegateDisposableStruct<T>(value, dispose);
    }

    [DebuggerStepThrough]
    public static DelegateDisposableStruct<T> Create(T value, Action<T> init, Action<T> dispose)
    {
      return new DelegateDisposableStruct<T>(value, init, dispose);
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct<T> DisposeExchange(DelegateDisposableStruct<T> newDisposable)
    {
      DisposeImpl();
      return newDisposable;
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct<T> DisposeExchange(IDisposable newDisposable)
    {
      DisposeImpl();
      return Create(default(T), t => newDisposable.Dispose());
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct<T> DisposeExchange(Func<IDisposable> disposableFactory)
    {
      DisposeImpl();
      return Create(default(T), t => disposableFactory().Dispose());
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct<T> DisposeExchange(Func<DelegateDisposableStruct<T>> disposableFactory)
    {
      DisposeImpl();
      return disposableFactory();
    }

    [DebuggerStepThrough]
    public DelegateDisposableStruct<T> DisposeExchange()
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
      var copy = this;

      this = Disposed;

      copy._dispose?.Invoke(copy.Value);
    }

    #endregion
  }
}