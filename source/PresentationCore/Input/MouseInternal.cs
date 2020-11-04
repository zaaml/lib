// <copyright file="MouseInt.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;
#if !SILVERLIGHT
using System.Linq;

#endif

namespace Zaaml.PresentationCore.Input
{
  internal static class MouseInternal
  {
    #region Properties

    public static DependencyObject DirectlyOver => MouseServiceInstance.DirectlyOver;

    public static Point ScreenPosition => MouseServiceInstance.ScreenPosition;

    public static bool IsMouseCaptured => MouseServiceInstance.IsMouseCaptured;

    internal static UIElement LastUIElement => MouseServiceInstance.LastElement;

    public static MouseButtonState LeftButtonState => MouseServiceInstance.LeftButtonState;

    private static IMouseService MouseServiceInstance
    {
#if SILVERLIGHT
      get { return MouseService.Instance; }
#else
      get { return MouseService.Instance; }
#endif
    }

    public static MouseButtonState RightButtonState => MouseServiceInstance.RightButtonState;

    #endregion

    #region  Methods

    public static bool Capture(UIElement element)
    {
      return MouseServiceInstance.CaptureMouse(element);
    }

    public static bool GetIsMouseOver(this FrameworkElement fre)
    {
      return fre.GetClientBox().Contains(GetPosition(fre));
    }

    public static Point GetPosition(UIElement relativeTo)
    {
      return MouseServiceInstance.GetPosition(relativeTo);
    }

    public static IEnumerable<UIElement> HitTest()
    {
      return HitTestUtils.ScreenHitTest(ScreenPosition);
    }

	  public static IEnumerable<UIElement> HitTest(Point screenPosition)
	  {
		  return HitTestUtils.ScreenHitTest(screenPosition);
	  }

		public static void InvokeOnMouseLeftButtonReleased(Action action, bool invokeIfNotPressed)
    {
      if (LeftButtonState == MouseButtonState.Released)
      {
        if (invokeIfNotPressed)
          action();
      }
      else
      {
        MouseButtonEventHandlerInt handler = null;
        handler = (sender, args) =>
        {
          MouseLeftButtonUp -= handler;
          action();
        };

        MouseLeftButtonUp += handler;
      }
    }

    public static void InvokeOnMouseRightButtonReleased(Action action, bool invokeIfNotPressed)
    {
      if (RightButtonState == MouseButtonState.Released)
      {
        if (invokeIfNotPressed)
          action();
      }
      else
      {
        MouseButtonEventHandlerInt handler = null;
        handler = (sender, args) =>
        {
          MouseRightButtonUp -= handler;
          action();
        };

        MouseRightButtonUp += handler;
      }
    }

    internal static bool IsMouseButtonClick(UIElement element, UIElement mouseEventElement)
    {
      if (mouseEventElement == null)
        return false;

      foreach (var ancestor in mouseEventElement.GetAncestors(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance))
      {
        if (ReferenceEquals(ancestor, element))
          break;

        var clickBarrier = ancestor as IClickBarrier;

        if (clickBarrier?.PropagateEvents == false)
          return false;
      }

      return IsMouseInsideEventHelper(element, mouseEventElement);
    }

	  internal static bool IsMouseInsideEventHelper(UIElement element, UIElement mouseEventElement)
	  {
		  return IsMouseInsideEventHelper(element, mouseEventElement, ScreenPosition);
	  }

		internal static bool IsMouseInsideEventHelper(UIElement element, UIElement mouseEventElement, Point screenPosition)
		{
			var isWithinTree = PresentationTreeUtils.IsWithinTree(element, mouseEventElement);

#if !SILVERLIGHT
			var captured = Mouse.Captured as UIElement;

			if (captured == null)
				return isWithinTree;

			if (captured.IsDescendantOf(element, MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance))
				return isWithinTree;

			var isCapturedSelf = ReferenceEquals(captured, mouseEventElement);

			if (isCapturedSelf)
				return IsUnderCursor(element) || HitTest(screenPosition).Any(e => ReferenceEquals(e, element));

			var isCapturedEvent = mouseEventElement.IsDescendantOf(captured, MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance);

			return isCapturedEvent || IsUnderCursor(element) || HitTest(screenPosition).Any(e => ReferenceEquals(e, element));
#else
			return isWithinTree;
#endif
	  }

#if !SILVERLIGHT
	  private static bool IsUnderCursor(UIElement element)
	  {
		  var directlyOver = Mouse.DirectlyOver as UIElement;
		  var isUnderCursor = directlyOver != null && directlyOver.IsSelfOrDescendantOf(element, VisualTreeEnumerationStrategy.Instance);

		  return isUnderCursor;
	  }
#endif

		public static void Release()
    {
      MouseServiceInstance.ReleaseMouseCapture();
    }

    #endregion

    public static event MouseEventHandlerInt MouseMove
    {
      add => MouseServiceInstance.MouseMove += value;
      remove => MouseServiceInstance.MouseMove -= value;
    }

    public static event MouseButtonEventHandlerInt MouseLeftButtonDown
    {
      add => MouseServiceInstance.MouseLeftButtonDown += value;
      remove => MouseServiceInstance.MouseLeftButtonDown -= value;
    }

    public static event MouseButtonEventHandlerInt MouseLeftButtonUp
    {
      add => MouseServiceInstance.MouseLeftButtonUp += value;
      remove => MouseServiceInstance.MouseLeftButtonUp -= value;
    }

    public static event MouseButtonEventHandlerInt MouseRightButtonDown
    {
      add => MouseServiceInstance.MouseRightButtonDown += value;
      remove => MouseServiceInstance.MouseRightButtonDown -= value;
    }

    public static event MouseButtonEventHandlerInt MouseRightButtonUp
    {
      add => MouseServiceInstance.MouseRightButtonUp += value;
      remove => MouseServiceInstance.MouseRightButtonUp -= value;
    }

    public static event MouseButtonEventHandlerInt PreviewMouseLeftButtonDown
    {
      add => MouseServiceInstance.PreviewMouseLeftButtonDown += value;
      remove => MouseServiceInstance.PreviewMouseLeftButtonDown -= value;
    }

    public static event MouseButtonEventHandlerInt PreviewMouseLeftButtonUp
    {
      add => MouseServiceInstance.PreviewMouseLeftButtonUp += value;
      remove => MouseServiceInstance.PreviewMouseLeftButtonUp -= value;
    }

    public static event MouseButtonEventHandlerInt PreviewMouseRightButtonDown
    {
      add => MouseServiceInstance.PreviewMouseRightButtonDown += value;
      remove => MouseServiceInstance.PreviewMouseRightButtonDown -= value;
    }

    public static event MouseButtonEventHandlerInt PreviewMouseRightButtonUp
    {
      add => MouseServiceInstance.PreviewMouseRightButtonUp += value;
      remove => MouseServiceInstance.MouseRightButtonUp -= value;
    }
  }
  
  internal static class MouseEventArgsExtensions
  {
    #region  Methods

    public static bool IsWithin(this MouseEventArgs args, FrameworkElement frameworkElement)
    {
      return frameworkElement.GetClientBox().Contains(args.GetPosition(frameworkElement));
    }

    #endregion
  }

  /// <summary>
  /// Mouse events for buttons handling should not propagate this barrier
  /// </summary>
  internal interface IClickBarrier
  {
    bool PropagateEvents { get; }
  }
}