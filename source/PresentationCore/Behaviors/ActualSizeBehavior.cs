// <copyright file="ActualSizeBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
  public class ActualSizeBehavior : BehaviorBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ActualWidthProperty = DPM.Register<double, ActualSizeBehavior>
      ("ActualWidth");

    public static readonly DependencyProperty ActualHeightProperty = DPM.Register<double, ActualSizeBehavior>
      ("ActualHeight");

    public static readonly DependencyProperty ActualSizeProperty = DPM.Register<Size, ActualSizeBehavior>
      ("ActualSize");

    public static readonly DependencyProperty ActualClientBoxProperty = DPM.Register<Rect, ActualSizeBehavior>
      ("ActualClientBox");

    #endregion

    #region Properties

    public Rect ActualClientBox
    {
      get => (Rect) GetValue(ActualClientBoxProperty);
      set => SetValue(ActualClientBoxProperty, value);
    }

    public double ActualHeight
    {
      get => (double) GetValue(ActualHeightProperty);
      set => SetValue(ActualHeightProperty, value);
    }

    public Size ActualSize
    {
      get => (Size) GetValue(ActualSizeProperty);
      set => SetValue(ActualSizeProperty, value);
    }

    public double ActualWidth
    {
      get => (double) GetValue(ActualWidthProperty);
      set => SetValue(ActualWidthProperty, value);
    }

    #endregion

    #region  Methods

    private void FrameworkElementOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
    {
      UpdateSize();
    }

    protected override void OnAttached()
    {
      base.OnAttached();

      FrameworkElement.SizeChanged += FrameworkElementOnSizeChanged;

      UpdateSize();
    }

    protected override void OnDetaching()
    {
      FrameworkElement.SizeChanged -= FrameworkElementOnSizeChanged;

      base.OnDetaching();
    }

    private void UpdateSize()
    {
      var size = new Size(FrameworkElement.ActualWidth, FrameworkElement.ActualHeight);

      ActualWidth = size.Width;
      ActualHeight = size.Height;
      ActualSize = size;
      ActualClientBox = size.Rect();
    }

    #endregion
  }


  //public class TrackIsFirstChildBehavior : BehaviorBase
  //{
  //	private static readonly DependencyPropertyKey IsFirstPropertyKey = DPM.RegisterReadOnly<bool, TrackIsFirstChildBehavior>
  //		("IsFirstInt");

  //	public static readonly DependencyProperty TargetProperty = DPM.Register<FrameworkElement, TrackIsFirstChildBehavior>
  //		("Target");

  //	public FrameworkElement Target
  //	{
  //		get { return (FrameworkElement) GetValue(TargetProperty); }
  //		set { SetValue(TargetProperty, value); }
  //	}

  //	public static readonly DependencyProperty IsFirstProperty = IsFirstPropertyKey.DependencyProperty;

  //	public bool IsFirst
  //	{
  //		get { return (bool) GetValue(IsFirstProperty); }
  //		private set { this.SetValue(IsFirstPropertyKey, value); }
  //	}

  //	protected override void OnAttached()
  //	{
  //		base.OnAttached();
  //		FrameworkElement.LayoutUpdated += FrameworkElementOnLayoutUpdated;
  //	}

  //	private void FrameworkElementOnLayoutUpdated(object sender, EventArgs eventArgs)
  //	{
  //		var target = Target ?? FrameworkElement;
  //		var parent = VisualTreeHelper.GetParent(target) as Panel;
  //		IsFirst = parent != null && ReferenceEquals(parent.Children.FirstOrDefault(), target);
  //	}

  //	protected override void OnDetaching()
  //	{
  //		FrameworkElement.LayoutUpdated -= FrameworkElementOnLayoutUpdated;
  //		base.OnDetaching();
  //	}
  //}
}