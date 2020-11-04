// <copyright file="WindowHeaderButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
  public sealed class WindowHeaderButton : WindowButton
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty KindProperty = DPM.Register<WindowHeaderButtonKind, WindowHeaderButton>
      ("Kind", b => b.OnKindChanged);

    #endregion

    #region Ctors

    static WindowHeaderButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<WindowHeaderButton>();
    }

    public WindowHeaderButton()
    {
      this.OverrideStyleKey<WindowHeaderButton>();
      SetCommand();
    }

    #endregion

    #region Properties

    public WindowHeaderButtonKind Kind
    {
      get => (WindowHeaderButtonKind) GetValue(KindProperty);
      set => SetValue(KindProperty, value);
    }

    #endregion

    #region  Methods

    private void OnKindChanged(WindowHeaderButtonKind oldKind, WindowHeaderButtonKind newKind)
    {
      SetCommand();

      UpdateVisibility();
    }

    private void OnVisibilityRelatedPropertyChanged(object sender, EventArgs eventArgs)
    {
      UpdateVisibility();
    }

    protected override void OnWindowChanged(WindowBase oldWindow, WindowBase newWindow)
    {
      base.OnWindowChanged(oldWindow, newWindow);

      if (oldWindow != null)
        oldWindow.HeaderButtonVisibilityRelatedPropertyChanged -= OnVisibilityRelatedPropertyChanged;

      if (newWindow != null)
        newWindow.HeaderButtonVisibilityRelatedPropertyChanged += OnVisibilityRelatedPropertyChanged;

      CommandTarget = newWindow;

      UpdateVisibility();
    }

    private void SetCommand()
    {
      switch (Kind)
      {
        case WindowHeaderButtonKind.Minimize:
          Command = WindowBase.MinimizeCommand;
          break;
        case WindowHeaderButtonKind.Maximize:
          Command = WindowBase.ToggleMaximizeNormalStateCommand;
          break;
        case WindowHeaderButtonKind.Restore:
          Command = WindowBase.ToggleMaximizeNormalStateCommand;
          break;
        case WindowHeaderButtonKind.Close:
          Command = WindowBase.CloseCommand;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    internal void UpdateVisibility()
    {
      var window = Window;

      if (window == null)
        return;

      switch (Kind)
      {
        case WindowHeaderButtonKind.Minimize:
          Visibility = window.ActualShowMinimizeButtonInt.ToVisibility();
          break;
        case WindowHeaderButtonKind.Maximize:
          Visibility = window.ActualShowMaximizeButtonInt.ToVisibility();
          break;
        case WindowHeaderButtonKind.Restore:
          Visibility = window.ActualShowRestoreButtonInt.ToVisibility();
          break;
        case WindowHeaderButtonKind.Close:
          Visibility = window.ActualShowCloseButtonInt.ToVisibility();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion
  }
}