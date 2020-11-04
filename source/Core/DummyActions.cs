// <copyright file="DummyActions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  internal static class DummyAction
  {
    #region Static Fields

    public static readonly Action Instance = () => { };

    #endregion
  }

  internal static class DummyAction<TArg0>
  {
    #region Static Fields

    public static Action<TArg0> Instance = a0 => { };

    #endregion
  }

  internal static class DummyAction<TArg0, TArg1>
  {
    #region Static Fields

    public static Action<TArg0, TArg1> Instance = (a0, a1) => { };

    #endregion
  }

  internal static class DummyAction<TArg0, TArg1, TArg2>
  {
    #region Static Fields

    public static Action<TArg0, TArg1, TArg2> Instance = (a0, a1, a2) => { };

    #endregion
  }

  internal static class DummyAction<TArg0, TArg1, TArg2, TArg3>
  {
    #region Static Fields

    public static Action<TArg0, TArg1, TArg2, TArg3> Instance = (a0, a1, a2, a3) => { };

    #endregion
  }

  internal static class NotSupportedAction
  {
    #region Static Fields

    public static readonly Action Instance = () => { };

    #endregion
  }

  internal static class NotSupportedAction<TArg0>
  {
    #region Static Fields

    public static Action<TArg0> Instance = a0 => Error.NotSupported();

    #endregion
  }

  internal static class NotSupportedAction<TArg0, TArg1>
  {
    #region Static Fields

    public static Action<TArg0, TArg1> Instance = (a0, a1) => Error.NotSupported();

    #endregion
  }

  internal static class NotSupportedAction<TArg0, TArg1, TArg2>
  {
    #region Static Fields

    public static Action<TArg0, TArg1, TArg2> Instance = (a0, a1, a2) => Error.NotSupported();

    #endregion
  }

  internal static class NotSupportedAction<TArg0, TArg1, TArg2, TArg3>
  {
    #region Static Fields

    public static Action<TArg0, TArg1, TArg2, TArg3> Instance = (a0, a1, a2, a3) => Error.NotSupported();

    #endregion
  }
}