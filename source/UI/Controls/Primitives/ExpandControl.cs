// <copyright file="ExpandControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Panels;
using ZaamlContentControl = Zaaml.UI.Controls.Core.ContentControl;

namespace Zaaml.UI.Controls.Primitives
{
  public class ExpandControl : ZaamlContentControl
  {
		#region Static Fields and Constants

		public static readonly DependencyProperty IsExpandedProperty = DPM.Register<bool, ExpandControl>
      ("IsExpanded", e => e.UpdatePanel(true));

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ExpandControl>
      ("Orientation", Orientation.Vertical);

    public static readonly DependencyProperty TransitionProperty = DPM.Register<PresentationCore.Animation.Transition, ExpandControl>
      ("Transition");

    #endregion

    #region Fields

    private Storyboard _currentStoryboard;
		private ExpandPanel _expandPanel;

    #endregion

    #region Ctors

    static ExpandControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ExpandControl>();
    }

    public ExpandControl()
    {
      this.OverrideStyleKey<ExpandControl>();
    }

    #endregion

    #region Properties

    private double ExpandRatio => IsExpanded ? 1.0 : 0.0;

    public bool IsExpanded
    {
	    get => (bool)GetValue(IsExpandedProperty);
	    set => SetValue(IsExpandedProperty, value.Box());
    }

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

		public PresentationCore.Animation.Transition Transition
    {
      get => (PresentationCore.Animation.Transition) GetValue(TransitionProperty);
      set => SetValue(TransitionProperty, value);
		}

    #endregion

    #region  Methods

    private void OnAnimationClockCompleted(object o, EventArgs e)
    {
      _expandPanel.ExpandRatio = ExpandRatio;
      _currentStoryboard = null;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _expandPanel = (ExpandPanel) GetTemplateChild("ExpandPanel");

      UpdatePanel(false);
    }

    private void StartAnimation()
    {
      var transition = Transition;

      var timeline = new DoubleAnimation
      {
        To = ExpandRatio,
        EasingFunction = transition.EasingFunction,
        FillBehavior = FillBehavior.HoldEnd,
        Duration = transition.Duration,
        BeginTime = transition.BeginTime
      };

      _currentStoryboard = new Storyboard();

      _currentStoryboard.Children.Add(timeline);
      _currentStoryboard.Completed += OnAnimationClockCompleted;

      Storyboard.SetTarget(_currentStoryboard, _expandPanel);
      Storyboard.SetTargetProperty(_currentStoryboard, new PropertyPath(ExpandPanel.ExpandRatioProperty));

      _currentStoryboard.Begin();
    }

    private void StopAnimation()
    {
      if (_currentStoryboard == null) 
	      return;

      _currentStoryboard.Completed -= OnAnimationClockCompleted;

      _currentStoryboard.Stop();

      _expandPanel.ExpandRatio = ExpandRatio;
      _currentStoryboard = null;
    }

    private void UpdatePanel(bool useAnimation)
    {
      if (_expandPanel == null)
        return;

      if (useAnimation == false || Transition == null)
      {
        _expandPanel.ExpandRatio = IsExpanded ? 1.0 : 0.0;

        return;
      }

      StopAnimation();
      StartAnimation();
    }

    #endregion
  }
}