// <copyright file="ScreenBoundsBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
  public class ScreenBoundsBehavior : BehaviorBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ActualScreenBoxProperty = DPM.Register<Rect, ScreenBoundsBehavior>
      ("ActualScreenBox");

    public static readonly DependencyProperty ActualScreenLocationProperty = DPM.Register<Point, ScreenBoundsBehavior>
      ("ActualScreenLocation");

    #endregion

    #region Properties

    public Rect ActualScreenBox
    {
      get => (Rect) GetValue(ActualScreenBoxProperty);
      set => SetValue(ActualScreenBoxProperty, value);
    }

		public Point ActualScreenLocation
    {
      get => (Point) GetValue(ActualScreenLocationProperty);
      set => SetValue(ActualScreenLocationProperty, value);
    }

    #endregion

    #region  Methods

    private void FrameworkElementOnLayoutUpdated(object sender, EventArgs eventArgs)
    {
      UpdateSize();
    }

    protected override void OnAttached()
    {
      base.OnAttached();

      FrameworkElement.LayoutUpdated += FrameworkElementOnLayoutUpdated;

      UpdateSize();
    }

    protected override void OnDetaching()
    {
      FrameworkElement.LayoutUpdated -= FrameworkElementOnLayoutUpdated;

      base.OnDetaching();
    }

    private void UpdateSize()
    {
      var screenBox = FrameworkElement.GetScreenBox();

      ActualScreenLocation = screenBox.GetTopLeft();
      ActualScreenBox = screenBox;
    }

    #endregion
  }
}