// <copyright file="OrientedScrollViewerWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Utils
{
  internal struct OrientedScrollViewerWrapper
  {
    #region Fields

    public readonly Orientation Orientation;
    public readonly ScrollViewControl ScrollViewer;

    #endregion

    #region Ctors

    public OrientedScrollViewerWrapper(ScrollViewControl scrollViewer, Orientation orientation)
    {
      ScrollViewer = scrollViewer;
      Orientation = orientation;
    }

    #endregion

    #region Properties

    public bool IsVertical => Orientation == Orientation.Vertical;

    public double Offset => IsVertical ? ScrollViewer.VerticalOffset : ScrollViewer.HorizontalOffset;

    public double ViewportSize => IsVertical ? ScrollViewer.ViewportHeight : ScrollViewer.Width;

    public double ExtentSize => IsVertical ? ScrollViewer.ExtentHeight : ScrollViewer.ExtentWidth;

    public double ScrollableSize => IsVertical ? ScrollViewer.ScrollableHeight : ScrollViewer.ScrollableWidth;

    #endregion

    #region  Methods

    public void ScrollToOffset(double offset)
    {
      if (IsVertical)
        ScrollViewer.VerticalOffset = offset;
      else
        ScrollViewer.HorizontalOffset = offset;
    }

    public void NavigatePage(Direction direction)
    {
      switch (direction)
      {
        case Direction.Forward:
          PageNext();
          break;
        case Direction.Backward:
          PagePrev();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(direction));
      }
    }

    public void NavigateLine(Direction direction)
    {
      switch (direction)
      {
        case Direction.Forward:
          LineNext();
          break;
        case Direction.Backward:
          LinePrev();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(direction));
      }
    }

    public void NavigateStart()
    {
      if (IsVertical)
        ScrollViewer.ScrollToTop();
      else
        ScrollViewer.ScrollToLeft();
    }

    public void NavigateEnd()
    {
      if (IsVertical)
        ScrollViewer.ScrollToBottom();
      else
        ScrollViewer.ScrollToRight();
    }

    public void PageNext()
    {
      if (IsVertical)
        ScrollViewer.PageDown();
      else
        ScrollViewer.PageRight();
    }

    public void PagePrev()
    {
      if (IsVertical)
        ScrollViewer.PageUp();
      else
        ScrollViewer.PageLeft();
    }

    public void LineNext()
    {
      if (IsVertical)
        ScrollViewer.LineDown();
      else
        ScrollViewer.LineRight();
    }

    public void LinePrev()
    {
      if (IsVertical)
        ScrollViewer.LineUp();
      else
        ScrollViewer.LineLeft();
    }

    #endregion
  }
}