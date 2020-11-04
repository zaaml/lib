// <copyright file="DragOutBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DragOutBehavior
  {
    #region Fields

    private FrameworkElement _target;

    #endregion

    #region Properties

    public ICommand DragOutCommand { get; set; }

    public double DragOutDistance { get; set; } = 20;

    private bool IsMouseCaptured { get; set; }

    public Point OriginMousePosition { get; private set; }

    public bool ProcessHandledEvents { get; set; }

    public FrameworkElement Target
    {
      get => _target;
      set
      {
        if (ReferenceEquals(_target, value))
          return;

        if (_target != null)
          OnDetaching();

        _target = value;

        if (_target != null)
          OnAttached();
      }
    }

    #endregion

    #region  Methods

    private void InvokeDragOut()
    {
      if (DragOutCommand?.CanExecute(DragOutDistance) != true)
        return;

      ReleaseMouseCapture();

      DragOutCommand.Execute(DragOutDistance);
    }

    private void OnAttached()
    {
      Target.AddHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnMouseLeftButtonDown, true);

      Target.MouseLeftButtonUp += OnMouseLeftButtonUp;
      Target.MouseMove += OnMouseMove;
      Target.LostMouseCapture += OnLostMouseCapture;
    }

    private void OnDetaching()
    {
      Target.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnMouseLeftButtonDown);

      Target.MouseLeftButtonUp -= OnMouseLeftButtonUp;
      Target.MouseMove -= OnMouseMove;
      Target.LostMouseCapture -= OnLostMouseCapture;

      ReleaseMouseCapture();
    }

    private void OnLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
    {
      IsMouseCaptured = false;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.Handled && ProcessHandledEvents == false)
        return;

      if (MouseInternal.IsMouseCaptured)
        return;

      IsMouseCaptured = MouseInternal.Capture(Target);

      if (IsMouseCaptured == false)
        return;

      OriginMousePosition = e.GetPosition(Target);
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ReleaseMouseCapture();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
      if (IsMouseCaptured == false)
        return;

      var position = e.GetPosition(Target);

      if (PointUtils.SubtractPoints(OriginMousePosition, position).Length >= DragOutDistance)
        InvokeDragOut();
    }

    private void ReleaseMouseCapture()
    {
      if (IsMouseCaptured)
        Target.ReleaseMouseCapture();

      IsMouseCaptured = false;
    }

    #endregion
  }
}