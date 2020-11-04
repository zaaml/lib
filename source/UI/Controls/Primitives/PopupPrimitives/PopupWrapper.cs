// <copyright file="PopupWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal class PopupWrapper : DependencyObject, IManagedPopupControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty PlacementProperty = DPM.Register<PopupPlacement, PopupWrapper>
      ("Placement", c => c.OnPlacementChanged);

    public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, PopupWrapper>
      ("IsOpen", c => c.OnIsOpenChangedInt);

    #endregion

    #region Fields

    private IPopup _popup;
    private bool _suspendHandle;

    #endregion

    #region Ctors

    public PopupWrapper(IPopup popup)
    {
      _popup = popup;

      AttachEvents();
      SyncWrapper();
    }

    public PopupWrapper()
    {
    }

    #endregion

    #region Properties

    public IPopup Popup
    {
      get => _popup;
      set
      {
        if (ReferenceEquals(_popup, value))
          return;

        if (_popup != null)
        {
          DetachEvents();
          CleanPopup();
        }

        _popup = value;

        if (_popup != null)
        {
          AttachEvents();
          Sync();
        }
      }
    }

    public PopupWrapperSyncDirection SyncDirection { get; set; }

    #endregion

    #region  Methods

    private void AttachEvents()
    {
      _popup.IsOpenChanged += PopupOnIsOpenChanged;
      _popup.PlacementChanged += PopupOnPlacementChanged;
    }

    private void CleanPopup()
    {
      if (_popup == null)
        return;

      try
      {
        _suspendHandle = true;

        if (ReferenceEquals(_popup.Placement, this.GetValue<PopupPlacement>(PlacementProperty)))
          _popup.Placement = null;

        _popup.IsOpen = false;
      }
      finally
      {
        _suspendHandle = false;
      }
    }

    private void DetachEvents()
    {
      _popup.IsOpenChanged -= PopupOnIsOpenChanged;
      _popup.PlacementChanged -= PopupOnPlacementChanged;
    }

    private void OnIsOpenChangedInt()
    {
      if (_suspendHandle || _popup == null) return;
      try
      {
        _suspendHandle = false;
        _popup.IsOpen = this.GetValue<bool>(IsOpenProperty);
      }
      finally
      {
        _suspendHandle = false;
      }
    }

    private void OnPlacementChanged()
    {
      if (_suspendHandle || _popup == null) return;
      try
      {
        _suspendHandle = false;
        _popup.Placement = this.GetValue<PopupPlacement>(PlacementProperty);
      }
      finally
      {
        _suspendHandle = false;
      }
    }

    private void PopupOnIsOpenChanged(object sender, EventArgs eventArgs)
    {
      if (_suspendHandle || _popup == null) return;
      try
      {
        _suspendHandle = false;
        SetValue(IsOpenProperty, _popup.IsOpen);
      }
      finally
      {
        _suspendHandle = false;
      }
    }

    private void PopupOnPlacementChanged(object sender, EventArgs eventArgs)
    {
      if (_suspendHandle || _popup == null) return;
      try
      {
        _suspendHandle = false;
        SetValue(PlacementProperty, _popup.Placement);
      }
      finally
      {
        _suspendHandle = false;
      }
    }

    protected virtual void Sync()
    {
      switch (SyncDirection)
      {
        case PopupWrapperSyncDirection.None:
          return;
        case PopupWrapperSyncDirection.SyncWrapper:
          SyncWrapper();
          break;
        case PopupWrapperSyncDirection.SyncPopup:
          SyncPopup();
          break;
      }
    }

    public void SyncPopup()
    {
      if (_popup == null)
        return;

      try
      {
        _suspendHandle = true;

        _popup.Placement = this.GetValue<PopupPlacement>(PlacementProperty);
        _popup.IsOpen = this.GetValue<bool>(IsOpenProperty);
      }
      finally
      {
        _suspendHandle = false;
      }
    }

    public void SyncWrapper()
    {
      if (_popup == null)
        return;

      try
      {
        _suspendHandle = true;

        SetValue(PlacementProperty, _popup.Placement);
        SetValue(IsOpenProperty, _popup.IsOpen);
      }
      finally
      {
        _suspendHandle = false;
      }
    }

    #endregion

    #region Interface Implementations

    #region IManagedPopupControl

    void IManagedPopupControl.OnClosing(PopupCancelEventArgs e)
    {
    }

    void IManagedPopupControl.OnOpening(PopupCancelEventArgs e)
    {
    }

    void IManagedPopupControl.OnOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner)
    {
    }

    void IManagedPopupControl.OnPlacementChanged(PopupPlacement oldPlacement, PopupPlacement newPlacement)
    {
    }

    void IManagedPopupControl.OnOpened()
    {
    }

    void IManagedPopupControl.OnClosed()
    {
    }

    void IManagedPopupControl.OnIsOpenChanged()
    {
    }

    DependencyProperty IManagedPopupControl.IsOpenProperty => IsOpenProperty;

    DependencyPropertyKey IManagedPopupControl.OwnerPropertyKey => null;

    DependencyProperty IManagedPopupControl.PlacementProperty => PlacementProperty;

    #endregion

    #endregion
  }
}