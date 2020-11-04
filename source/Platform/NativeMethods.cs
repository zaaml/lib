// <copyright file="WindowAPI.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Hwnd = System.IntPtr;

// ReSharper disable InconsistentNaming

namespace Zaaml.Platform
{
  internal delegate bool EnumWindowsProc(Hwnd hwnd, Hwnd lParam);

  internal delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

  internal delegate Hwnd WndProc(Hwnd hwnd, uint msg, Hwnd wParam, Hwnd lParam);

  internal delegate Hwnd MessageHandler(WM uMsg, Hwnd wParam, Hwnd lParam, out bool handled);

  internal static partial class NativeMethods
  {
    internal delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);


    #region Static Fields

    private const string User32 = "user32.dll";
    private const string UXTheme = "uxtheme.dll";
    private const string Shell32 = "shell32.dll";
    private const string Dwmapi = "dwmapi.dll";
    private const string Kernel32 = "kernel32.dll";

    #endregion

    [DllImport(User32, ExactSpelling = true)]
    public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr rcClip, NativeMethods.MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

    [DllImport(User32)]
    public static extern IntPtr MonitorFromRect([In] ref RECT lprc, MonitorOptions dwFlags);

    [DllImport(Dwmapi, PreserveSig = true)]
    public static extern int DwmExtendFrameIntoClientArea(Hwnd hwnd, ref MARGINS margins);

    [DllImport(Dwmapi, PreserveSig = true)]
    public static extern int DwmSetWindowAttribute(Hwnd hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize);

    [DllImport(Dwmapi, EntryPoint = "#131", PreserveSig = false)]
    public static extern void DwmSetColorizationParameters(ref DWM_COLORIZATION_PARAMS parameters, bool unknown);

    [DllImport(Shell32, CallingConvention = CallingConvention.StdCall)]
	  public static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

		#region Methods

		[DllImport(User32)]
    public static extern bool BringWindowToTop(Hwnd hwnd);

    [DllImport(User32)]
    public static extern Hwnd ChildWindowFromPoint(Hwnd hwndParent, POINT point);

    [DllImport(User32)]
    public static extern bool CloseWindow(Hwnd hwnd);

    [DllImport(User32, SetLastError = true)]
    public static extern Hwnd CreateWindowEx(
      WS_EX exStyle,
      [MarshalAs(UnmanagedType.LPStr)] string lpClassName,
      [MarshalAs(UnmanagedType.LPStr)] string lpWindowName,
			WS style,
      int x,
      int y,
      int width,
      int height,
      Hwnd hwndParent,
      Hwnd hmenu,
      Hwnd hinstance,
      Hwnd param);

    [DllImport(User32)]
    public static extern IntPtr CreatePopupMenu();

    [DllImport(User32, CharSet = CharSet.Auto)]
    public static extern bool AppendMenu(IntPtr hMenu, MF uFlags, uint uIDNewItem, string lpNewItem);

    [DllImport(User32)]
    public static extern bool InsertMenuItem(IntPtr hMenu, uint uItem, bool fByPosition, ref MENUITEMINFO lpmii);

    [DllImport(User32)]
    public static extern Hwnd DefWindowProc(Hwnd hwnd, WM msg, Hwnd wParam, Hwnd lParam);

    [DllImport(User32)]
    public static extern bool DestroyWindow(Hwnd hwnd);

    [DllImport(User32)]
    public static extern Hwnd DispatchMessage(ref Msg lpmsg);

    [DllImport(User32)]
    public static extern bool EnumChildWindows(Hwnd hwndParent, EnumWindowsProc lpEnumFunc, Hwnd lParam);

    [DllImport(User32)]
    public static extern bool EnumThreadWindows(uint threadId, EnumWindowsProc lpEnumFunc, Hwnd lParam);

    [DllImport(User32)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, Hwnd lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern Hwnd FindWindow(String className, String windowName);

    [DllImport(User32, SetLastError = true)]
    public static extern Hwnd FindWindowEx(Hwnd parentHwnd, Hwnd childAfterHwnd, string className, string windowTitle);

    [DllImport(User32, ExactSpelling = true)]
    public static extern Hwnd GetAncestor(Hwnd hwnd, GetAncestorFlags flags);

    [DllImport(User32)]
    public static extern bool GetClientRect(Hwnd hwnd, out RECT rect);

    [DllImport(User32)]
    public static extern Hwnd GetDesktopWindow();

    [DllImport(User32)]
    public static extern sbyte GetMessage(out Msg message, Hwnd hwnd, uint messageFilterMin, uint messageFilterMax);

    [DllImport(User32, ExactSpelling = true, CharSet = CharSet.Auto)]
    public static extern Hwnd GetParent(Hwnd hwnd);

    [DllImport(User32)]
    public static extern Hwnd GetShellWindow();

    [DllImport(User32)]
    public static extern Hwnd GetTopWindow(Hwnd hwnd);

    [DllImport(User32, SetLastError = true)]
    public static extern Hwnd GetWindow(Hwnd hwnd, GetWindowCommand command);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetWindowInfo(Hwnd hwnd, ref WindowInfo pwi);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowModuleFileName(Hwnd hwnd, StringBuilder fileName, int nMaxCount);

    //[DllImport(User32, SetLastError = true)]
    //public static extern bool GetWindowPlacement(Hwnd hwnd, ref WINDOWPLACEMENT windowPlacement);

    [DllImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    [SecurityCritical]
    public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
    {
      var wndpl = WINDOWPLACEMENT.Default;

      if (GetWindowPlacement(hwnd, ref wndpl))
        return wndpl;

      throw new Win32Exception();
    }

    [DllImport(User32)]
    public static extern bool GetWindowRect(Hwnd hwnd, out RECT lpRect);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(Hwnd hwnd, StringBuilder lpString, int nMaxCount);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLength(Hwnd hwnd);

    [DllImport(User32, SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out int lpdwProcessId);

    [DllImport(User32)]
    public static extern bool InvalidateRect(Hwnd hwnd, ref RECT rect, bool erase);

    [DllImport(User32)]
    public static extern bool IsChild(Hwnd hwndParent, Hwnd hwnd);

    [DllImport(User32)]
    public static extern bool IsIconic(Hwnd hwnd);

    [DllImport(User32)]
    public static extern bool IsWindow(Hwnd hwnd);

    [DllImport(User32)]
    public static extern bool IsWindowVisible(Hwnd hwnd);

    [DllImport(User32)]
    public static extern bool IsZoomed(Hwnd hwnd);

    [DllImport(User32)]
    public static extern bool LogicalToPhysicalPoint(Hwnd hwnd, ref POINT point);

    [DllImport(User32, SetLastError = true)]
    public static extern bool MoveWindow(Hwnd hwnd, int x, int y, int width, int height, bool repaint);

    [DllImport(User32)]
    public static extern bool OpenIcon(Hwnd hwnd);

    [DllImport(User32)]
    public static extern bool PhysicalToLogicalPoint(Hwnd hwnd, ref POINT point);

    [DllImport(User32)]
    public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

    [DllImport(User32)]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

    [DllImport(User32)]
    public static extern Hwnd SetActiveWindow(Hwnd hWnd);

    [DllImport(User32)]
    public static extern bool SetForegroundWindow(Hwnd hWnd);

    [DllImport(User32)]
    public static extern Hwnd GetForegroundWindow();

    [DllImport(User32)]
    public static extern bool SetWindowPos(Hwnd hwnd, Hwnd hwndInsertAfter, int x, int y, int cx, int cy, SWP flags);

    [DllImport(Shell32)]
    public static extern bool Shell_NotifyIcon(NIM message, ref NOTIFYICONDATA notifyIconData);

    [DllImport(Shell32, CharSet = CharSet.Auto)]
    public static extern IntPtr SHGetFileInfo(string pszPath, FILE_ATTRIBUTE dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

    [DllImport(User32)]
    public static extern bool ShowWindow(Hwnd hwnd, SW showCommand);

		[DllImport(User32)]
		public static extern bool ClientToScreen(Hwnd hWnd, ref POINT lpPoint);

		[DllImport(User32)]
    public static extern bool SystemParametersInfo(SPI action, uint uiParam, IntPtr pvParam, SPIF winIniFlags);

    [DllImport(User32)]
    public static extern bool TranslateMessage(ref Msg message);

    [DllImport(User32)]
    public static extern bool UpdateWindow(Hwnd hwnd);

    [DllImport(User32)]
    public static extern Hwnd WindowFromPhysicalPoint(POINT point);

    [DllImport(User32)]
    public static extern Hwnd WindowFromPoint(POINT point);

    [DllImport(User32)]
    public static extern IntPtr SendMessage(Hwnd hWnd, IntPtr msg, IntPtr wParam, IntPtr lParam);

    [DllImport(User32)]
    public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

    public static IntPtr SetClassLong(IntPtr hWnd, ClassLongFlags nIndex, IntPtr dwNewLong)
    {
	    return Is64Bit ? SetClassLongPtr64(hWnd, nIndex, dwNewLong) : SetClassLongPtr32(hWnd, nIndex, dwNewLong);
    }

    [DllImport(User32, EntryPoint = "SetClassLong")]
    private static extern IntPtr SetClassLongPtr32(IntPtr hWnd, ClassLongFlags nIndex, IntPtr dwNewLong);

    [DllImport(User32, EntryPoint = "SetClassLongPtr")]
    private static extern IntPtr SetClassLongPtr64(IntPtr hWnd, ClassLongFlags nIndex, IntPtr dwNewLong);


    [DllImport(User32, EntryPoint = "GetWindowLong")]
    private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);


    [DllImport(User32, EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);


    public static IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex)
    {
      return Is64Bit ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
    }

    public static bool Is64Bit => IntPtr.Size == 8;

    [DllImport(User32)]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, COLORREF crKey, byte bAlpha, LWA dwFlags);
    
		// This static method is required because legacy OSes do not support
		// SetWindowLongPtr 
		public static IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong)
    {
      return Is64Bit ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, dwNewLong);
    }

		[DllImport(User32, EntryPoint = "SetWindowLong")]
    private static extern IntPtr SetWindowLong32(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

    [DllImport(User32, EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

    [DllImport(User32)]
    public static extern IntPtr BeginPaint(IntPtr hwnd, out PaintStruct lpPaint);

    [DllImport(User32)]
    public static extern bool EndPaint(IntPtr hWnd, [In] ref PaintStruct lpPaint);

    [DllImport(User32)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport(User32)]
    public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);

    [DllImport(User32)]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport(User32)]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport(User32)]
    public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

	  [DllImport(User32)]
	  public static extern bool GetUpdateRect(IntPtr hWnd, ref RECT rect, bool bErase);

		[DllImport(User32, SetLastError = true)]
    public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

    [SecurityCritical]
    [DllImport(Dwmapi, EntryPoint = "DwmIsCompositionEnabled", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool _DwmIsCompositionEnabled();

    [SecurityCritical]
    public static bool DwmIsCompositionEnabled()
    {
      return PlatformUtil.IsOSVistaOrNewer && _DwmIsCompositionEnabled();
    }

    [SecurityCritical]
    [DllImport(User32)]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOptions dwFlags);

    [SecurityCritical]
    [DllImport(User32, EntryPoint = "GetMonitorInfo", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool _GetMonitorInfo(IntPtr hMonitor, [In, Out] ref MONITORINFO lpmi);

    [SecurityCritical]
    public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
    {
      var mi = MONITORINFO.Default;
      if (_GetMonitorInfo(hMonitor, ref mi) == false)
        throw new Win32Exception();

      return mi;
    }

    [SecurityCritical]
    [DllImport(User32, EntryPoint = "AdjustWindowRectEx", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool _AdjustWindowRectEx(ref RECT lpRect, WS dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu, WS_EX dwExStyle);

    [SecurityCritical]
    public static RECT AdjustWindowRectEx(RECT lpRect, WS dwStyle, bool bMenu, WS_EX dwExStyle)
    {
      if (_AdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle) == false)
        NativeGuard.ThrowLastError();

      return lpRect;
    }

    [DllImport(UXTheme, PreserveSig = false)]
    public static extern void SetWindowThemeAttribute(IntPtr hwnd, WINDOWTHEMEATTRIBUTETYPE attribute, ref WTA_OPTIONS pvAttribute, [In] uint cbAttribute);

    [DllImport(UXTheme, ExactSpelling = true, CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    [SecurityCritical]
    [DllImport(User32)]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

    [SecurityCritical]
    [DllImport(User32, EntryPoint = "EnableMenuItem")]
    private static extern int _EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable);


    [SecurityCritical]
    public static MF EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable)
    {
      var iRet = _EnableMenuItem(hMenu, uIDEnableItem, uEnable);
      return (MF)iRet;
    }

    [SecurityCritical]
    [DllImport(User32, EntryPoint = "PostMessage", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool _PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

    [SecurityCritical]
    public static void PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam)
    {
      if (_PostMessage(hWnd, Msg, wParam, lParam) == false)
        throw new Win32Exception();
    }

    [DllImport(User32)]
    public static extern bool WaitMessage();

    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, PM wRemoveMsg);

    [SecurityCritical]
    [DllImport(User32)]
    public static extern uint TrackPopupMenuEx(IntPtr hmenu, TPM fuFlags, int x, int y, IntPtr hwnd, IntPtr tpmParams);

    [DllImport(User32)]
    public static extern IntPtr CreateIconIndirect([In] ref ICONINFO piconinfo);

    #endregion

    public const int WM_SYSCOMMAND = 0x112;
    public const int SC_SIZE = 0xF000;
    public const int SC_MOVE = 0xF010;
    public const int SC_MOUSEMOVE = 0xF012;
    internal const int SC_KEYMENU = 0xF100;

    [DllImport(Dwmapi, EntryPoint = "DwmGetCompositionTimingInfo")]
    private static extern HRESULT _DwmGetCompositionTimingInfo(IntPtr hwnd, ref DWM_TIMING_INFO pTimingInfo);

    public static DWM_TIMING_INFO? DwmGetCompositionTimingInfo(IntPtr hwnd)
    {
      if (!PlatformUtil.IsOSVistaOrNewer)
      {
        // API was new to Vista.
        return null;
      }

      var dti = new DWM_TIMING_INFO { cbSize = Marshal.SizeOf(typeof(DWM_TIMING_INFO)) };
      var hr = _DwmGetCompositionTimingInfo(hwnd, ref dti);
      if (hr == HRESULT.E_PENDING)
        return null;

      hr.ThrowIfFailed();

      return dti;
    }

    [DllImport(Dwmapi)]
    public static extern bool DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);

    [DllImport(User32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(ref POINT pt);

    [DllImport(User32)]
    internal static extern bool GetPhysicalCursorPos(ref POINT pt);

    [DllImport(User32)]
    public static extern bool SetCursorPos(int X, int Y);

    [DllImport(User32)]
    public static extern bool SetPhysicalCursorPos(int X, int Y);

    [DllImport(User32, CharSet = CharSet.Auto)]
    public static extern short GetKeyState(VirtualKeyStates keyCode);

		[DllImport(Kernel32)]
		public static extern uint GetCurrentThreadId();

		[DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport(User32)]
    public static extern int GetSystemMetrics(SM smIndex);

    [DllImport(User32)]
    public static extern void NotifyWinEvent(EVENT eventId, IntPtr hWnd, OBJID objId, OBJID childId);
  }

  public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

  internal static class NativeMethodsSafe
  {
    public static RECT GetWindowRect(IntPtr hwnd)
    {
      RECT rc;

      if (NativeMethods.GetWindowRect(hwnd, out rc) == false)
        NativeGuard.ThrowLastError();

      return rc;
    }

    public static RECT GetClientRect(IntPtr hwnd)
    {
      RECT rc;

      if (NativeMethods.GetClientRect(hwnd, out rc) == false)
        NativeGuard.ThrowLastError();

      return rc;
    }

    public static POINT GetCursorPos()
    {
      var point = new POINT();

      NativeMethods.GetCursorPos(ref point);

      return point;
    }

    public static WS GetWindowStyle(IntPtr hwnd)
    {
      return (WS)NativeMethods.GetWindowLongPtr(hwnd, GWL.STYLE);
    }

    public static WS_EX GetWindowStyleEx(IntPtr hwnd)
    {
      return (WS_EX)NativeMethods.GetWindowLongPtr(hwnd, GWL.EXSTYLE);
    }

		public static POINT ClientToScreen(Hwnd hWnd, POINT point)
		{
			var result = point;

			if (NativeMethods.ClientToScreen(hWnd, ref result) == false)
				NativeGuard.ThrowLastError();

			return result;
		}
	}

	internal static class NativeGuard
  {
    public static void ThrowLastError()
    {
      if (Marshal.GetLastWin32Error() < 0)
        throw new Win32Exception(Marshal.GetLastWin32Error());
    }
  }

  internal static class KeyState
  {
    public const int KEY_PRESSED = 0x8000;
    public const int KEY_TOGGLED = 0x0001;

    public static bool IsKeyPressed(VirtualKeyStates key)
    {
      return Convert.ToBoolean(NativeMethods.GetKeyState(VirtualKeyStates.VK_LBUTTON) & KEY_PRESSED);
    }

    public static bool IsKeyToggled(VirtualKeyStates key)
    {
      return Convert.ToBoolean(NativeMethods.GetKeyState(VirtualKeyStates.VK_LBUTTON) & KEY_TOGGLED);
    }
  }


  [SecurityCritical]
  internal sealed class SafeHICON : SafeHandleZeroOrMinusOneIsInvalid
  {
    [SecurityCritical]
    private SafeHICON() : base(true) { }

    [SecurityCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    protected override bool ReleaseHandle()
    {
      return NativeMethods.DeleteObject(handle);
    }
  }
  // ReSharper restore InconsistentNaming
}