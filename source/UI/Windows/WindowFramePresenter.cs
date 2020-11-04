// <copyright file="WindowFramePresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Windows
{
  public sealed class WindowFramePresenter : ContentControl, IWindowElement, IWindowEventListener
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ActualFrameStylePropertyKey = DPM.RegisterReadOnly<WindowFrameStyle, WindowFramePresenter>
      ("ActualFrameStyle", WindowFrameStyle.Border);

    public static readonly DependencyProperty ActualFrameStyleProperty = ActualFrameStylePropertyKey.DependencyProperty;

    public static readonly DependencyProperty DropShadowProperty = DPM.Register<bool, WindowFramePresenter>
      ("DropShadow", false);

    public static readonly DependencyProperty FrameStyleProperty = DPM.Register<WindowFrameStyle, WindowFramePresenter>
      ("FrameStyle", WindowFrameStyle.Border, w => w.OnFrameStyleChanged);

    private static readonly DependencyPropertyKey IsResizingPropertyKey = DPM.RegisterReadOnly<bool, WindowFramePresenter>
      ("IsResizing");

    public static readonly DependencyProperty IsResizingProperty = IsResizingPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private IWindow _window;

    #endregion

    #region Ctors

    static WindowFramePresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<WindowFramePresenter>();
    }

    public WindowFramePresenter()
    {
      IsTabStop = false;

      this.OverrideStyleKey<WindowFramePresenter>();
    }

    #endregion

    #region Properties

    public WindowFrameStyle ActualFrameStyle
    {
      get => (WindowFrameStyle) GetValue(ActualFrameStyleProperty);
      private set => this.SetReadOnlyValue(ActualFrameStylePropertyKey, value);
    }

    public bool DropShadow
    {
      get => (bool) GetValue(DropShadowProperty);
      set => SetValue(DropShadowProperty, value);
    }

    public WindowFrameStyle FrameStyle
    {
      get => (WindowFrameStyle) GetValue(FrameStyleProperty);
      set => SetValue(FrameStyleProperty, value);
    }

    private bool IsMaximized => Window?.WindowState == WindowState.Maximized;

    public bool IsResizing
    {
      get => (bool) GetValue(IsResizingProperty);
      private set => this.SetReadOnlyValue(IsResizingPropertyKey, value);
    }

    private IWindow Window
    {
      get => _window;
      set
      {
        if (ReferenceEquals(_window, value))
          return;

        if (_window != null)
        {
          _window.StateChanged -= WindowOnStateChanged;
          _window.IsActiveChanged -= WindowOnIsActiveChanged;
        }

        _window = value;

        if (_window != null)
        {
          _window.StateChanged += WindowOnStateChanged;
          _window.IsActiveChanged += WindowOnIsActiveChanged;
        }
      }
    }

    private bool IsActive => Window?.IsActive == true;

    IWindow IWindowElement.Window
    {
      get => Window;
      set => Window = value;
    }

    #endregion

    #region  Methods

    private void WindowOnIsActiveChanged(object sender, EventArgs e)
    {
      UpdateVisualState(true);
    }

    protected override void UpdateVisualState(bool useTransitions)
    {
      if (IsActive)
				GotoVisualState(CommonVisualStates.Active, useTransitions);
      else
				GotoVisualState(CommonVisualStates.Normal, useTransitions);
    }

    private void OnFrameStyleChanged()
    {
      UpdateActualFrameStyle();
    }

    private void UpdateActualFrameStyle()
    {
      ActualFrameStyle = IsMaximized ? WindowFrameStyle.None : FrameStyle;
    }

    private void WindowOnStateChanged(object sender, EventArgs eventArgs)
    {
      UpdateActualFrameStyle();
    }

    IEnumerable<IWindowElement> IWindowElement.EnumerateWindowElements()
    {
      yield break;
    }

    void IWindowEventListener.OnResizeStarted()
    {
      IsResizing = true;
    }

    void IWindowEventListener.OnResizeFinished()
    {
      IsResizing = false;
    }

    #endregion
  }
}

public enum WindowFrameStyle
{
  None,
  Border
}