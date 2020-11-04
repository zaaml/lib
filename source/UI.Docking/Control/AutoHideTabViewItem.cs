// <copyright file="AutoHideTabViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Docking
{
  [TemplateContractType(typeof(AutoHideTabViewItemTemplateContract))]
  public sealed class AutoHideTabViewItem : TemplateContractControl, ISelectionStateControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty DockSideProperty = DPM.Register<Dock, AutoHideTabViewItem>
      ("DockSide", a => a.OnDockSideChanged);

    private static readonly PropertyPath PropertyPath = new PropertyPath(AutoHideLayout.DockSideProperty);

    private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(200);

    #endregion

    #region Fields

    private readonly DelayAction _hideAction;
    private readonly DelayAction _showAction;
    private bool _isOpen;

    #endregion

    #region Ctors

    public AutoHideTabViewItem(DockItem dockItem)
    {
      DockItem = dockItem;

      ItemPresenter = new AutoHideDockItemPresenter(this)
      {
        Visibility = Visibility.Collapsed
      };

      SetBinding(DockSideProperty, new Binding {Path = PropertyPath, Source = dockItem});

      dockItem.IsSelectedChanged += OnIsSelectedChanged;

      _showAction = new DelayAction(Show);
      _hideAction = new DelayAction(Hide);

      DragOutBehavior = new DragOutBehavior
      {
        DragOutCommand = new RelayCommand(OnDragOutCommandExecuted),
        ProcessHandledEvents = true,
        Target = this
      };
    }

    #endregion

    #region Properties

    public AutoHideTabViewControl AutoHideTabViewControl { get; internal set; }

    public DockItem DockItem { get; }

    public Dock DockSide
    {
      get => (Dock) GetValue(DockSideProperty);
      set => SetValue(DockSideProperty, value);
    }

    [UsedImplicitly]
    private DragOutBehavior DragOutBehavior { get; }

    public bool IsOpen
    {
      get => _isOpen;
      set
      {
        if (_isOpen == value)
          return;

        _isOpen = value;

        ItemPresenter.Visibility = _isOpen ? Visibility : Visibility.Collapsed;

        if (_isOpen)
          Panel.SetZIndex(ItemPresenter, ZIndex++);

        UpdateVisualState(true);
      }
    }

    internal AutoHideDockItemPresenter ItemPresenter { get; }

    private static int ZIndex { get; set; }

    #endregion

    #region  Methods

    internal void AttachItem()
    {
      ItemPresenter.AttachItem();
    }

    internal void DetachItem()
    {
      Hide();

      ItemPresenter.DetachItem();
    }

    private void Hide()
    {
      _showAction.Revoke();
      _hideAction.Revoke();

      IsOpen = false;
    }

    private void OnDockSideChanged(Dock oldDockSide, Dock newDockSide)
    {
      ItemPresenter.OnItemDockSideChanged(oldDockSide, newDockSide);
      AutoHideTabViewControl?.OnItemDockSideChanged(this, oldDockSide, newDockSide);
    }

    private void OnDragOutCommandExecuted()
    {
      DockItem.DragOutInternal(DragOutBehavior.OriginMousePosition);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);

      TriggerShow(TimeSpan.Zero);
    }

    private void OnIsSelectedChanged(object sender, EventArgs e)
    {
      IsOpen = DockItem.IsSelected;
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);

      if (ShouldClose())
        TriggerHide(TimeSpan.Zero);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      TriggerShow(Delay);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);

      if (ShouldClose())
        TriggerHide(Delay);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      if (e.Handled)
        return;

      DockItem.SelectAndFocus();
    }

    internal void OnPresenterMouseEnter(AutoHideDockItemPresenter autoHideItemPresenter)
    {
      TriggerShow(Delay);
    }

    internal void OnPresenterMouseLeave(AutoHideDockItemPresenter autoHideItemPresenter)
    {
      if (ShouldClose())
        TriggerHide(Delay);
    }

    private bool ShouldClose()
    {
      return DockItem.IsSelected == false && IsMouseOver == false && ItemPresenter.IsMouseOver == false && FocusHelper.IsKeyboardFocusWithin(this) == false && FocusHelper.IsKeyboardFocusWithin(ItemPresenter) == false;
    }

    private void Show()
    {
      _showAction.Revoke();
      _hideAction.Revoke();

      IsOpen = true;
    }

    private void TriggerHide(TimeSpan timeSpan)
    {
      _showAction.Revoke();
      _hideAction.Invoke(timeSpan);
    }

    private void TriggerShow(TimeSpan timeSpan)
    {
      _hideAction.Revoke();
      _showAction.Invoke(timeSpan);
    }

    #endregion

    #region Interface Implementations

    #region ISelectionStateControl

    bool ISelectionStateControl.IsSelected => IsOpen;

    #endregion

    #endregion
  }

  public sealed class AutoHideTabViewItemTemplateContract : TemplateContract
  {
  }
}