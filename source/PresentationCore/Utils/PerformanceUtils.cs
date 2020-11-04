// <copyright file="PerformanceUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Utils
{
  internal static class PerformanceUtils
  {
    #region  Methods

    public static TimeSpan MeasureTime(Action action)
    {
      var start = DateTime.Now;

      action();

      var end = DateTime.Now;

      return end - start;
    }

    #endregion
  }
}