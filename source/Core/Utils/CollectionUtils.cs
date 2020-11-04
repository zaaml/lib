// <copyright file="CollectionUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.Core.Utils
{
  internal static class CollectionUtils
  {
    #region  Methods

    public static bool IsWithinRanges(int index, ICollection collection)
    {
      return index >= 0 && index < collection.Count;
    }

    #endregion
  }
}