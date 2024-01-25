// <copyright file="NativeMethods.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Hwnd = System.IntPtr;

// ReSharper disable InconsistentNaming

namespace Zaaml.Platform
{
	internal delegate bool EnumWindowsProc(Hwnd hwnd, Hwnd lParam);

	internal delegate bool MonitorEnumProc(Hwnd monitor, Hwnd hdc, Hwnd lprcMonitor, Hwnd lParam);

	internal delegate Hwnd WndProc(Hwnd hwnd, uint msg, Hwnd wParam, Hwnd lParam);

	internal delegate Hwnd MessageHandler(WM uMsg, Hwnd wParam, Hwnd lParam, out bool handled);

	internal static partial class NativeMethods
	{
		private const string User32 = "user32.dll";
		private const string UXTheme = "uxtheme.dll";
		private const string Shell32 = "shell32.dll";
		private const string Dwmapi = "dwmapi.dll";
		private const string Kernel32 = "kernel32.dll";

		public const int WM_SYSCOMMAND = 0x112;
		public const int SC_SIZE = 0xF000;
		public const int SC_MOVE = 0xF010;
		public const int SC_MOUSEMOVE = 0xF012;
		internal const int SC_KEYMENU = 0xF100;

		public static bool Is64Bit => Hwnd.Size == 8;

		[SecurityCritical]
		[DllImport(User32, EntryPoint = "AdjustWindowRectEx", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _AdjustWindowRectEx(ref RECT lpRect, WS dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu, WS_EX dwExStyle);

		[DllImport(Dwmapi, EntryPoint = "DwmGetCompositionTimingInfo")]
		private static extern HRESULT _DwmGetCompositionTimingInfo(Hwnd hwnd, ref DWM_TIMING_INFO pTimingInfo);

		[SecurityCritical]
		[DllImport(Dwmapi, EntryPoint = "DwmIsCompositionEnabled", PreserveSig = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _DwmIsCompositionEnabled();

		[SecurityCritical]
		[DllImport(User32, EntryPoint = "EnableMenuItem")]
		private static extern int _EnableMenuItem(Hwnd hMenu, SC uIDEnableItem, MF uEnable);

		[SecurityCritical]
		[DllImport(User32, EntryPoint = "GetMonitorInfo", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _GetMonitorInfo(Hwnd hMonitor, [In] [Out] ref MONITORINFO lpmi);

		[SecurityCritical]
		[DllImport(User32, EntryPoint = "PostMessage", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _PostMessage(Hwnd hWnd, WM Msg, Hwnd wParam, Hwnd lParam);

		[SecurityCritical]
		public static RECT AdjustWindowRectEx(RECT lpRect, WS dwStyle, bool bMenu, WS_EX dwExStyle)
		{
			if (_AdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle) == false)
				NativeGuard.ThrowLastError();

			return lpRect;
		}

		[DllImport(User32, CharSet = CharSet.Auto)]
		public static extern bool AppendMenu(Hwnd hMenu, MF uFlags, uint uIDNewItem, string lpNewItem);

		[DllImport(User32)]
		public static extern Hwnd BeginPaint(Hwnd hwnd, out PaintStruct lpPaint);

		[DllImport(User32)]
		public static extern bool BringWindowToTop(Hwnd hwnd);

		[DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern Hwnd CallNextHookEx(Hwnd hhk, int nCode, Hwnd wParam, Hwnd lParam);

		[DllImport(User32)]
		public static extern Hwnd ChildWindowFromPoint(Hwnd hwndParent, POINT point);

		[DllImport(User32)]
		public static extern bool ClientToScreen(Hwnd hWnd, ref POINT lpPoint);

		[DllImport(User32)]
		public static extern bool CloseWindow(Hwnd hwnd);

		[DllImport(User32)]
		public static extern Hwnd CreateIconIndirect([In] ref ICONINFO piconinfo);

		[DllImport(User32)]
		public static extern Hwnd CreatePopupMenu();

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
		public static extern Hwnd DefWindowProc(Hwnd hwnd, WM msg, Hwnd wParam, Hwnd lParam);

		[DllImport(User32)]
		public static extern bool DestroyWindow(Hwnd hwnd);

		[DllImport(User32)]
		public static extern Hwnd DispatchMessage(ref Msg lpmsg);

		[DllImport(Dwmapi)]
		public static extern bool DwmDefWindowProc(Hwnd hWnd, int msg, Hwnd wParam, Hwnd lParam, ref Hwnd plResult);

		[DllImport(Dwmapi, PreserveSig = true)]
		public static extern int DwmExtendFrameIntoClientArea(Hwnd hwnd, ref MARGINS margins);

		public static DWM_TIMING_INFO? DwmGetCompositionTimingInfo(Hwnd hwnd)
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

		[SecurityCritical]
		public static bool DwmIsCompositionEnabled()
		{
			return PlatformUtil.IsOSVistaOrNewer && _DwmIsCompositionEnabled();
		}

		[DllImport(Dwmapi, EntryPoint = "#131", PreserveSig = false)]
		public static extern void DwmSetColorizationParameters(ref DWM_COLORIZATION_PARAMS parameters, bool unknown);

		[DllImport(Dwmapi, PreserveSig = true)]
		public static extern int DwmSetWindowAttribute(Hwnd hwnd, DwmWindowAttribute attr, ref int attrValue, int attrSize);


		[SecurityCritical]
		public static MF EnableMenuItem(Hwnd hMenu, SC uIDEnableItem, MF uEnable)
		{
			var iRet = _EnableMenuItem(hMenu, uIDEnableItem, uEnable);
			return (MF)iRet;
		}

		[DllImport(User32)]
		public static extern bool EndPaint(Hwnd hWnd, [In] ref PaintStruct lpPaint);

		[DllImport(User32)]
		public static extern bool EnumChildWindows(Hwnd hwndParent, EnumWindowsProc lpEnumFunc, Hwnd lParam);

		[DllImport(User32, ExactSpelling = true)]
		public static extern bool EnumDisplayMonitors(Hwnd hdc, Hwnd rcClip, MonitorEnumProc lpfnEnum, Hwnd dwData);

		[DllImport(User32)]
		public static extern bool EnumThreadWindows(uint threadId, EnumWindowsProc lpEnumFunc, Hwnd lParam);

		[DllImport(User32)]
		public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, Hwnd lParam);

		[DllImport(User32)]
		public static extern int FillRect(Hwnd hDC, [In] ref RECT lprc, Hwnd hbr);

		[DllImport(User32, SetLastError = true)]
		public static extern Hwnd FindWindow(string className, string windowName);

		[DllImport(User32, SetLastError = true)]
		public static extern Hwnd FindWindowEx(Hwnd parentHwnd, Hwnd childAfterHwnd, string className, string windowTitle);

		[DllImport(User32, ExactSpelling = true)]
		public static extern Hwnd GetAncestor(Hwnd hwnd, GetAncestorFlags flags);

		[DllImport(User32, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetClassInfoEx(Hwnd hInstance, string lpClassName, ref WNDCLASSEX lpWndClass);

		[DllImport(User32)]
		public static extern int GetClassName(Hwnd hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport(User32)]
		public static extern bool GetClientRect(Hwnd hwnd, out RECT rect);

		[DllImport(Kernel32)]
		public static extern uint GetCurrentThreadId();

		[DllImport(User32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(ref POINT pt);

		[DllImport(User32)]
		public static extern Hwnd GetDC(Hwnd hWnd);

		[DllImport(User32)]
		public static extern Hwnd GetDCEx(Hwnd hWnd, Hwnd hrgnClip, DeviceContextValues flags);

		[DllImport(User32)]
		public static extern Hwnd GetDesktopWindow();

		[DllImport(Gdi32)]
		public static extern int GetDeviceCaps(Hwnd hdc, DeviceCap nIndex);

		[DllImport("Shcore")]
		public static extern int GetDpiForMonitor(Hwnd hmonitor, MonitorDpiTypes dpiType, out int dpiX, out int dpiY);

		[DllImport(User32)]
		public static extern Hwnd GetForegroundWindow();

		[DllImport(User32, CharSet = CharSet.Auto)]
		public static extern short GetKeyState(VirtualKeyStates keyCode);

		[DllImport(User32)]
		public static extern sbyte GetMessage(out Msg message, Hwnd hwnd, uint messageFilterMin, uint messageFilterMax);

		[DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern Hwnd GetModuleHandle(string lpModuleName);

		[SecurityCritical]
		public static MONITORINFO GetMonitorInfo(Hwnd hMonitor)
		{
			var mi = MONITORINFO.Default;
			if (_GetMonitorInfo(hMonitor, ref mi) == false)
				throw new Win32Exception();

			return mi;
		}

		[DllImport(User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern Hwnd GetParent(Hwnd hwnd);

		[DllImport(User32)]
		internal static extern bool GetPhysicalCursorPos(ref POINT pt);

		[DllImport("Shcore.dll")]
		public static extern int GetProcessDpiAwareness(Hwnd hProcess, out PROCESS_DPI_AWARENESS value);

		[DllImport(User32)]
		public static extern Hwnd GetShellWindow();

		[SecurityCritical]
		[DllImport(User32)]
		public static extern Hwnd GetSystemMenu(Hwnd hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

		[DllImport(User32)]
		public static extern int GetSystemMetrics(SM smIndex);

		[DllImport(User32)]
		public static extern Hwnd GetTopWindow(Hwnd hwnd);

		[DllImport(User32)]
		public static extern bool GetUpdateRect(Hwnd hWnd, ref RECT rect, bool bErase);

		[DllImport(User32, SetLastError = true)]
		public static extern Hwnd GetWindow(Hwnd hwnd, GetWindowCommand command);

		[DllImport(User32)]
		public static extern Hwnd GetWindowDC(Hwnd hWnd);

		[DllImport(User32, SetLastError = true)]
		public static extern bool GetWindowInfo(Hwnd hwnd, ref WindowInfo pwi);


		public static Hwnd GetWindowLongPtr(Hwnd hWnd, GWL nIndex)
		{
			return Is64Bit ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
		}


		[DllImport(User32, EntryPoint = "GetWindowLong", SetLastError = true)]
		private static extern Hwnd GetWindowLongPtr32(Hwnd hWnd, GWL nIndex);


		[DllImport(User32, EntryPoint = "GetWindowLongPtr", SetLastError = true)]
		private static extern Hwnd GetWindowLongPtr64(Hwnd hWnd, GWL nIndex);

		[DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowModuleFileName(Hwnd hwnd, StringBuilder fileName, int nMaxCount);

		//[DllImport(User32, SetLastError = true)]
		//public static extern bool GetWindowPlacement(Hwnd hwnd, ref WINDOWPLACEMENT windowPlacement);

		[DllImport(User32, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowPlacement(Hwnd hWnd, ref WINDOWPLACEMENT lpwndpl);

		[SecurityCritical]
		public static WINDOWPLACEMENT GetWindowPlacement(Hwnd hwnd)
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
		public static extern uint GetWindowThreadProcessId(Hwnd hwnd, out int lpdwProcessId);

		[DllImport(User32)]
		public static extern bool InsertMenuItem(Hwnd hMenu, uint uItem, bool fByPosition, ref MENUITEMINFO lpmii);

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
		public static extern Hwnd MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

		[DllImport(User32)]
		public static extern Hwnd MonitorFromRect([In] ref RECT lprc, MonitorOptions dwFlags);

		[SecurityCritical]
		[DllImport(User32)]
		public static extern Hwnd MonitorFromWindow(Hwnd hwnd, MonitorOptions dwFlags);

		[DllImport(User32)]
		public static extern void mouse_event(MOUSEEVENT dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		[DllImport(User32, SetLastError = true)]
		public static extern bool MoveWindow(Hwnd hwnd, int x, int y, int width, int height, bool repaint);

		[DllImport(User32)]
		public static extern void NotifyWinEvent(EVENT eventId, Hwnd hWnd, OBJID objId, OBJID childId);

		[DllImport(User32)]
		public static extern bool OpenIcon(Hwnd hwnd);

		[DllImport(User32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PeekMessage(out Msg lpMsg, Hwnd hWnd, uint wMsgFilterMin, uint wMsgFilterMax, PM wRemoveMsg);

		[DllImport(User32)]
		public static extern bool PhysicalToLogicalPoint(Hwnd hwnd, ref POINT point);

		[SecurityCritical]
		public static void PostMessage(Hwnd hWnd, WM Msg, Hwnd wParam, Hwnd lParam)
		{
			if (_PostMessage(hWnd, Msg, wParam, lParam) == false)
				throw new Win32Exception();
		}

		[DllImport(User32, SetLastError = true)]
		public static extern bool RedrawWindow(Hwnd hWnd, Hwnd lprcUpdate, Hwnd hrgnUpdate, RedrawWindowFlags flags);

		[DllImport(User32)]
		public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

		[DllImport(User32)]
		public static extern bool ReleaseDC(Hwnd hWnd, Hwnd hDC);

		[DllImport(User32)]
		public static extern Hwnd SendMessage(Hwnd hWnd, Hwnd msg, Hwnd wParam, Hwnd lParam);

		[DllImport(User32)]
		public static extern Hwnd SetActiveWindow(Hwnd hWnd);

		public static Hwnd SetClassLong(Hwnd hWnd, ClassLongFlags nIndex, Hwnd dwNewLong)
		{
			return Is64Bit ? SetClassLongPtr64(hWnd, nIndex, dwNewLong) : SetClassLongPtr32(hWnd, nIndex, dwNewLong);
		}

		[DllImport(User32, EntryPoint = "SetClassLong")]
		private static extern Hwnd SetClassLongPtr32(Hwnd hWnd, ClassLongFlags nIndex, Hwnd dwNewLong);

		[DllImport(User32, EntryPoint = "SetClassLongPtr")]
		private static extern Hwnd SetClassLongPtr64(Hwnd hWnd, ClassLongFlags nIndex, Hwnd dwNewLong);

		[DllImport(User32)]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport(User32)]
		public static extern bool SetForegroundWindow(Hwnd hWnd);

		[DllImport(User32)]
		public static extern bool SetLayeredWindowAttributes(Hwnd hwnd, COLORREF crKey, byte bAlpha, LWA dwFlags);

		[DllImport(User32)]
		public static extern bool SetParent(Hwnd hWndChild, Hwnd hWndNewParent);

		[DllImport(User32)]
		public static extern bool SetPhysicalCursorPos(int X, int Y);

		[DllImport("Shcore")]
		public static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

		[DllImport(User32, EntryPoint = "SetWindowLong")]
		private static extern Hwnd SetWindowLong32(Hwnd hWnd, GWL nIndex, Hwnd dwNewLong);

		// This static method is required because legacy OSes do not support
		// SetWindowLongPtr 
		public static Hwnd SetWindowLongPtr(Hwnd hWnd, GWL nIndex, Hwnd dwNewLong)
		{
			return Is64Bit ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, dwNewLong);
		}

		[DllImport(User32, EntryPoint = "SetWindowLongPtr")]
		private static extern Hwnd SetWindowLongPtr64(Hwnd hWnd, GWL nIndex, Hwnd dwNewLong);

		[DllImport(User32)]
		public static extern bool SetWindowPos(Hwnd hwnd, Hwnd hwndInsertAfter, int x, int y, int cx, int cy, SWP flags);

		[DllImport(User32)]
		public static extern int SetWindowRgn(Hwnd hWnd, Hwnd hRgn, bool bRedraw);

		[DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern Hwnd SetWindowsHookEx(HookType idHook, HookProc lpfn, Hwnd hMod, uint dwThreadId);

		[DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetWindowText(Hwnd hwnd, string lpString);

		[DllImport(UXTheme, ExactSpelling = true, CharSet = CharSet.Unicode)]
		public static extern int SetWindowTheme(Hwnd hWnd, string pszSubAppName, string pszSubIdList);

		[DllImport(UXTheme, PreserveSig = false)]
		public static extern void SetWindowThemeAttribute(Hwnd hwnd, WINDOWTHEMEATTRIBUTETYPE attribute, ref WTA_OPTIONS pvAttribute, [In] uint cbAttribute);

		[DllImport(Shell32, CallingConvention = CallingConvention.StdCall)]
		public static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

		[DllImport(Shell32)]
		public static extern bool Shell_NotifyIcon(NIM message, ref NOTIFYICONDATA notifyIconData);

		[DllImport(Shell32, CharSet = CharSet.Auto)]
		public static extern Hwnd SHGetFileInfo(string pszPath, FILE_ATTRIBUTE dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

		[DllImport(User32)]
		public static extern bool ShowWindow(Hwnd hwnd, SW showCommand);

		[DllImport(User32)]
		public static extern bool SystemParametersInfo(SPI action, uint uiParam, Hwnd pvParam, SPIF winIniFlags);

		[SecurityCritical]
		[DllImport(User32)]
		public static extern uint TrackPopupMenuEx(Hwnd hmenu, TPM fuFlags, int x, int y, Hwnd hwnd, Hwnd tpmParams);

		[DllImport(User32)]
		public static extern bool TranslateMessage(ref Msg message);

		[DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(Hwnd hhk);

		[DllImport(User32)]
		public static extern bool UnregisterClass(string lpClassName, Hwnd hInstance);

		[DllImport(User32)]
		public static extern bool UpdateWindow(Hwnd hwnd);

		[DllImport(User32)]
		public static extern bool WaitMessage();

		[DllImport(User32)]
		public static extern Hwnd WindowFromPhysicalPoint(POINT point);

		[DllImport(User32)]
		public static extern Hwnd WindowFromPoint(POINT point);

		internal delegate bool MonitorEnumProc(Hwnd monitor, Hwnd hdc, Hwnd lprcMonitor, Hwnd lParam);
	}

	public delegate Hwnd HookProc(int nCode, Hwnd wParam, Hwnd lParam);

	internal static class NativeMethodsSafe
	{
		public static POINT ClientToScreen(Hwnd hWnd, POINT point)
		{
			var result = point;

			if (NativeMethods.ClientToScreen(hWnd, ref result) == false)
				NativeGuard.ThrowLastError();

			return result;
		}

		public static RECT GetClientRect(Hwnd hwnd)
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

		public static RECT GetWindowRect(Hwnd hwnd)
		{
			RECT rc;

			if (NativeMethods.GetWindowRect(hwnd, out rc) == false)
				NativeGuard.ThrowLastError();

			return rc;
		}

		public static WS GetWindowStyle(Hwnd hwnd)
		{
			return (WS)NativeMethods.GetWindowLongPtr(hwnd, GWL.STYLE);
		}

		public static WS_EX GetWindowStyleEx(Hwnd hwnd)
		{
			return (WS_EX)NativeMethods.GetWindowLongPtr(hwnd, GWL.EXSTYLE);
		}

		public static void SetCursorPos(POINT point)
		{
			NativeMethods.SetCursorPos(point.x, point.y);
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
		private SafeHICON() : base(true)
		{
		}

		[SecurityCritical]
#if !NET5_0
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
		protected override bool ReleaseHandle()
		{
			return NativeMethods.DeleteObject(handle);
		}
	}
	// ReSharper restore InconsistentNaming
}