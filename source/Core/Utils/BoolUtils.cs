// <copyright file="BoolUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Utils
{
  internal static class BoolUtils
  {
    #region  Methods

    public static bool ImplicitConvertFrom(object value)
    {
      if (value == null)
        return false;

      if (value is bool)
        return (bool) value;

      if (value is int)
        return (int) value != 0;

      if (value is uint)
        return (uint) value != 0;

      if (value is long)
        return (long) value != 0;

      if (value is ulong)
        return (ulong) value != 0;

      if (value is short)
        return (short) value != 0;

      if (value is ushort)
        return (ushort) value != 0;

      if (value is sbyte)
        return (sbyte) value != 0;

      if (value is byte)
        return (byte) value != 0;

      if (value is float)
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return (float) value != .0f;

      if (value is double)
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return (double) value != .0;

      if (value is string)
        return ((string) value).IsNullOrEmpty() == false;

      return true;
    }

    #endregion
  }
}