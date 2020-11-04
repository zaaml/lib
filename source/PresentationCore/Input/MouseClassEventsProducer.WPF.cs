// <copyright file="MouseClassEventsProducer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
  internal sealed class MouseClassEventsProducer : GlobalMouseEventsProducerBase
  {
    #region Static Fields and Constants

    public static readonly IGlobalMouseEventsProducer Instance = new MouseClassEventsProducer();

    #endregion

    #region Ctors

    private MouseClassEventsProducer()
    {
      EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnGlobalMouseDown), true);
      EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseUpEvent, new MouseButtonEventHandler(OnGlobalMouseUp), true);

      EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnGlobalPreviewMouseDown), true);
      EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(OnGlobalPreviewMouseUp), true);

      EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseMoveEvent, new MouseEventHandler(OnGlobalMouseMove), true);
    }

    #endregion

    #region Properties

    private object LastEventArgs { get; set; }

    #endregion

    #region  Methods

    private void OnGlobalMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (ReferenceEquals(e, LastEventArgs))
        return;

      LastEventArgs = e;

      switch (e.ChangedButton)
      {
        case MouseButton.Left:
          OnMouseLeftButtonDown(e);
          break;
        case MouseButton.Middle:
          break;
        case MouseButton.Right:
          OnMouseRightButtonDown(e);
          break;
        case MouseButton.XButton1:
          break;
        case MouseButton.XButton2:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }


    private void OnGlobalMouseMove(object sender, MouseEventArgs e)
    {
      if (ReferenceEquals(e, LastEventArgs))
        return;

      LastEventArgs = e;

      var visualSource = e.Source as UIElement;

      if (visualSource == null || PresentationSource.FromVisual(visualSource) == null)
        return;

      OnMouseMove(e);
    }

    private void OnGlobalMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (ReferenceEquals(e, LastEventArgs))
        return;

      LastEventArgs = e;

      switch (e.ChangedButton)
      {
        case MouseButton.Left:
          OnMouseLeftButtonUp(e);
          break;
        case MouseButton.Middle:
          break;
        case MouseButton.Right:
          OnMouseRightButtonUp(e);
          break;
        case MouseButton.XButton1:
          break;
        case MouseButton.XButton2:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void OnGlobalPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (ReferenceEquals(e, LastEventArgs))
        return;

      LastEventArgs = e;

      switch (e.ChangedButton)
      {
        case MouseButton.Left:
          OnPreviewMouseLeftButtonDown(e);
          break;
        case MouseButton.Middle:
          break;
        case MouseButton.Right:
          OnPreviewMouseRightButtonDown(e);
          break;
        case MouseButton.XButton1:
          break;
        case MouseButton.XButton2:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void OnGlobalPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (ReferenceEquals(e, LastEventArgs))
        return;

      LastEventArgs = e;

      switch (e.ChangedButton)
      {
        case MouseButton.Left:
          OnPreviewMouseLeftButtonUp(e);
          break;
        case MouseButton.Middle:
          break;
        case MouseButton.Right:
          OnPreviewMouseRightButtonUp(e);
          break;
        case MouseButton.XButton1:
          break;
        case MouseButton.XButton2:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion
  }
}