// <copyright file="Ensure.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  internal static class Ensure
  {
    #region  Methods

    public static void CollectionRanges(int index, int count, string argument = null)
    {
      if (index < 0 || index >= count)
        throw new ArgumentOutOfRangeException(argument, "Index was out of range. Must be non-negative and less than the size of the collection.");
    }

    public static void NotNull(object obj, string argumentName = null)
    {
      if (obj == null)
        throw new ArgumentNullException(argumentName);
    }

    public static void Ranges(int index, int min, int max, string argument = null)
    {
      if (index < min || index > max)
        throw new ArgumentOutOfRangeException(argument);
    }

    #endregion
  }

  internal static class EnsureExtensions
  {
    #region  Methods

    public static void EnsureCollectionRanges(this int index, int count, string argument = null)
    {
      Ensure.CollectionRanges(index, count, argument);
    }

    public static void EnsureNotNull(this object obj, string argumentName = null)
    {
      Ensure.NotNull(obj, argumentName);
    }

    public static void EnsureRanges(this int index, int min, int max, string argument = null)
    {
      Ensure.Ranges(index, min, max, argument);
    }

    #endregion
  }
}