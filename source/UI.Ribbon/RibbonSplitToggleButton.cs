// <copyright file="RibbonSplitToggleButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonSplitToggleButton : RibbonSplitButton, IToggleButton
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsCheckedProperty = DPM.Register<bool?, RibbonSplitToggleButton>
      ("IsChecked", false, c => c.UpdateVisualState(true));

    public static readonly DependencyProperty IsThreeStateProperty = DPM.Register<bool, RibbonSplitToggleButton>
      ("IsThreeState");

    #endregion

    #region Ctors

    static RibbonSplitToggleButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonSplitToggleButton>();
    }

    public RibbonSplitToggleButton()
    {
      this.OverrideStyleKey<RibbonSplitToggleButton>();
    }

    #endregion

    #region  Methods

    protected virtual void OnChecked()
    {
      Checked?.Invoke(this, RoutedEventArgsFactory.Create(this));
    }

    protected override void OnClick()
    {
      this.OnToggle();

      switch (IsChecked)
      {
        case true:
          OnChecked();

          break;
        case false:
          OnUnchecked();

          break;
        default:
          OnIndeterminate();

          break;
      }

      base.OnClick();
    }

    protected virtual void OnIndeterminate()
    {
      Indeterminate?.Invoke(this, RoutedEventArgsFactory.Create(this));
    }

    protected virtual void OnUnchecked()
    {
      Unchecked?.Invoke(this, RoutedEventArgsFactory.Create(this));
    }

    #endregion

    #region Interface Implementations

    #region IToggleButton

    public event RoutedEventHandler Checked;
    public event RoutedEventHandler Unchecked;
    public event RoutedEventHandler Indeterminate;

    public bool? IsChecked
    {
      get => (bool?) GetValue(IsCheckedProperty);
      set => SetValue(IsCheckedProperty, value);
    }

    public bool IsThreeState
    {
      get => this.GetValue<bool>(IsThreeStateProperty);
      set => SetValue(IsThreeStateProperty, value);
    }

    DependencyProperty IToggleButton.IsCheckedPropertyInt => IsCheckedProperty;

    #endregion

    #endregion
  }
}