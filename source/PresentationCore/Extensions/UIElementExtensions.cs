// <copyright file="UIElementExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
using RoutedEvent = System.Windows.RoutedEventSL;
#endif

namespace Zaaml.PresentationCore.Extensions
{
  public static class UIElementExtensions
  {
#region  Methods

    public static double DesiredHeight(this UIElement uie)
    {
      return uie.DesiredSize.Height;
    }

    public static double DesiredWidth(this UIElement uie)
    {
      return uie.DesiredSize.Width;
    }

    public static TranslateTransform GetOrCreateTranslateTransform(this UIElement uiElement)
    {
      return uiElement.GetValueOrCreate(UIElement.RenderTransformProperty, () => new TranslateTransform());
    }

    public static void Hide(this UIElement uie)
    {
      uie.Visibility = Visibility.Collapsed;
    }

    public static bool IsHidden(this UIElement uie)
    {
      return uie.Visibility == Visibility.Collapsed;
    }

    public static bool IsVisible(this UIElement uie)
    {
      return uie.Visibility == Visibility.Visible;
    }

    internal static bool IsLayoutVisible(this UIElement uie)
    {
      return uie.Visibility != Visibility.Collapsed;
    }

    public static void Show(this UIElement uie)
    {
      uie.Visibility = Visibility.Visible;
    }

    public static void RaiseRoutedEvent(this UIElement uie, RoutedEventArgs args)
    {
      uie.RaiseEventInt(args);
    }

    public static void RemoveRoutedHandler(this UIElement uie, RoutedEvent routedEvent, Delegate handler)
    {
      uie.RemoveHandlerInt(routedEvent, handler);
    }
    public static void AddRoutedHandler(this UIElement uie, RoutedEvent routedEvent, Delegate handler)
    {
      uie.AddHandlerInt(routedEvent, handler, false);
    }

    public static void AddRoutedHandler(this UIElement uie, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
    {
      uie.AddHandlerInt(routedEvent, handler, true);
    }

    internal static void InvalidateAncestorsMeasure(this UIElement uie, UIElement untilAncestor)
    {
      PanelUtils.InvalidateAncestorsMeasure(uie, untilAncestor);
    }

    internal static void InvalidateAncestorsArrange(this UIElement uie, UIElement untilAncestor)
    {
      PanelUtils.InvalidateAncestorsArrange(uie, untilAncestor);
    }

    internal static void InvalidateAncestorsMeasure(this UIElement uie)
    {
      PanelUtils.InvalidateAncestorsMeasure(uie);
    }

    internal static void InvalidateAncestorsArrange(this UIElement uie)
    {
      PanelUtils.InvalidateAncestorsArrange(uie);
    }

    internal static void InvalidateAncestorsMeasure(this UIElement uie, UIElement untilAncestor, bool includeLast)
    {
      PanelUtils.InvalidateAncestorsMeasureIncludeLast(uie, untilAncestor);
    }

    internal static void InvalidateAncestorsArrange(this UIElement uie, UIElement untilAncestor, bool includeLast)
    {
      PanelUtils.InvalidateAncestorsArrangeIncludeLast(uie, untilAncestor);
    }

#if SILVERLIGHT
    internal static void RaiseEventInt(this UIElement uie, RoutedEventArgs args)
    {
      EventManager.RaiseEvent(uie, args);
    }

    internal static void RemoveHandlerInt(this UIElement uie, RoutedEvent routedEvent, Delegate handler)
    {
      EventManager.RemoveHandler(uie, routedEvent, handler);
    }

    internal static void AddHandlerInt(this UIElement uie, RoutedEvent routedEvent, Delegate handler)
    {
      EventManager.AddHandler(uie, routedEvent, handler, false);
    }

    internal static void AddHandlerInt(this UIElement uie, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
    {
      EventManager.AddHandler(uie, routedEvent, handler, handledEventsToo);
    }
#else
    internal static void RaiseEventInt(this UIElement uie, RoutedEventArgs args)
    {
      uie.RaiseEvent(args);
    }

    internal static void RemoveHandlerInt(this UIElement uie, RoutedEvent routedEvent, Delegate handler)
    {
      uie.RemoveHandler(routedEvent, handler);
    }

    internal static void AddHandlerInt(this UIElement uie, RoutedEvent routedEvent, Delegate handler)
    {
      uie.AddHandler(routedEvent, handler);
    }

    internal static void AddHandlerInt(this UIElement uie, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
    {
      uie.AddHandler(routedEvent, handler, handledEventsToo);
    }
#endif

#endregion
  }
}