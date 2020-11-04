// <copyright file="DelegateDisposable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace Zaaml.Core.Disposable
{
  public sealed class DelegateDisposable : IDisposable
  {
    #region Fields

    private Action _dispose;

    #endregion

    #region Ctors

    public DelegateDisposable(Action dispose)
    {
      _dispose = dispose;
    }

    public DelegateDisposable(Action init, Action dispose)
    {
      init();
      _dispose = dispose;
    }

    #endregion

    #region Properties

    public static IDisposable DummyDisposable
    {
      get { return new DelegateDisposable(() => { }); }
    }

    public bool IsDisposed => _dispose == null;

    #endregion

    #region  Methods

    public static IDisposable Create(Action dispose)
    {
      return new DelegateDisposable(dispose);
    }

    public static IDisposable Create(Action init, Action dispose)
    {
      return new DelegateDisposable(init, dispose);
    }

    public void Dispose()
    {
      Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }

    #endregion
  }
}