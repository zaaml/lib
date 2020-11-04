// <copyright file="WeakActionExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Weak
{
  internal static class WeakActionExtensions
  {
    #region  Methods

    public static Action ConvertWeak(this Action action)
    {
      return action != null ? WeakAction.ConvertWeak(action) : null;
    }

    public static Action<T> ConvertWeak<T>(this Action<T> action)
    {
      return action != null ? WeakAction.ConvertWeak(action) : null;
    }

    public static Action<T1, T2> ConvertWeak<T1, T2>(this Action<T1, T2> action)
    {
      return action != null ? WeakAction.ConvertWeak(action) : null;
    }

    public static Action<T1, T2, T3> ConvertWeak<T1, T2, T3>(this Action<T1, T2, T3> action)
    {
      return action != null ? WeakAction.ConvertWeak(action) : null;
    }

    public static Action<T1, T2, T3, T4> ConvertWeak<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
    {
      return action != null ? WeakAction.ConvertWeak(action) : null;
    }

    #endregion
  }
}