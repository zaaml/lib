// <copyright file="FlexUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.UI.Panels.Flexible
{
  internal static class FlexUtils
  {
    #region  Methods

    public static double CalcStarValue(double availableLength, double fixedLength, double starLength)
    {
      if (starLength.IsZero())
        starLength = 1.0;

      return (availableLength - fixedLength).Clamp(0, availableLength) / starLength;
    }

    #endregion
  }
}