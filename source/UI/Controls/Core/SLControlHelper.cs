// <copyright file="SLControlHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Core
{
  internal enum CorePropertyKind
  {
    IsLoaded,
    IsFocused,
    IsMouseOver,
    HasItems
  }

  internal static class SLControlHelper
  {
    #region  Methods

    public static void InitFocusChangedState(System.Windows.Controls.Control control, DependencyPropertyKey isFocusedProperty)
    {
      control.GotFocus += (sender, args) => control.SetValue(isFocusedProperty, BooleanBoxes.True);
      control.LostFocus += (sender, args) => control.SetValue(isFocusedProperty, BooleanBoxes.False);
    }

    public static void InitIsMouseOverState(System.Windows.Controls.Control control, DependencyPropertyKey isMouseOverProperty)
    {
      control.MouseEnter += (sender, args) => control.SetValue(isMouseOverProperty, BooleanBoxes.True);
      control.MouseLeave += (sender, args) => control.SetValue(isMouseOverProperty, BooleanBoxes.False);
    }

    public static void InitLoadUnloadState(System.Windows.Controls.Control control, DependencyPropertyKey isLoadedProperty)
    {
      control.Loaded += (sender, args) => control.SetValue(isLoadedProperty, BooleanBoxes.True);
      control.Unloaded += (sender, args) => control.SetValue(isLoadedProperty, BooleanBoxes.False);
    }

    public static DependencyPropertyKey RegisterCoreProperty<TProperty, TOwner>(CorePropertyKind propertyKind, Func<TOwner, Action> changedHandlerFactory) where TOwner : DependencyObject
    {
      return DependencyPropertyManager.RegisterReadOnly<TProperty, TOwner>(propertyKind.ToString(), changedHandlerFactory);
    }

    #endregion
  }
}