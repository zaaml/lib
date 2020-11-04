// <copyright file="NotifyIcon.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zaaml.Core;
using Zaaml.Platform;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.TaskBar
{
  public class NotifyIcon : DependencyObject
  {
    #region Static Fields and Constants

    // ReSharper disable once InconsistentNaming
    private const int WM_NOTIFYICON = (int) WM.USER + 100;

    public static readonly DependencyProperty BalloonTipIconProperty = DPM.Register<ImageSource, NotifyIcon>
      ("BalloonTipIcon");

    public static readonly DependencyProperty IconProperty = DPM.Register<ImageSource, NotifyIcon>
      ("Icon", n => n.OnIconChanged);

    public static readonly DependencyProperty BalloonTipTextProperty = DPM.Register<string, NotifyIcon>
      ("BalloonTipText");

    public static readonly DependencyProperty BalloonTipTitleProperty = DPM.Register<string, NotifyIcon>
      ("BalloonTipTitle");

    public static readonly DependencyProperty TooltipProperty = DPM.Register<string, NotifyIcon>
      ("Tooltip", n => n.OnTooltipChanged);

    public static readonly DependencyProperty InfoProperty = DPM.Register<string, NotifyIcon>
      ("Info", n => n.OnInfoChanged);

    public static readonly DependencyProperty InfoTitleProperty = DPM.Register<string, NotifyIcon>
      ("InfoTitle", n => n.OnInfoTitleChanged);

    public static readonly DependencyProperty IsVisibleProperty = DPM.Register<bool, NotifyIcon>
      ("IsVisible");

    #endregion

    #region Fields

    [UsedImplicitly] private readonly WindowDispatcher _dispatcher;

    [UsedImplicitly] private NOTIFYICONDATA _notifyIconData;

    public event EventHandler MouseLeftButtonDown;
    public event EventHandler MouseRightButtonDown;
    public event EventHandler MouseLeftButtonUp;
    public event EventHandler MouseRightButtonUp;

    #endregion

    #region Ctors

    public NotifyIcon()
    {
      _dispatcher = new WindowDispatcher(this);

      _notifyIconData = NOTIFYICONDATA.Default;
      _notifyIconData.hwnd = _dispatcher.Handle;
      _notifyIconData.uFlags = (int) (NIF.TIP | NIF.MESSAGE);
      _notifyIconData.szTip = string.Empty;
      _notifyIconData.szInfo = string.Empty;
      _notifyIconData.szInfoTitle = string.Empty;
      _notifyIconData.dwState = 2;
      _notifyIconData.uCallbackMessage = WM_NOTIFYICON;

      NativeMethods.Shell_NotifyIcon(NIM.ADD, ref _notifyIconData);
    }

    #endregion

    #region Properties

    public ImageSource BalloonTipIcon
    {
      get => (ImageSource) GetValue(BalloonTipIconProperty);
      set => SetValue(BalloonTipIconProperty, value);
    }

    public string BalloonTipText
    {
      get => (string) GetValue(BalloonTipTextProperty);
      set => SetValue(BalloonTipTextProperty, value);
    }

    public string BalloonTipTitle
    {
      get => (string) GetValue(BalloonTipTitleProperty);
      set => SetValue(BalloonTipTitleProperty, value);
    }

    public ImageSource Icon
    {
      get => (ImageSource) GetValue(IconProperty);
      set => SetValue(IconProperty, value);
    }

    public string Info
    {
      get => (string) GetValue(InfoProperty);
      set => SetValue(InfoProperty, value);
    }

    public string InfoTitle
    {
      get => (string) GetValue(InfoTitleProperty);
      set => SetValue(InfoTitleProperty, value);
    }

    public bool IsVisible
    {
      get => (bool) GetValue(IsVisibleProperty);
      set => SetValue(IsVisibleProperty, value);
    }

    internal IntPtr NativeHandle => _dispatcher.Handle;

    public string Tooltip
    {
      get => (string) GetValue(TooltipProperty);
      set => SetValue(TooltipProperty, value);
    }

    #endregion

    #region  Methods

    private static IntPtr ConvertToBitmap(BitmapSource bitmapSource)
    {
      var width = bitmapSource.PixelWidth;
      var height = bitmapSource.PixelHeight;

      var nativeBitmap = NativeBitmap.Create(bitmapSource);

      var hbmMask = NativeMethods.CreateCompatibleBitmap(NativeMethods.GetDC(IntPtr.Zero), width, height);

      var iconInfo = new ICONINFO
      {
        fIcon = true,
        hbmColor = nativeBitmap.Handle,
        hbmMask = hbmMask
      };

      var hIcon = NativeMethods.CreateIconIndirect(ref iconInfo);

      NativeMethods.DeleteObject(hbmMask);

      return hIcon;
    }

    private void Dispose()
    {
      NativeMethods.Shell_NotifyIcon(NIM.DELETE, ref _notifyIconData);
    }

    private void EnterContextMenu()
    {
      NativeMethods.NotifyWinEvent(EVENT.SYSTEM_MENUPOPUPSTART, NativeHandle, OBJID.CLIENT, OBJID.CHILDID_SELF);
    }

    private void ExitContextMenu()
    {
      InputManager.Current.PopMenuMode(_dispatcher);
      NativeMethods.NotifyWinEvent(EVENT.SYSTEM_MENUPOPUPEND, NativeHandle, OBJID.CLIENT, OBJID.CHILDID_SELF);
    }

    private void OnIconChanged()
    {
      var bitmap = Icon as BitmapSource;

      if (_notifyIconData.hIcon != IntPtr.Zero)
        NativeMethods.DeleteObject(_notifyIconData.hIcon);

      if (bitmap != null)
      {
        _notifyIconData.hIcon = ConvertToBitmap(bitmap);
        _notifyIconData.uFlags |= (int) NIF.ICON;
      }
      else
      {
        _notifyIconData.hIcon = IntPtr.Zero;
        _notifyIconData.uFlags &= ~(int) NIF.ICON;
      }

      UpdateNotifyIcon();
    }

    private void OnInfoChanged()
    {
      _notifyIconData.szInfo = Info ?? string.Empty;

      UpdateNotifyIcon();
    }

    private void OnInfoTitleChanged()
    {
      _notifyIconData.szInfoTitle = InfoTitle ?? string.Empty;

      UpdateNotifyIcon();
    }

    private void OnMouseButton()
    {
    }

    private void OnMouseLeftButtonDown()
    {
      OnMouseButton();

      MouseLeftButtonDown?.Invoke(this, EventArgs.Empty);
    }

    private void OnMouseLeftButtonUp()
    {
      OnMouseButton();

      MouseLeftButtonUp?.Invoke(this, EventArgs.Empty);
    }

    private void OnMouseMove()
    {
    }

    private void OnMouseRightButtonDown()
    {
      OnMouseButton();

      MouseRightButtonDown?.Invoke(this, EventArgs.Empty);
    }

    private void OnMouseRightButtonUp()
    {
      OnMouseButton();

      MouseRightButtonUp?.Invoke(this, EventArgs.Empty);
    }

    private void OnTooltipChanged()
    {
      _notifyIconData.szTip = Tooltip ?? string.Empty;

      UpdateNotifyIcon();
    }

    public void ShowContextPopupControl(IContextPopupControl contextPopup)
    {
      ShowContextPopupControlImpl(contextPopup, false);
    }

    private void ShowContextPopupControlImpl(IContextPopupControl contextPopup, bool modal)
    {
      var cancellationToken = new ModalStateCancellationToken();

      contextPopup.IsOpen = true;

      EventHandler closedHandler = null;
      closedHandler = (sender, args) =>
      {
        if (contextPopup.IsOpen)
          return;

        cancellationToken.Cancel();

        contextPopup.IsOpenChanged -= closedHandler;

        if (modal) return;

        ExitContextMenu();
      };

      if (contextPopup.IsOpen == false)
        return;

      contextPopup.IsOpenChanged += closedHandler;

      EnterContextMenu();

      if (modal == false)
        return;

      ModalState.Enter(cancellationToken);
      ExitContextMenu();
    }

    internal void ShowContextPopupControlModal(IContextPopupControl contextPopup)
    {
      ShowContextPopupControlImpl(contextPopup, true);
    }

    private void UpdateNotifyIcon()
    {
      NativeMethods.Shell_NotifyIcon(NIM.MODIFY, ref _notifyIconData);
    }

    #endregion

    #region  Nested Types

    private sealed class WindowDispatcher : HwndSource
    {
      #region Fields

      private readonly NotifyIcon _notifyIcon;

      #endregion

      #region Ctors

      public WindowDispatcher(NotifyIcon notifyIcon) : base(new HwndSourceParameters("NotifyIconMouseWindow") {WindowStyle = 0})
      {
        _notifyIcon = notifyIcon;
        AddHook(OnMessage);

        RootVisual = new Border();
      }

      #endregion

      #region  Methods

      private IntPtr OnMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
      {
        if ((WM) msg == WM.DESTROY)
          _notifyIcon.Dispose();

        if (msg != WM_NOTIFYICON)
          return IntPtr.Zero;

        var nmsg = (WM) lparam;

        switch (nmsg)
        {
          case WM.MOUSEMOVE:
            _notifyIcon.OnMouseMove();
            break;
          case WM.LBUTTONDOWN:
            _notifyIcon.OnMouseLeftButtonDown();
            break;
          case WM.LBUTTONUP:
            _notifyIcon.OnMouseLeftButtonUp();
            break;
          case WM.RBUTTONDOWN:
            _notifyIcon.OnMouseRightButtonDown();
            break;
          case WM.RBUTTONUP:
            _notifyIcon.OnMouseRightButtonUp();
            break;
        }

        return IntPtr.Zero;
      }

      #endregion
    }

    #endregion
  }
}