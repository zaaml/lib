// <copyright file="ScrollViewerExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Extensions
{
  internal static class ScrollViewerExtensions
  {
    #region Static Fields and Constants

    private const double Tolerance = 0.001;
    private const double LineChange = 16.0;

    #endregion

    #region  Methods

    internal static OrientedScrollViewerWrapper AsOriented(this ScrollViewControl scrollViewer, Orientation orientation)
    {
      return new OrientedScrollViewerWrapper(scrollViewer, orientation);
    }

    public static Vector GetOffset(this ScrollViewer scrollViewer)
    {
      return new Vector(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
    }

    internal static bool HandleKeyDown(this ScrollViewer scrollViewer, Key key, Orientation? orientation = null)
    {
      var isControlPressed = ModifierKeys.Control == (Keyboard.Modifiers & ModifierKeys.Control);

      var isLTR = scrollViewer.FlowDirection == FlowDirection.LeftToRight;

      var diffOrientation = (isControlPressed && orientation == Orientation.Vertical);

      switch (key)
      {
        case Key.PageUp:
          if (orientation == Orientation.Horizontal)
            return false;
          scrollViewer.PageUp();
          return true;
        case Key.PageDown:
          if (orientation == Orientation.Horizontal)
            return false;
          scrollViewer.PageDown();
          return true;
        case Key.End:
          if (diffOrientation)
            return false;

          if (isControlPressed)
            scrollViewer.ScrollToRight();
          else
            scrollViewer.ScrollToBottom();
          return true;
        case Key.Home:
          if (diffOrientation)
            return false;

          if (isControlPressed)
            scrollViewer.ScrollToLeft();
          else
            scrollViewer.ScrollToTop();
          return true;
        case Key.Left:
          if (orientation == Orientation.Vertical)
            return false;

          if (isLTR)
            scrollViewer.LineLeft();
          else
            scrollViewer.LineRight();

          return true;
        case Key.Up:
          if (orientation == Orientation.Horizontal)
            return false;

          scrollViewer.LineUp();
          return true;
        case Key.Right:
          if (orientation == Orientation.Vertical)
            return false;

          if (isLTR)
            scrollViewer.LineRight();
          else
            scrollViewer.LineLeft();
          return true;
        case Key.Down:
          if (orientation == Orientation.Horizontal)
            return false;

          scrollViewer.LineDown();
          return true;
      }

      return false;
    }

    public static bool IsAtBottom(this ScrollViewer viewer)
    {
      return Math.Abs(viewer.VerticalOffset - viewer.ScrollableHeight) < Tolerance;
    }

    public static bool IsAtLeft(this ScrollViewer viewer)
    {
      return Math.Abs(viewer.HorizontalOffset - 0.0) < Tolerance;
    }

    public static bool IsAtRight(this ScrollViewer viewer)
    {
      return Math.Abs(viewer.HorizontalOffset - viewer.ScrollableWidth) < Tolerance;
    }

    public static bool IsAtTop(this ScrollViewer viewer)
    {
      return Math.Abs(viewer.VerticalOffset - 0.0) < Tolerance;
    }

    public static bool IsHorizontalScrollAvailable(this ScrollViewer viewer)
    {
      return viewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
    }

    public static bool IsVerticalScrollAvailable(this ScrollViewer viewer)
    {
      return viewer.ComputedVerticalScrollBarVisibility == Visibility.Visible;
    }

    public static void LineDown(this ScrollViewer viewer)
    {
      ScrollByVerticalOffset(viewer, LineChange);
    }

    public static void LineLeft(this ScrollViewer viewer)
    {
      ScrollByHorizontalOffset(viewer, -LineChange);
    }

    public static void LineRight(this ScrollViewer viewer)
    {
      ScrollByHorizontalOffset(viewer, LineChange);
    }

    public static void LineUp(this ScrollViewer viewer)
    {
      ScrollByVerticalOffset(viewer, -LineChange);
    }

    public static void PageDown(this ScrollViewer viewer)
    {
      ScrollByVerticalOffset(viewer, viewer.ViewportHeight);
    }

    public static void PageLeft(this ScrollViewer viewer)
    {
      ScrollByHorizontalOffset(viewer, -viewer.ViewportWidth);
    }

    public static void PageRight(this ScrollViewer viewer)
    {
      ScrollByHorizontalOffset(viewer, viewer.ViewportWidth);
    }

    public static void PageUp(this ScrollViewer viewer)
    {
      ScrollByVerticalOffset(viewer, -viewer.ViewportHeight);
    }

    private static void ScrollByHorizontalOffset(ScrollViewer viewer, double offset)
    {
      offset += viewer.HorizontalOffset;
      viewer.ScrollToHorizontalOffset(offset.Clamp(0, viewer.ScrollableWidth));
    }

    private static void ScrollByVerticalOffset(ScrollViewer viewer, double offset)
    {
      offset += viewer.VerticalOffset;
      viewer.ScrollToVerticalOffset(offset.Clamp(0, viewer.ScrollableHeight));
    }

    public static void ScrollTo(this ScrollViewer scrollViewer, Vector vector)
    {
      scrollViewer.ScrollToHorizontalOffset(vector.X.Clamp(0, scrollViewer.ScrollableWidth));
      scrollViewer.ScrollToVerticalOffset(vector.Y.Clamp(0, scrollViewer.ScrollableHeight));
    }

    public static void ScrollToBottom(this ScrollViewer viewer)
    {
      viewer.ScrollToVerticalOffset(viewer.ExtentHeight);
    }

    public static void ScrollToLeft(this ScrollViewer viewer)
    {
      viewer.ScrollToHorizontalOffset(viewer.ExtentWidth);
    }

    public static void ScrollToRight(this ScrollViewer viewer)
    {
      viewer.ScrollToHorizontalOffset(0);
    }

    public static void ScrollToTop(this ScrollViewer viewer)
    {
      viewer.ScrollToVerticalOffset(0);
    }

    #endregion
  }
}