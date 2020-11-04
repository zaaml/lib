// <copyright file="MouseObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Input
{
  internal class MouseObserver
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsEnabledProperty = DPM.RegisterAttached<bool, MouseObserver>
      ("IsEnabled", false, OnIsEnabledChanged);

    public static readonly DependencyProperty PositionProperty = DPM.RegisterAttached<Point, MouseObserver>
      ("Position");

    #endregion

    #region  Methods

    public static bool GetIsEnabled(UIElement element)
    {
      return (bool) element.GetValue(IsEnabledProperty);
    }

    public static Point GetPosition(UIElement element)
    {
      return (Point) element.GetValue(PositionProperty);
    }

    private static void OnElementMouseMove(object sender, MouseEventArgs e)
    {
      var element = sender as FrameworkElement;
      SetPosition(element, e.GetPosition(element));
    }

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = (UIElement) d;

      if ((bool) e.NewValue)
        element.MouseMove += OnElementMouseMove;
      else
        element.MouseMove -= OnElementMouseMove;
    }

    public static void SetIsEnabled(UIElement element, bool value)
    {
      element.SetValue(IsEnabledProperty, value);
    }

    public static void SetPosition(UIElement element, Point value)
    {
      element.SetValue(PositionProperty, value);
    }

    #endregion
  }
}