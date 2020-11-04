// <copyright file="IMouseService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
  internal interface IMouseService
  {
    #region Fields

    event MouseEventHandlerInt MouseMove;

    event MouseButtonEventHandlerInt MouseLeftButtonDown;
    event MouseButtonEventHandlerInt MouseLeftButtonUp;
    event MouseButtonEventHandlerInt MouseRightButtonDown;
    event MouseButtonEventHandlerInt MouseRightButtonUp;


    event MouseButtonEventHandlerInt PreviewMouseLeftButtonDown;
    event MouseButtonEventHandlerInt PreviewMouseLeftButtonUp;
    event MouseButtonEventHandlerInt PreviewMouseRightButtonDown;
    event MouseButtonEventHandlerInt PreviewMouseRightButtonUp;

    #endregion

    #region Properties

    UIElement DirectlyOver { get; }

    bool IsMouseCaptured { get; }

    UIElement LastElement { get; }

    MouseButtonState LeftButtonState { get; }

    UIElement MouseCaptureElement { get; }

    MouseButtonState RightButtonState { get; }

    Point ScreenPosition { get; }

    #endregion

    #region  Methods

    bool CaptureMouse(UIElement uie);

    Point GetPosition(UIElement relativeTo);

    void ReleaseMouseCapture();

    #endregion
  }
}