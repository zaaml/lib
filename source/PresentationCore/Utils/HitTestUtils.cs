// <copyright file="HitTestUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.PresentationCore.Input;

namespace Zaaml.PresentationCore.Utils
{
  public static class HitTestUtils
  {
    public static IEnumerable<UIElement> ScreenHitTest(this FrameworkElement reference, Rect screenRect)
    {
#if SILVERLIGHT
			return VisualTreeHelper.FindElementsInHostCoordinates(screenRect, reference).ToList();
#else
      var uielements = new List<UIElement>();

      VisualTreeHelper.HitTest(reference, r =>
      {
        var uie = r as UIElement;
        if (uie != null)
          uielements.Add(uie);
        return HitTestFilterBehavior.Continue;
      }, result => HitTestResultBehavior.Continue, new GeometryHitTestParameters(new RectangleGeometry(reference.TransformRectToClient(screenRect))));

      return uielements.AsEnumerable().Reverse();
#endif
    }

    public static IEnumerable<UIElement> ScreenHitTest(Rect screenRect)
    {
#if SILVERLIGHT
			return VisualTreeHelper.FindElementsInHostCoordinates(screenRect, Application.Current.RootVisual).ToList();
#else
      var uielements = new List<UIElement>();
      foreach (var visualRoot in PresentationTreeUtils.EnumerateVisualRoots().OfType<FrameworkElement>().Where(fre => fre.Dispatcher.CheckAccess()))
      {
				if (visualRoot.Dispatcher.CheckAccess() == false)
					continue;

        VisualTreeHelper.HitTest(visualRoot, r =>
        {
          var uie = r as UIElement;
          if (uie != null)
            uielements.Add(uie);
          return HitTestFilterBehavior.Continue;
        }, result => HitTestResultBehavior.Continue, new GeometryHitTestParameters(new RectangleGeometry(visualRoot.TransformRectToClient(screenRect))));
      }

      return uielements.AsEnumerable().Reverse();
#endif
    }

    public static IEnumerable<UIElement> ScreenHitTest(this FrameworkElement reference, Point screenPoint)
    {
#if SILVERLIGHT
			return VisualTreeHelper.FindElementsInHostCoordinates(screenPoint, reference).ToList();
#else
      var uielements = new List<UIElement>();

      VisualTreeHelper.HitTest(reference, r =>
      {
        var uie = r as UIElement;

        if (uie != null)
          uielements.Add(uie);

        return HitTestFilterBehavior.Continue;
      }, result => HitTestResultBehavior.Continue, new PointHitTestParameters(reference.PointFromScreen(screenPoint)));

      return uielements.AsEnumerable().Reverse();
#endif
    }

    public static IEnumerable<UIElement> ScreenHitTest(Point screenPoint)
    {
#if SILVERLIGHT
      var uielements = new List<UIElement>();

      foreach (var visualRoot in PresentationTreeUtils.EnumerateVisualRoots().OfType<FrameworkElement>())
        uielements.AddRange(VisualTreeHelper.FindElementsInHostCoordinates(screenPoint, visualRoot));

      return uielements;
#else
      var uielements = new List<UIElement>();

      foreach (var visualRoot in PresentationTreeUtils.EnumerateVisualRoots().OfType<FrameworkElement>().Where(fre => fre.Dispatcher.CheckAccess()))
      {
        VisualTreeHelper.HitTest(visualRoot, r =>
        {
          var uie = r as UIElement;

          if (uie != null)
            uielements.Add(uie);

          return HitTestFilterBehavior.Continue;
        }, result => HitTestResultBehavior.Continue, new PointHitTestParameters(visualRoot.PointFromScreen(screenPoint)));
      }

      return uielements.AsEnumerable().Reverse();
#endif
    }

#if SILVERLIGHT
    internal static List<UIElement> CursorHitTest(this FrameworkElement relative)
    {
			return VisualTreeHelper.FindElementsInHostCoordinates(MouseInt.ScreenPosition, relative).ToList();
    }
#else
    internal static List<UIElement> CursorHitTest(this FrameworkElement relative)
    {
      var uielements = new List<UIElement>();

      VisualTreeHelper.HitTest(relative, r =>
      {
        var uie = r as UIElement;

        if (uie != null)
          uielements.Add(uie);

        return HitTestFilterBehavior.Continue;
      }, result => HitTestResultBehavior.Continue, new PointHitTestParameters(Mouse.GetPosition(relative)));

      return uielements;
    }
#endif
  }
}