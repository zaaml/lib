// <copyright file="RepeatButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
#if !SILVERLIGHT
using Zaaml.Core.Extensions;

#endif

namespace Zaaml.UI.Controls.Primitives
{
  public class RepeatButton : ButtonBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty DelayProperty = DPM.Register<int, RepeatButton>
      ("Delay", DefaultDelay);

    public static readonly DependencyProperty IntervalProperty = DPM.Register<int, RepeatButton>
      ("Interval", DefaultInterval);

    #endregion

    #region Fields

    private DispatcherTimer _timer;

    #endregion

    #region Ctors

    static RepeatButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RepeatButton>();

#if !SILVERLIGHT
      ClickModeProperty.OverrideMetadata(typeof(RepeatButton), new PropertyMetadataXm(ClickMode.Press));
#endif
    }

    public RepeatButton()
    {
      this.OverrideStyleKey<RepeatButton>();
    }

    #endregion

    #region Properties

    private static int DefaultDelay
    {
      get
      {
#if SILVERLIGHT
        return 500;
#else
        const int delayFactor = 250;
        var systemDelay = SystemParameters.KeyboardDelay.Clamp(0, 3);
        return (systemDelay + 1) * delayFactor;
#endif
      }
    }

    private static int DefaultInterval
    {
      get
      {
#if SILVERLIGHT
        return 33;
#else
        var keyboardSpeed = SystemParameters.KeyboardSpeed.Clamp(0, 31);
        return (31 - keyboardSpeed) * (400 - 1000 / 30) / 31 + 1000 / 30;
#endif
      }
    }

    public int Delay
    {
      get => (int) GetValue(DelayProperty);
      set => SetValue(DelayProperty, value);
    }


    public int Interval
    {
      get => (int) GetValue(IntervalProperty);
      set => SetValue(IntervalProperty, value);
    }

    #endregion

    #region  Methods

    private bool HandleIsMouseOverChanged()
    {
      if (ClickMode != ClickMode.Hover)
        return false;

      if (IsMouseOver)
        StartTimer();
      else
        StopTimer();

      return true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      if (e.Key != Key.Space || ClickMode == ClickMode.Hover)
        return;

      StartTimer();
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      if (e.Key == Key.Space && ClickMode != ClickMode.Hover)
        StopTimer();

      base.OnKeyUp(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      base.OnLostMouseCapture(e);
      StopTimer();
    }


    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

#if !SILVERLIGHT
      e.Handled |= HandleIsMouseOverChanged();
#endif
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);

#if !SILVERLIGHT
      e.Handled |= HandleIsMouseOverChanged();
#endif
    }


    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      if (!IsPressed || ClickMode == ClickMode.Hover)
        return;

      StartTimer();
    }


    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);

      if (ClickMode == ClickMode.Hover)
        return;

      StopTimer();
    }

    private void OnTimeout(object sender, EventArgs e)
    {
      var timeSpan = TimeSpan.FromMilliseconds(Interval);

      if (_timer.Interval != timeSpan)
        _timer.Interval = timeSpan;

      if (!IsPressed)
        return;

      RaiseClick();
    }

    private void StartTimer()
    {
      if (_timer == null)
      {
        _timer = new DispatcherTimer();
        _timer.Tick += OnTimeout;
      }
      else if (_timer.IsEnabled)
        return;

      _timer.Interval = TimeSpan.FromMilliseconds(Delay);
      _timer.Start();
    }

    private void StopTimer()
    {
      _timer?.Stop();
    }

    #endregion
  }
}