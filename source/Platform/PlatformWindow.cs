#if COMPILE_EVER

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Zaaml.Platform
{

  internal class WindowClassEx : IDisposable
  {
    private WNDCLASSEX _wndClassEx;

    public WindowClassEx(string className, WndProc wndProc)
    {
      var color = new COLORREF(0xff, 0xff, 0xff);
      var brush = NativeMethods.CreateSolidBrush(color.Value);

      _wndClassEx = WNDCLASSEX.Default;
      _wndClassEx.hInstance = Process.GetCurrentProcess().Handle;
      _wndClassEx.style = CS.HREDRAW | CS.VREDRAW;
      _wndClassEx.lpfnWndProc = wndProc;
      _wndClassEx.cbClsExtra = 0;
      _wndClassEx.cbWndExtra = 0;
      _wndClassEx.hIcon = IntPtr.Zero;
      _wndClassEx.hCursor = IntPtr.Zero;
      _wndClassEx.hbrBackground = brush;
      _wndClassEx.lpszMenuName = null;
      _wndClassEx.lpszClassName = className;

      NativeMethods.RegisterClassEx(ref _wndClassEx);
    }

    public void Dispose()
    {
      if (_wndClassEx.hInstance == IntPtr.Zero)
        return;

      NativeMethods.UnregisterClass(_wndClassEx.lpszClassName, _wndClassEx.hInstance);

      _wndClassEx.hInstance = IntPtr.Zero;
    }
  }

  internal class PlatformWindow : IDisposable
  {
    private readonly WindowClassEx _windowClassEx;
    private readonly IntPtr _hwnd;

    public PlatformWindow()
    {
      var className = $"Zaaml.Platform_{Guid.NewGuid()}";

      _windowClassEx = new WindowClassEx(className, OnWindowProc);
      _hwnd = NativeMethods.CreateWindowEx(0, className, className, WS.SYSMENU | WS.VISIBLE | WS.THICKFRAME | WS.CAPTION | WS.MAXIMIZEBOX | WS.MINIMIZEBOX | WS.DLGFRAME | WS.BORDER, 500, 300, 600, 500, IntPtr.Zero, IntPtr.Zero, Process.GetCurrentProcess().Handle, IntPtr.Zero);

      MARGINS margins = new MARGINS
      {
        cxLeftWidth = 1,
        cxRightWidth = 1,
        cyBottomHeight = 1,
        cyTopHeight=1
      };
      NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref margins);
    }

    internal IntPtr Handle => _hwnd;

    private IntPtr OnWindowProc(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam)
    {
      var wmMsg = (WM) msg;
      switch (wmMsg)
      {
        case WM.NCCALCSIZE:
          return OnNcCalcSize(wparam, lparam);
        case WM.NCHITTEST:
          return OnNcHitTest(wparam, lparam);
        case WM.NCPAINT:
          OnNcPaint(wparam, lparam);
          break;
        case WM.ERASEBKGND:
          OnEraseBackground(wparam);
          break;
        //case WM.PAINT:
        //	OnPaint();
        //	return IntPtr.Zero;
      }
      return NativeMethods.DefWindowProc(hwnd, (WM)msg, wparam, lparam);
    }

    private IntPtr OnEraseBackground(IntPtr wparam)
    {
      var hdc = wparam;
      var color = new COLORREF(0xff, 0x00, 0xff);
      var brush = NativeMethods.CreateSolidBrush(color.Value);

      var rect = WindowRect;
      rect.Top = 0;
      rect.Left = 0;
      NativeMethods.FillRect(hdc, ref rect, brush);

      NativeMethods.DeleteObject(brush);

      return new IntPtr(1);
    }

    private IntPtr OnNcPaint(IntPtr wparam, IntPtr lparam)
    {
      var hrgn = wparam;

      var deleteRegion = false;
      var hdc = NativeMethods.GetDCEx(_hwnd, hrgn,
        DeviceContextValues.Window | DeviceContextValues.IntersectRgn | DeviceContextValues.Cache |
        DeviceContextValues.LockWindowUpdate);

      //if (hdc == IntPtr.Zero)
      //{
      //	hdc = NativeMethods.GetWindowDC(_hwnd);
      //	var windowRect = WindowRect;
      //	windowRect.Left = 0;
      //	windowRect.Top = 0;
      //	hrgn = NativeMethods.CreateRectRgnIndirect(windowRect);
      //	deleteRegion = true;
      //}

      var color = new COLORREF(0xff, 0x00, 0xff);
      var brush = NativeMethods.CreateSolidBrush(color.Value);

      NativeMethods.FillRgn(hdc, hrgn, brush);

      NativeMethods.DeleteObject(brush);

      if (deleteRegion)
        NativeMethods.DeleteObject(hrgn);

      NativeMethods.ReleaseDC(_hwnd, hdc);


      return IntPtr.Zero;
    }

    public RECT WindowRect => NativeMethodsSafe.GetWindowRect(_hwnd);

    private IntPtr OnNcHitTest(IntPtr wparam, IntPtr lparam)
    {
      var mousePosScreen = new POINT
      {
        x = PlatformUtil.GET_X_LPARAM(lparam),
        y = PlatformUtil.GET_Y_LPARAM(lparam)
      };

      var windowPosition = WindowRect;

      var mousePosWindow = mousePosScreen;
      mousePosWindow.x -= windowPosition.Left; 
      mousePosWindow.y -= windowPosition.Top;

      var hitTestResizeBorder = HitTestResizeBorder(mousePosWindow, windowPosition.Height, windowPosition.Width, 4, 1);
      if (hitTestResizeBorder != HT.NOWHERE)
        return (IntPtr)hitTestResizeBorder;

      return (IntPtr)HT.NOWHERE;
    }

    private COLORREF BackgroundColor => new COLORREF(0xff, 0xff, 0xff);

    private void OnPaint()
    {
      var hdc = NativeMethods.GetDC(_hwnd);

      var rect = NativeMethodsSafe.GetWindowRect(_hwnd);
      rect.Left = 0;
      rect.Top = 0;

      var color = new COLORREF(0xff, 0xff, 0xff);
      var brush = NativeMethods.CreateSolidBrush(color.Value);

      NativeMethods.FillRect(hdc, ref rect, brush);

      NativeMethods.DeleteObject(brush);

      NativeMethods.ReleaseDC(_hwnd, hdc);
    }

    private IntPtr OnNcCalcSize(IntPtr wparam, IntPtr lparam)
    {
      var rcClientArea = (RECT)Marshal.PtrToStructure(lparam, typeof(RECT));

      rcClientArea.Top -= 1;
      rcClientArea.Left -= 1;
      rcClientArea.Bottom += 1;
      rcClientArea.Right += 1;

      Marshal.StructureToPtr(rcClientArea, lparam, false);

      return new IntPtr((int)(WVR.VALIDRECTS | WVR.REDRAW));
    }

    public void Dispose()
    {
      _windowClassEx.Dispose();
    }

    private static HT HitTestResizeBorder(POINT position, int height, int width, int thickness, int dc)
    {
      var resPart = ResizeHandlerPart.Undefined;

      resPart |= position.y >= 0 && position.y <= dc * thickness ? ResizeHandlerPart.Top : 0;
      resPart |= position.y >= height - dc * thickness && position.y <= height ? ResizeHandlerPart.Bottom : 0;
      resPart |= position.x >= 0 && position.x <= dc * thickness ? ResizeHandlerPart.Left : 0;
      resPart |= position.x >= width - dc * thickness && position.x <= width ? ResizeHandlerPart.Right : 0;


      switch (resPart)
      {
        case ResizeHandlerPart.Left:
          return HT.LEFT;
        case ResizeHandlerPart.Top:
          return HT.TOP;
        case ResizeHandlerPart.Right:
          return HT.RIGHT;
        case ResizeHandlerPart.Bottom:
          return HT.BOTTOM;
        case ResizeHandlerPart.TopLeft:
          return HT.TOPLEFT;
        case ResizeHandlerPart.TopRight:
          return HT.TOPRIGHT;
        case ResizeHandlerPart.BottomRight:
          return HT.BOTTOMRIGHT;
        case ResizeHandlerPart.BottomLeft:
          return HT.BOTTOMLEFT;
      }

      return HT.NOWHERE;
    }

    [Flags]
    private enum ResizeHandlerPart
    {
      Undefined = 0x0,
      Left = 0x1,
      Top = 0x2,
      Right = 0x4,
      Bottom = 0x8,
      TopLeft = Top | Left,
      TopRight = Top | Right,
      BottomRight = Bottom | Right,
      BottomLeft = Bottom | Left,
    }
  }
}
#endif
