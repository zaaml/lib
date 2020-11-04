// <copyright file="Common.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class CommonExtensions
  {
    #region  Methods

    public static string AttributeValue(this XElement element, string attributeName)
    {
      var attribute = element.Attributes().SingleOrDefault(a => a.Name.LocalName.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
      return attribute?.Value ?? string.Empty;
    }

    public static double Height(this Thickness thickness)
    {
      return Math.Abs(thickness.Top) + Math.Abs(thickness.Bottom);
    }

    public static bool IsHorizontal(this Orientation orientation)
    {
      return orientation == Orientation.Horizontal;
    }

    public static bool IsVertical(this Orientation orientation)
    {
      return orientation == Orientation.Vertical;
    }

    public static Thickness Negate(this Thickness value)
    {
      return new Thickness(-value.Left, -value.Top, -value.Right, -value.Bottom);
    }

    public static Orientation Rotate(this Orientation orientation)
    {
      return orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
    }

    public static double Size(this Thickness thickness, Orientation orientation)
    {
      return orientation.IsHorizontal() ? thickness.Width() : thickness.Height();
    }

    public static Visibility ToVisibility(this bool isVisible)
    {
      return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public static double Width(this Thickness thickness)
    {
      return Math.Abs(thickness.Left) + Math.Abs(thickness.Right);
    }

    #endregion
  }
}