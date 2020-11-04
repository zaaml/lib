// <copyright file="SelectableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Behaviors.Selectable
{
  internal static class SelectableBehavior
  {
    #region Static Fields and Constants

    private static readonly BoolSuspender SkipChangedSuspender = new BoolSuspender();

    public static readonly DependencyProperty IsEnabledProperty = DPM.RegisterAttached<bool>
      ("IsEnabled", typeof(SelectableBehavior), OnIsEnabledPropertyChanged);

    public static readonly DependencyProperty SelectTargetProperty = DPM.RegisterAttached<DependencyObject>
      ("SelectTarget", typeof(SelectableBehavior));

    public static readonly DependencyProperty SelectionScopeProperty = DPM.RegisterAttached<SelectionScope>
      ("SelectionScope", typeof(SelectableBehavior), SelectionScopePropertyChanged);

    public static readonly DependencyProperty IsSelectedProperty = DPM.RegisterAttached<bool>
      ("IsSelected", typeof(SelectableBehavior), OnIsSelectedPropertyChanged);

    public static readonly DependencyProperty NonSelectableProperty = DPM.RegisterAttached<bool>
      ("NonSelectable", typeof(SelectableBehavior));

    private static readonly Dictionary<object, SelectionScope> GeneratedScopes = new Dictionary<object, SelectionScope>();

    #endregion

    #region  Methods

    private static void Deinitialize(UIElement element)
    {
#if !SILVERLIGHT
      element.RemoveHandler(Mouse.MouseDownEvent, (MouseButtonEventHandler) OnMouseDown);
#else
      element.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnMouseDown);
#endif
    }

    public static bool GetIsEnabled(DependencyObject element)
    {
      return (bool) element.GetValue(IsEnabledProperty);
    }

    public static bool GetIsSelected(DependencyObject element)
    {
      return (bool) element.GetValue(IsSelectedProperty);
    }

    public static bool GetNonSelectable(DependencyObject element)
    {
      return (bool) element.GetValue(NonSelectableProperty);
    }

    public static SelectionScope GetOrCreateSelectionScopeByKey(object key)
    {
      return key.Return(k => GeneratedScopes.GetValueOrCreate(k, () => new SelectionScope()));
    }

    public static SelectionScope GetSelectionScope(DependencyObject element)
    {
      return (SelectionScope) element.GetValue(SelectionScopeProperty);
    }

    public static DependencyObject GetSelectTarget(DependencyObject element)
    {
      return (DependencyObject) element.GetValue(SelectTargetProperty);
    }

    private static void Initialize(UIElement element)
    {
#if !SILVERLIGHT
      element.AddHandler(Mouse.MouseDownEvent, (MouseButtonEventHandler) OnMouseDown, true);
#else
      element.AddHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnMouseDown, true);
#endif
    }

    private static void OnIsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var element = (UIElement) d;

      if ((bool) e.OldValue)
        Deinitialize(element);

      if ((bool) e.NewValue)
        Initialize(element);
    }

    private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (SkipChangedSuspender.IsSuspended)
        return;

      var selectionScope = GetSelectionScope(d);

      if (selectionScope == null)
        return;

      var value = (bool) e.NewValue;

      var selectionTarget = GetSelectTarget(d) ?? d;
      if (value)
        selectionScope.SelectedItem = d;
      else if (ReferenceEquals(selectionScope.SelectedItem, selectionTarget))
        selectionScope.SelectedItem = null;
    }

    private static void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.Handled && e.ClickCount != 1)
        return;

      var element = (DependencyObject) sender;
      var uieSource = PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource);

      if (uieSource.GetVisualAncestors().TakeWhile(p => !ReferenceEquals(p, element)).Any(GetNonSelectable))
        return;

      var selectionScope = GetSelectionScope(element);

      if (selectionScope == null)
        return;

      selectionScope.SelectedItem = GetSelectTarget(element) ?? element;
    }

    private static void SelectionScopePropertyChanged(DependencyObject d, SelectionScope oldSelectionScope, SelectionScope newSelectionScope)
    {
      if (oldSelectionScope != null && ReferenceEquals(oldSelectionScope.SelectedItem, d))
        SetIsSelectedInt(d, false);

      if (newSelectionScope != null && ReferenceEquals(newSelectionScope.SelectedItem, d))
        SetIsSelectedInt(d, true);
    }

    public static void SetIsEnabled(DependencyObject element, bool value)
    {
      element.SetValue(IsEnabledProperty, value);
    }

    public static void SetIsSelected(DependencyObject element, bool value)
    {
      element.SetValue(IsSelectedProperty, value);
    }

    internal static void SetIsSelectedInt(DependencyObject element, bool value)
    {
      using (BoolSuspender.Suspend(SkipChangedSuspender))
        element.SetValue(IsSelectedProperty, value);
    }

    public static void SetNonSelectable(DependencyObject element, bool value)
    {
      element.SetValue(NonSelectableProperty, value);
    }

    public static void SetSelectionScope(DependencyObject element, SelectionScope value)
    {
      element.SetValue(SelectionScopeProperty, value);
    }

    public static void SetSelectTarget(DependencyObject element, DependencyObject value)
    {
      element.SetValue(SelectTargetProperty, value);
    }

    #endregion
  }
}