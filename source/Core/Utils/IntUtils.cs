// <copyright file="IntUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Utils
{
  public static class IntUtils
  {
    #region  Methods

    public static int Clamp(int value, int min, int max)
    {
      if (min > max)
        throw new ArgumentOutOfRangeException();

      if (value < min)
        return min;
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (value > max)
        return max;

      return value;
    }

    public static int Clamp(int value, Range<int> range)
    {
      return Clamp(value, range.Minimum, range.Maximum);
    }

    public static void UpdateMinMax(ref int min, ref int max, int value)
    {
			if (value < min)
				min = value;

			if (value > max)
				max = value;
    }

    #endregion
  }
}