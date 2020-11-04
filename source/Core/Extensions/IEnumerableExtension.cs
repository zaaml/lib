// <copyright file="IEnumerableExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Core.Extensions
{
// ReSharper disable once InconsistentNaming
  internal static class IEnumerableExtension
  {
    #region  Methods

    public static IEnumerable<T> WithAction<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var element in enumerable)
      {
        action(element);
        yield return element;
      }
    }

    public static IList<T> AsIList<T>(this IEnumerable<T> enumerable)
    {
      return enumerable as IList<T> ?? enumerable.ToList();
    }

    public static void ExecuteQuery<T>(this IEnumerable<T> enumerable)
    {
      enumerable.GetEnumerator().MoveToEnd();
    }

    #endregion
  }
}