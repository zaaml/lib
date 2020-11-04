// <copyright file="CommonUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Utils
{
  internal static class CommonUtils
  {
    #region  Methods

    public static bool IsFlagSet(int value, int mask)
    {
      return 0 != (value & mask);
    }

    public static bool IsFlagSet(uint value, uint mask)
    {
      return 0 != (value & mask);
    }

    public static bool IsFlagSet(long value, long mask)
    {
      return 0 != (value & mask);
    }

    public static bool IsFlagSet(ulong value, ulong mask)
    {
      return 0 != (value & mask);
    }

    public static void Swap<T>(ref T first, ref T second)
    {
      var tmp = first;
      first = second;
      second = tmp;
    }

    #endregion
  }
}