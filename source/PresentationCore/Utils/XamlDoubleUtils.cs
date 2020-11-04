// <copyright file="XamlDoubleUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Utils;

namespace Zaaml.PresentationCore.Utils
{
  public class XamlDoubleUtils
  {
    #region  Methods

    public static bool AreClose(Size size1, Size size2)
    {
      return DoubleUtils.AreClose(size1.Width, size2.Width) && DoubleUtils.AreClose(size1.Height, size2.Height);
    }

    public static bool AreClose(Vector vector1, Vector vector2)
    {
      return DoubleUtils.AreClose(vector1.X, vector2.X) && DoubleUtils.AreClose(vector1.Y, vector2.Y);
    }

    public static bool AreClose(Rect rect1, Rect rect2)
    {
      if (rect1.IsEmpty)
        return rect2.IsEmpty;

      return !rect2.IsEmpty && DoubleUtils.AreClose(rect1.X, rect2.X) && (DoubleUtils.AreClose(rect1.Y, rect2.Y) && DoubleUtils.AreClose(rect1.Height, rect2.Height)) && DoubleUtils.AreClose(rect1.Width, rect2.Width);
    }

    public static double LayoutRound(double value)
    {
      return Math.Round(value);
    }

    #endregion
  }
}