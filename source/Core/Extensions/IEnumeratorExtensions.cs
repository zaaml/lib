// <copyright file="IEnumeratorExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
  // ReSharper disable once InconsistentNaming
  internal static class IEnumeratorExtensions
  {
    #region  Methods

    public static IEnumerable<T> Enumerate<T>(this IEnumerator<T> enumerator)
    {
      return EnumeratorUtils.Enumerate(enumerator);
    }

    public static IEnumerable<object> Enumerate(this IEnumerator enumerator)
    {
      return EnumeratorUtils.Enumerate(enumerator);
    }

    public static void MoveToEnd(this IEnumerator enumerator)
    {
      EnumeratorUtils.MoveToEnd(enumerator);
    }

    public static void Visit<T>(this IEnumerator<T> enumerator, Func<T, bool> visitor)
    {
      EnumeratorUtils.Visit(enumerator, visitor);
    }

    public static void Visit<T>(this IEnumerator<T> enumerator, Action<T> visitor)
    {
      EnumeratorUtils.Visit(enumerator, visitor);
    }

    #endregion
  }
}