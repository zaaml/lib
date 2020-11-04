// <copyright file="DummyDisposable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Disposable
{
  public sealed class DummyDisposable : IDisposable
  {
    #region Static Fields and Constants

    public static IDisposable Instance = new DummyDisposable();

    #endregion

    #region Ctors

    private DummyDisposable()
    {
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
    }

    #endregion

    #endregion
  }
}