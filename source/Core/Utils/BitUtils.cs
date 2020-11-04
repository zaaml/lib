// <copyright file="BitUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Utils
{
  internal static class BitUtils
  {
    #region  Methods

    public static int SignificantBitCount(int value)
    {
      return SignificantBitCount((ulong) value);
    }

    public static int SignificantBitCount(uint value)
    {
      return SignificantBitCount((ulong) value);
    }

    public static int SignificantBitCount(short value)
    {
      return SignificantBitCount((ulong) value);
    }

    public static int SignificantBitCount(ushort value)
    {
      return SignificantBitCount((ulong) value);
    }

    public static int SignificantBitCount(byte value)
    {
      return SignificantBitCount((ulong) value);
    }

    public static int SignificantBitCount(long value)
    {
      return SignificantBitCount((ulong) value);
    }

    public static int SignificantBitCount(ulong value)
    {
      var count = 0;

      while (value > 0)
      {
        count++;
        value >>= 1;
      }

      return count;
    }

    #endregion
  }
}