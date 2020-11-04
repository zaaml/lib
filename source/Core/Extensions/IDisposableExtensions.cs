// <copyright file="IDisposableExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Extensions
{
	// ReSharper disable once InconsistentNaming
	internal static class IDisposableExtensions
  {
    #region  Methods

    [DebuggerStepThrough]
    public static T DisposeExchange<T>(this T disposable, T newDisposable = null) where T : class, IDisposable
    {
      disposable?.Dispose();

      return newDisposable;
    }

    [DebuggerStepThrough]
    internal static T DisposeExchangeStruct<T>(this T disposable) where T : struct, IDisposable
    {
      disposable.Dispose();

      return default;
    }

    [DebuggerStepThrough]
    public static IDisposable DisposeExchange(this IDisposable disposable, IDisposable newDisposable = null)
    {
      disposable?.Dispose();

      return newDisposable;
    }

    [DebuggerStepThrough]
    public static IDisposable DisposeExchange(this IDisposable disposable, Func<IDisposable> disposableFactory)
    {
      disposable?.Dispose();

      return disposableFactory();
    }

    #endregion
  }
}