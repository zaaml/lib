// <copyright file="ContextBarService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public static class ContextBarService
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ContextBarProperty = DPM.RegisterAttached<ContextBar>
    ("ContextBar", typeof(ContextBarService),
      DPM.StaticCallback<FrameworkElement, ContextBar>(OnContextBarPropertyChanged));

    public static readonly DependencyProperty ContextBarSelectorProperty = DPM.RegisterAttached<ContextBarSelector>
    ("ContextBarSelector", typeof(ContextBarService),
      DPM.StaticCallback<FrameworkElement, ContextBarSelector>(OnContextBarSelectorPropertyChanged));

    #endregion

    #region  Methods

    public static ContextBar GetContextBar(DependencyObject element)
    {
      return (ContextBar) element.GetValue(ContextBarProperty);
    }

    public static ContextBarSelector GetContextBarSelector(FrameworkElement element)
    {
      return element.GetValue<ContextBarSelector>(ContextBarSelectorProperty);
    }

    private static void OnContextBarPropertyChanged(FrameworkElement frameworkElement, ContextBar oldBar, ContextBar newBar)
    {
      SharedItemHelper.Share(frameworkElement, oldBar, newBar);
      PopupControlService.OnPopupControllerSelectorChanged(frameworkElement, oldBar?.PopupController, newBar?.PopupController);
    }

    private static void OnContextBarSelectorPropertyChanged(FrameworkElement frameworkElement, ContextBarSelector oldBarSelector, ContextBarSelector newBarSelector)
    {
      SharedItemHelper.Share(frameworkElement, oldBarSelector, newBarSelector);
      PopupControlService.OnPopupControllerSelectorChanged(frameworkElement, oldBarSelector, newBarSelector);
    }

    public static void SetContextBar(DependencyObject element, ContextBar value)
    {
      element.SetValue(ContextBarProperty, value);
    }

    public static void SetContextBarSelector(FrameworkElement element, ContextBarSelector value)
    {
      element.SetValue(ContextBarSelectorProperty, value);
    }

    #endregion
  }
}