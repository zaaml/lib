// <copyright file="FloatingWindowExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Windows
{
  internal static class FloatingWindowExtensions
  {
    #region  Methods

    public static Rect GetLayoutBox(this WindowBase window)
    {
      return new Rect(window.GetLocation(), window.GetSize());
    }

    public static Point GetLocation(this WindowBase window)
    {
      return new Point(window.Left, window.Top).Round();
    }

    public static Size GetSize(this WindowBase window)
    {
      return window.GetCurrentSize().Round();
    }

    public static void SetLayoutBox(this WindowBase window, Rect layoutRect)
    {
      window.SetLocation(layoutRect.GetTopLeft());
      window.SetSize(layoutRect.Size());
    }

    public static void SetLocation(this WindowBase window, Point point)
    {
      point = point.Round();

      window.Left = point.X;
      window.Top = point.Y;
    }

    public static void SetSize(this WindowBase window, Size size)
    {
      size = size.Round();

      window.Width = size.Width;
      window.Height = size.Height;
    }

    #endregion
  }
}