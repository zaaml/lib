// <copyright file="ICollectionExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
// ReSharper disable once InconsistentNaming
	internal static class ICollectionExtensions
  {
    #region  Methods

    public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> values)
    {
      foreach (var value in values)
        list.Add(value);
    }

    public static void RemoveRange<T>(this ICollection<T> list, IEnumerable<T> values)
    {
      foreach (var value in values)
        list.Remove(value);
    }

    public static bool IsWithinRanges(this ICollection collection, int index)
    {
	    return CollectionUtils.IsWithinRanges(index, collection);
    }

    #endregion
  }
}