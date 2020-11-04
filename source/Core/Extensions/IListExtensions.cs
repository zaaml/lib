// <copyright file="IListExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Extensions
{
// ReSharper disable once InconsistentNaming
	internal static class IListExtensions
  {
    #region  Methods

    public static void AddRange(this IList list, IEnumerable values)
    {
      foreach (var value in values)
        list.Add(value);
    }

    public static void RemoveRange(this IList list, IEnumerable values)
    {
      foreach (var value in values)
        list.Remove(value);
    }

    public static int IndexOf<T>(this IList<T> source, T value, int startIndex, int count)
    {
      startIndex.EnsureCollectionRanges(source.Count, "startIndex");
      (startIndex + count).EnsureCollectionRanges(source.Count + 1, "count");

      var equalityComparer = EqualityComparer<T>.Default;
      var num = startIndex + count;

      for (var index = startIndex; index < num; ++index)
      {
        if (equalityComparer.Equals(source[index], value))
          return index;
      }

      return -1;
    }

    public static void Swap<T>(this IList<T> list, int first, int second)
    {
      var tmp = list[first];

      list[first] = list[second];
      list[second] = tmp;
    }

    public static void Swap<T>(this IList<T> list, T first, T second)
    {
      list.Swap(list.IndexOf(first), list.IndexOf(second));
    }

    #endregion
  }
}