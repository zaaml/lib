// <copyright file="CheckedPopupBarItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public class CheckedPopupBarItem : PopupBarItem, IToggleButton
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsCheckedProperty = DPM.Register<bool?, CheckedPopupBarItem>
      ("IsChecked", false, c => c.UpdateVisualState(true));

    public static readonly DependencyProperty IsThreeStateProperty = DPM.Register<bool, CheckedPopupBarItem>
      ("IsThreeState");

    public static readonly DependencyProperty IsReadOnlyProperty = DPM.Register<bool, CheckedPopupBarItem>
      ("IsReadOnly");

    public static readonly DependencyProperty SubBarOpenModeProperty = DPM.Register<CheckedPopupBarOpenMode, CheckedPopupBarItem>
      ("SubBarOpenMode", CheckedPopupBarOpenMode.Checked);

    #endregion

    #region Ctors

    static CheckedPopupBarItem()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<CheckedPopupBarItem>();
    }

    public CheckedPopupBarItem()
    {
      this.OverrideStyleKey<CheckedPopupBarItem>();
    }

    #endregion

    #region Properties

    public bool IsReadOnly
    {
      get => (bool) GetValue(IsReadOnlyProperty);
      set => SetValue(IsReadOnlyProperty, value.Box());
    }

    public CheckedPopupBarOpenMode SubBarOpenMode
    {
      get => (CheckedPopupBarOpenMode) GetValue(SubBarOpenModeProperty);
      set => SetValue(SubBarOpenModeProperty, value);
    }

    #endregion

    #region  Methods

    protected override bool CoerceIsSubBarOpen(bool isSubBarOpen)
    {
      if (isSubBarOpen == false)
        return false;

      var openMode = SubBarOpenMode;

      if (IsChecked == true)
        return (openMode & CheckedPopupBarOpenMode.Checked) != 0;

      if (IsChecked == false)
        return (openMode & CheckedPopupBarOpenMode.Unchecked) != 0;

      if (IsChecked == null)
        return (openMode & CheckedPopupBarOpenMode.Indeterminate) != 0;

      return true;
    }

    protected virtual void OnChecked()
    {
      Checked?.Invoke(this, RoutedEventArgsFactory.Create(this));
    }

    protected override void OnClick()
    {
      if (IsReadOnly == false)
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
      get => (bool) GetValue(IsThreeStateProperty);
      set => SetValue(IsThreeStateProperty, value.Box());
    }

    DependencyProperty IToggleButton.IsCheckedPropertyInt => IsCheckedProperty;

    #endregion

    #endregion
  }

  [Flags]
  public enum CheckedPopupBarOpenMode
  {
    Checked = 1,
    Unchecked = 2,
    Indeterminate = 4
  }
}