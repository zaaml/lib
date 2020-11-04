// <copyright file="MouseRootsEventsProducer.Service.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Input
{
  internal sealed partial class MouseRootsEventsProducer
  {
    #region  Nested Types

    private class MouseServiceInt : ServiceBase<UIElement>
    {
      #region  Methods

      protected override void OnAttach()
      {
        base.OnAttach();
        var uiElement = Target;
        if (uiElement == null) return;

        uiElement.MouseMove += OnMouseMove;

        uiElement.AddHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnMouseLeftButtonDown, true);
        uiElement.AddHandler(UIElement.MouseLeftButtonUpEvent, (MouseButtonEventHandler) OnMouseLeftButtonUp, true);
        uiElement.AddHandler(UIElement.MouseRightButtonDownEvent, (MouseButtonEventHandler) OnMouseRightButtonDown, true);
        uiElement.AddHandler(UIElement.MouseRightButtonUpEvent, (MouseButtonEventHandler) OnMouseRightButtonUp, true);

        uiElement.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, (MouseButtonEventHandler) OnPreviewMouseLeftButtonDown, true);
        uiElement.AddHandler(UIElement.PreviewMouseLeftButtonUpEvent, (MouseButtonEventHandler) OnPreviewMouseLeftButtonUp, true);
        uiElement.AddHandler(UIElement.PreviewMouseRightButtonDownEvent, (MouseButtonEventHandler) OnPreviewMouseRightButtonDown, true);
        uiElement.AddHandler(UIElement.PreviewMouseRightButtonUpEvent, (MouseButtonEventHandler) OnPreviewMouseRightButtonUp, true);
      }

      protected override void OnDetach()
      {
        base.OnDetach();
        var uiElement = Target;
        if (uiElement == null) return;

        uiElement.MouseMove -= OnMouseMove;

        uiElement.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnMouseLeftButtonDown);
        uiElement.RemoveHandler(UIElement.MouseLeftButtonUpEvent, (MouseButtonEventHandler) OnMouseLeftButtonUp);
        uiElement.RemoveHandler(UIElement.MouseRightButtonDownEvent, (MouseButtonEventHandler) OnMouseRightButtonDown);
        uiElement.RemoveHandler(UIElement.MouseRightButtonUpEvent, (MouseButtonEventHandler) OnMouseRightButtonUp);

        uiElement.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, (MouseButtonEventHandler) OnPreviewMouseLeftButtonDown);
        uiElement.RemoveHandler(UIElement.PreviewMouseLeftButtonUpEvent, (MouseButtonEventHandler) OnPreviewMouseLeftButtonUp);
        uiElement.RemoveHandler(UIElement.PreviewMouseRightButtonDownEvent, (MouseButtonEventHandler) OnPreviewMouseRightButtonDown);
        uiElement.RemoveHandler(UIElement.PreviewMouseRightButtonUpEvent, (MouseButtonEventHandler) OnPreviewMouseRightButtonUp);
      }

      private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnMouseLeftButtonDown(e);
      }

      private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnMouseLeftButtonUp(e);
      }

      private static void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
      {
        InstanceCore.OnMouseMove(mouseEventArgs);
      }

      private static void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnMouseRightButtonDown(e);
      }

      private static void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnMouseRightButtonUp(e);
      }

      private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnPreviewMouseLeftButtonDown(e);
      }

      private static void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnPreviewMouseLeftButtonUp(e);
      }

      private static void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnPreviewMouseRightButtonDown(e);
      }

      private static void OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
      {
        InstanceCore.OnPreviewMouseRightButtonUp(e);
      }

      #endregion
    }

    #endregion
  }
}