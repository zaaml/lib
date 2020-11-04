// <copyright file="DockItemHeaderPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Docking
{
  [TemplateContractType(typeof(DockItemHeaderPresenterTemplateContract))]
  public class DockItemHeaderPresenter : TemplateContractControl, ISelectionStateControl, IActiveStateControl
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey DockItemPropertyKey = DPM.RegisterReadOnly<DockItem, DockItemHeaderPresenter>
      ("DockItem", p => p.OnDockItemChanged);

    public static readonly DependencyProperty DockItemProperty = DockItemPropertyKey.DependencyProperty;

    private static readonly DependencyProperty FloatingWindowProperty = DPM.Register<FloatingDockWindow, DockItemHeaderPresenter>
      ("FloatingWindow", p => p.OnFloatingWindowChanged);

    public static readonly DependencyProperty IsAutoHideStateProperty = DPM.Register<bool, DockItemHeaderPresenter>
      ("IsAutoHideState", p => p.OnIsAutoHideStateChanged);

    #endregion

    #region Fields

    private DockItem _floatingWindowDockItem;
    private DockItem _ownItem;

    private bool _suspendIsAutoHideState;

    #endregion

    #region Ctors

    static DockItemHeaderPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DockItemHeaderPresenter>();
    }

    public DockItemHeaderPresenter()
    {
      this.OverrideStyleKey<DockItemHeaderPresenter>();

      var commandBindings = CommandBindings;

      commandBindings.Add(new CommandBinding(SetDockStateCommand,
        (s, e) =>
        {
          OnSetDockStateCommandExecuted((DockItemState) e.Parameter);
          e.Handled = true;
        },
        (s, e) =>
        {
          e.CanExecute = OnCanExecuteSetDockStateCommand((DockItemState) e.Parameter);
          e.ContinueRouting = false;
          e.Handled = true;
        }));
    }

    #endregion

    #region Properties

    public DockItem DockItem
    {
      get => (DockItem) GetValue(DockItemProperty);
      private set => this.SetReadOnlyValue(DockItemPropertyKey, value);
    }

    internal FloatingDockWindow FloatingWindow
    {
      get => (FloatingDockWindow) GetValue(FloatingWindowProperty);
      set => SetValue(FloatingWindowProperty, value);
    }

    private DockItem FloatingWindowDockItem
    {
      get => _floatingWindowDockItem;
      set
      {
        if (ReferenceEquals(_floatingWindowDockItem, value))
          return;

        var groupItem = _floatingWindowDockItem as DockItemGroup;

        if (groupItem != null)
          groupItem.SelectedItemChanged -= OnSelectedItemChanged;

        _floatingWindowDockItem = value;

        groupItem = _floatingWindowDockItem as DockItemGroup;

        if (groupItem != null)
          groupItem.SelectedItemChanged += OnSelectedItemChanged;

        UpdateActualItem();
      }
    }

    private bool IsActive
    {
      get
      {
        if (FloatingWindow != null && FloatingWindow.IsVisible)
          return FloatingWindow.IsActive;

        return ((IActiveStateControl) DockItem)?.IsActive ?? false;
      }
    }

    public bool IsAutoHideState
    {
      get => (bool) GetValue(IsAutoHideStateProperty);
      set => SetValue(IsAutoHideStateProperty, value);
    }

    private bool IsSelected
    {
      get
      {
        if (FloatingWindow != null && FloatingWindow.IsVisible)
          return FloatingWindow.IsActive;

        return ((ISelectionStateControl) DockItem)?.IsSelected ?? false;
      }
    }

    internal DockItem OwnItem
    {
      get => _ownItem;
      set
      {
        if (ReferenceEquals(_ownItem, value))
          return;

        var groupItem = _ownItem as DockItemGroup;

        if (groupItem != null)
          groupItem.SelectedItemChanged -= OnSelectedItemChanged;

        _ownItem = value;

        groupItem = _ownItem as DockItemGroup;

        if (groupItem != null)
          groupItem.SelectedItemChanged += OnSelectedItemChanged;

        UpdateActualItem();
      }
    }

    public static RoutedUICommand SetDockStateCommand { get; } = new RoutedUICommand();

    #endregion

    #region  Methods

    private bool OnCanExecuteSetDockStateCommand(DockItemState dockItemState)
    {
      return DockItem != null && DockItem.CanSetDockState(dockItemState);
    }

    private void OnDockItemChanged(DockItem oldItem, DockItem newItem)
    {
      if (oldItem != null)
      {
        oldItem.RequestUpdateVisualState -= OnItemRequestUpdateVisualState;
        oldItem.DockStateChanged -= OnDockStateChanged;
      }

      if (newItem != null)
      {
        newItem.RequestUpdateVisualState += OnItemRequestUpdateVisualState;
        newItem.DockStateChanged += OnDockStateChanged;
      }

      UpdateIsAutoHideState();
      UpdateVisualState(true);
    }

    private void OnDockStateChanged(object sender, DockItemStateChangedEventArgs e)
    {
      UpdateIsAutoHideState();
    }

    private void OnFloatingWindowChanged(FloatingDockWindow oldWindow, FloatingDockWindow newWindow)
    {
      if (oldWindow != null)
      {
        oldWindow.DockItemChanged -= OnFloatingWindowDockItemChanged;
        oldWindow.IsActiveChanged -= OnFloatingWindowIsActiveChanged;

        FloatingWindowDockItem = null;
      }

      if (newWindow != null)
      {
        newWindow.DockItemChanged += OnFloatingWindowDockItemChanged;
        newWindow.IsActiveChanged += OnFloatingWindowIsActiveChanged;

        FloatingWindowDockItem = newWindow.DockItem;
      }

      UpdateActualItem();
    }

    private void OnFloatingWindowDockItemChanged(object sender, EventArgs e)
    {
      FloatingWindowDockItem = FloatingWindow.DockItem;
    }

    private void OnFloatingWindowIsActiveChanged(object sender, EventArgs e)
    {
      UpdateVisualState(true);
    }

    private void OnIsAutoHideStateChanged()
    {
      if (_suspendIsAutoHideState || DockItem == null)
        return;

      SetDockState(IsAutoHideState ? DockItemState.AutoHide : DockItemState.Dock);
    }

    private void OnItemRequestUpdateVisualState(object sender, EventArgs e)
    {
      UpdateVisualState(true);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      if (e.Handled)
        return;

      DockItem?.SelectAndFocus();
    }

    private void OnSelectedItemChanged(object sender, EventArgs e)
    {
      UpdateActualItem();
    }

    private void OnSetDockStateCommandExecuted(DockItemState dockItemState)
    {
      SetDockState(dockItemState);
    }

    private void SetDockState(DockItemState dockItemState)
    {
      if (DockItem == null)
        return;

      Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
      {
        DockItem.DockState = dockItemState;
        DockItem.SelectAndFocus();
      });
    }

    private void UpdateActualItem()
    {
      var actualDockItem = FloatingWindowDockItem ?? OwnItem;
      var actualGroupItem = actualDockItem as DockItemGroup;

      DockItem = actualGroupItem?.SelectedItem ?? actualDockItem;
    }

    private void UpdateIsAutoHideState()
    {
      _suspendIsAutoHideState = true;

      IsAutoHideState = DockItem != null && DockItem.DockState == DockItemState.AutoHide;

      _suspendIsAutoHideState = false;
    }

    #endregion

    #region Interface Implementations

    #region IActiveStateControl

    bool IActiveStateControl.IsActive => IsActive;

    #endregion

    #region ISelectionStateControl

    bool ISelectionStateControl.IsSelected => IsSelected;

    #endregion

    #endregion
  }

  public class DockItemHeaderPresenterTemplateContract : TemplateContract
  {
  }
}