// <copyright file="GlobalMouseEventsProducerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
  internal abstract class GlobalMouseEventsProducerBase : IGlobalMouseEventsProducer
  {
    #region  Methods

    protected void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      MouseLeftButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Left, MouseButtonState.Pressed));
    }

    protected void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      MouseLeftButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Left, MouseButtonState.Released));
    }

    protected void OnMouseMove(MouseEventArgs e)
    {
      MouseMove?.Invoke(this, e.ToMouseEventArgsInt());
    }

    protected void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
      MouseRightButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Right, MouseButtonState.Pressed));
    }

    protected void OnMouseRightButtonUp(MouseButtonEventArgs e)
    {
      MouseRightButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Right, MouseButtonState.Released));
    }

    protected void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      PreviewMouseLeftButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Left, MouseButtonState.Pressed));
    }

    protected void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      PreviewMouseLeftButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Left, MouseButtonState.Released));
    }

    protected void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
      PreviewMouseRightButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Right, MouseButtonState.Pressed));
    }

    protected void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
    {
      PreviewMouseRightButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt(MouseButton.Right, MouseButtonState.Released));
    }

    #endregion

    #region Interface Implementations

    #region IGlobalMouseEventsProducer

    public event MouseEventHandlerInt MouseMove;

    public event MouseButtonEventHandlerInt MouseLeftButtonDown;
    public event MouseButtonEventHandlerInt MouseLeftButtonUp;
    public event MouseButtonEventHandlerInt MouseRightButtonDown;
    public event MouseButtonEventHandlerInt MouseRightButtonUp;

    public event MouseButtonEventHandlerInt PreviewMouseLeftButtonDown;
    public event MouseButtonEventHandlerInt PreviewMouseLeftButtonUp;
    public event MouseButtonEventHandlerInt PreviewMouseRightButtonDown;
    public event MouseButtonEventHandlerInt PreviewMouseRightButtonUp;

    #endregion

    #endregion
  }
}