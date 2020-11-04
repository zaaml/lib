//  <copyright file="Monads.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Monads
{
  public static class Monads
  {
    #region Methods

    [DebuggerStepThrough]
    public static TOutput As<TOutput>(this object obj) where TOutput : class
    {
      return obj as TOutput;
    }

    [DebuggerStepThrough]
    public static dynamic AsDynamic(this object obj)
    {
      return obj;
    }

    [DebuggerStepThrough]
    public static TInput? AsNullable<TInput>(this TInput value) where TInput : struct
    {
      return value;
    }

    [DebuggerStepThrough]
    public static TOutput DirectCast<TOutput>(this object obj)
    {
      return (TOutput) obj;
    }

    [DebuggerStepThrough]
    public static TInput Do<TInput>(this TInput obj, Action<TInput> action) where TInput : class
    {
      if (obj != null)
        action(obj);

      return obj;
    }

    [DebuggerStepThrough]
    public static TInput Do<TInput>(this TInput obj, Action action) where TInput : class
    {
      if (obj != null)
        action();

      return obj;
    }

    [DebuggerStepThrough]
    public static bool Do(this bool value, Action action)
    {
      if (value)
        action();

      return value;
    }

    [DebuggerStepThrough]
    public static TInput Do<TInput>(this TInput obj, Action<TInput> action, TInput returnValue) where TInput : class
    {
      if (obj != null)
        action(obj);

      return returnValue;
    }

    [DebuggerStepThrough]
    public static TInput Do<TInput>(this TInput obj, Action action, TInput returnValue) where TInput : class
    {
      if (obj != null)
        action();

      return returnValue;
    }

    [DebuggerStepThrough]
    public static bool Do(this bool value, Action action, bool returnValue)
    {
      if (value)
        action();

      return returnValue;
    }

    [DebuggerStepThrough]
    public static TInput ElseDo<TInput>(this TInput obj, Action action) where TInput : class
    {
      if (obj == null)
        action();

      return null;
    }

    [DebuggerStepThrough]
    public static bool ElseDo(this bool value, Action action)
    {
      if (value == false)
        action();

      return false;
    }

    [DebuggerStepThrough]
    public static TInput If<TInput>(this TInput obj, Predicate<TInput> predicate) where TInput : class
    {
      return obj != null && predicate(obj) ? obj : null;
    }

    [DebuggerStepThrough]
    public static bool IsNull<TInput>(this TInput obj) where TInput : class
    {
      return obj == null;
    }

    [DebuggerStepThrough]
    public static TResult Return<TInput, TResult>(this TInput input, Func<TInput, TResult> eval,
      TResult defaultResult = default(TResult)) where TInput : class
    {
      return input != null ? eval(input) : defaultResult;
    }

    [DebuggerStepThrough]
    public static TResult Return<TInput, TResult>(this TInput input, Func<TInput, TResult> eval, Func<TResult> defaultResultFactory) where TInput : class
    {
      return input != null ? eval(input) : defaultResultFactory();
    }

    [DebuggerStepThrough]
    public static TInput SafeDo<TInput>(this TInput obj, Action<TInput> action, bool breakChainOnFail = true)
      where TInput : class
    {
      try
      {
        if (obj != null)
          action(obj);
      }
      catch (Exception)
      {
        if (breakChainOnFail)
          return null;
      }

      return obj;
    }

    [DebuggerStepThrough]
    public static TInput SafeDo<TInput>(this TInput obj, Action action, bool breakChainOnFail = true)
      where TInput : class
    {
      try
      {
        if (obj != null)
          action();
      }
      catch (Exception)
      {
        if (breakChainOnFail)
          return null;
      }

      return obj;
    }

    [DebuggerStepThrough]
    public static bool SafeDo(this bool value, Action action, bool breakChainOnFail = true)
    {
      try
      {
        if (value)
          action();
      }
      catch (Exception)
      {
        if (breakChainOnFail)
          return false;
      }

      return value;
    }

    [DebuggerStepThrough]
    public static TOutput SafeWith<TInput, TOutput>(this TInput input, Func<TInput, TOutput> eval,
      TOutput exceptionValue = null)
      where TInput : class
      where TOutput : class
    {
      try
      {
        return input != null ? eval(input) : null;
      }
      catch (Exception)
      {
        return exceptionValue;
      }
    }

    [DebuggerStepThrough]
    public static TInput SelfOrCreate<TInput>(this TInput obj, Func<TInput> creator) where TInput : class
    {
      return obj ?? creator();
    }

    [DebuggerStepThrough]
    public static TInput SelfOrDefault<TInput>(this TInput obj, TInput defaultObj) where TInput : class
    {
      return obj ?? defaultObj;
    }

    [DebuggerStepThrough]
    public static TOutput TransformFrom<TInput, TOutput>(this object value, Func<TInput, TOutput> transform) where TInput : class
    {
      return value.As<TInput>().Return(transform);
    }

    [DebuggerStepThrough]
    public static TOutput With<TInput, TOutput>(this TInput input, Func<TInput, TOutput> eval) where TInput : class
      where TOutput : class
    {
      return input != null ? eval(input) : null;
    }

    #endregion
  }
}