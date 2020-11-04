// // <copyright file="Win32.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
// //   Copyright (c) zaaml. All rights reserved.
// // </copyright>

#if COMPILE_EVER
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zaaml.Platform
{
  internal static class Win32
  {
#region  Methods

    public static bool BringWindowToTop(IntPtr hwnd)
    {
      return NativeMethods.BringWindowToTop(hwnd);
    }

    public static IntPtr ChildWindowFromPoint(IntPtr hwndParent, POINT point)
    {
      return NativeMethods.ChildWindowFromPoint(hwndParent, point);
    }

    public static bool CloseWindow(IntPtr hwnd)
    {
      return NativeMethods.CloseWindow(hwnd);
    }

    public static IntPtr CreatePointer<T>(T struc)
    {
      var pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (T)));
      Marshal.StructureToPtr(struc, pointer, false);
      return pointer;
    }

    public static IntPtr CreateWindowEx(
      WS_EX exStyle,
      string className,
      string windowName,
      WS style,
      int x,
      int y,
      int width,
      int height,
      IntPtr hwndParent,
      IntPtr hmenu,
      IntPtr hinstance,
      IntPtr param)
    {
      return NativeMethods.CreateWindowEx(exStyle, className, windowName, style, x, y, width, height, hwndParent, hmenu,
        hinstance, param);
    }

    public static IntPtr DefWindowProc(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam)
    {
      return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
    }

    public static IntPtr DefWindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
      return NativeMethods.DefWindowProc(hwnd, (WM) msg, wParam, lParam);
    }

    public static bool DestroyWindow(IntPtr hwnd)
    {
      return NativeMethods.DestroyWindow(hwnd);
    }

    public static IntPtr DispatchMessage(ref Msg message)
    {
      return NativeMethods.DispatchMessage(ref message);
    }

    public static IEnumerable<IntPtr> EnumChildWindows(IntPtr hwndParent)
    {
      var list = new List<IntPtr>();
      EnumWindowsProc intFilter = (hwnd, param) =>
      {
        list.Add(hwnd);
        return true;
      };

      NativeMethods.EnumChildWindows(hwndParent, intFilter, IntPtr.Zero);

      return list;
    }

    public static IEnumerable<IntPtr> EnumThreadWindows(uint threadId)
    {
      var list = new List<IntPtr>();
      EnumWindowsProc intFilter = (hwnd, param) =>
      {
        list.Add(hwnd);
        return true;
      };

      NativeMethods.EnumThreadWindows(threadId, intFilter, IntPtr.Zero);

      return list;
    }

    public static IEnumerable<IntPtr> EnumWindows()
    {
      var list = new List<IntPtr>();
      EnumWindowsProc intFilter = (hwnd, param) =>
      {
        list.Add(hwnd);
        return true;
      };

      NativeMethods.EnumWindows(intFilter, IntPtr.Zero);

      return list;
    }

    public static IntPtr FindWindow(string className, string windowName)
    {
      return NativeMethods.FindWindow(className, windowName);
    }

    public static IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, string className, string windowTitle)
    {
      return NativeMethods.FindWindowEx(parentHwnd, childAfterHwnd, className, windowTitle);
    }

    public static IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags)
    {
      return NativeMethods.GetAncestor(hwnd, flags);
    }

    public static RECT GetClientRect(IntPtr hwnd)
    {
      RECT rect;
      return NativeMethods.GetClientRect(hwnd, out rect) ? rect : new RECT();
    }

    public static IntPtr GetDesktopWindow()
    {
      return NativeMethods.GetDesktopWindow();
    }

    public static sbyte GetMessage(out Msg message, IntPtr hwnd, uint messageFilterMin, uint messageFilterMax)
    {
      return NativeMethods.GetMessage(out message, hwnd, messageFilterMin, messageFilterMax);
    }

    public static IntPtr GetParent(IntPtr hwnd)
    {
      return NativeMethods.GetParent(hwnd);
    }

    public static IntPtr GetShellWindow()
    {
      return NativeMethods.GetShellWindow();
    }

    public static IntPtr GetTopWindow(IntPtr hwnd)
    {
      return NativeMethods.GetTopWindow(hwnd);
    }

    public static IntPtr GetWindow(IntPtr hwnd, GetWindowCommand command)
    {
      return NativeMethods.GetWindow(hwnd, command);
    }

    public static WindowInfo GetWindowInfo(IntPtr hwnd)
    {
      var windowinfo = WindowInfo.Create();
      NativeMethods.GetWindowInfo(hwnd, ref windowinfo);
      return windowinfo;
    }

    public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
    {
      var windowPlacement = new WINDOWPLACEMENT();
      NativeMethods.GetWindowPlacement(hwnd, ref windowPlacement);
      return windowPlacement;
    }

    public static RECT GetWindowRect(IntPtr hwnd)
    {
      RECT rect;
      return NativeMethods.GetWindowRect(hwnd, out rect) ? rect : new RECT();
    }

    public static string GetWindowText(IntPtr hwnd)
    {
      return GetWindowText(hwnd, NativeMethods.GetWindowTextLength(hwnd) + 1);
    }

    public static uint GetWindowThreadProcessId(IntPtr hwnd)
    {
      uint lpdwProcessId;
      NativeMethods.GetWindowThreadProcessId(hwnd, out lpdwProcessId);
      return lpdwProcessId;
    }

    public static RECT GetWorkArea()
    {
      var rect = new RECT();
      using (var workAreaPtr = PointerWrapper.CreatePointeWrapper(rect))
      {
        SystemParametersInfo(SPI.GETWORKAREA, 0, workAreaPtr.Pointer, SPIF.NONE);
        rect = workAreaPtr.Value;
      }
      return rect;
    }

    public static bool InvalidateRect(IntPtr hwnd, RECT rect, bool erase)
    {
      return NativeMethods.InvalidateRect(hwnd, ref rect, erase);
    }

    public static bool IsChild(IntPtr hwndParent, IntPtr hwnd)
    {
      return NativeMethods.IsChild(hwndParent, hwnd);
    }

    public static bool IsIconic(IntPtr hwnd)
    {
      return NativeMethods.IsIconic(hwnd);
    }

    public static bool IsWindow(IntPtr hwnd)
    {
      return NativeMethods.IsWindow(hwnd);
    }

    public static bool IsWindowVisible(IntPtr hwnd)
    {
      return NativeMethods.IsWindowVisible(hwnd);
    }

    public static bool IsZoomed(IntPtr hwnd)
    {
      return NativeMethods.IsZoomed(hwnd);
    }

    public static POINT LogicalToPhysicalPoint(IntPtr hwnd, POINT point)
    {
      var localPoint = point;
      NativeMethods.LogicalToPhysicalPoint(hwnd, ref localPoint);
      return localPoint;
    }

    public static bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint)
    {
      return NativeMethods.MoveWindow(hwnd, x, y, width, height, repaint);
    }

    public static bool MoveWindow(IntPtr hwnd, RECT rect, bool repaint)
    {
      return NativeMethods.MoveWindow(hwnd, rect.Left, rect.Top, rect.Width, rect.Height, repaint);
    }

    public static bool OpenIcon(IntPtr hwnd)
    {
      return NativeMethods.OpenIcon(hwnd);
    }

    public static POINT PhysicalToLogicalPoint(IntPtr hwnd, POINT point)
    {
      var localPoint = point;
      NativeMethods.PhysicalToLogicalPoint(hwnd, ref localPoint);
      return localPoint;
    }

    public static ushort RegisterClassEx(ref WNDCLASSEX lpWndClass)
    {
      return NativeMethods.RegisterClassEx(ref lpWndClass);
    }

    public static bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy,
      SWP flags)
    {
      return NativeMethods.SetWindowPos(hwnd, hwndInsertAfter, x, y, cx, cy, flags);
    }

    public static bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, RECT rect, SWP flags)
    {
      return NativeMethods.SetWindowPos(hwnd, hwndInsertAfter, rect.Left, rect.Top, rect.Width, rect.Height, flags);
    }

    public static void SetWorkArea(RECT workArea)
    {
      using (var workAreaPtr = PointerWrapper.CreatePointeWrapper(workArea))
        SystemParametersInfo(SPI.SETWORKAREA, 0, workAreaPtr.Pointer, SPIF.SENDCHANGE);
    }

    public static bool Shell_NotifyIcon(NIM message, NOTIFYICONDATA notifyIconData)
    {
      return NativeMethods.Shell_NotifyIcon(message, ref notifyIconData);
    }

    public static bool ShowWindow(IntPtr hwnd, SW showCommand)
    {
      return NativeMethods.ShowWindow(hwnd, showCommand);
    }

    public static bool SystemParametersInfo(SPI action, uint uiParam, IntPtr pvParam,
      SPIF winIniFlags)
    {
      return NativeMethods.SystemParametersInfo(action, uiParam, pvParam, winIniFlags);
    }

    public static bool TranslateMessage(ref Msg message)
    {
      return NativeMethods.TranslateMessage(ref message);
    }

    public static bool UpdateWindow(IntPtr hwnd)
    {
      return NativeMethods.UpdateWindow(hwnd);
    }

    public static IntPtr WindowFromPhysicalPoint(POINT point)
    {
      return NativeMethods.WindowFromPhysicalPoint(point);
    }

    public static IntPtr WindowFromPoint(POINT point)
    {
      return NativeMethods.WindowFromPoint(point);
    }

    internal static string GetWindowModuleFileName(IntPtr hwnd, int maxCharCount, out int charCount)
    {
      charCount = 0;

      if (hwnd == IntPtr.Zero)
        return null;

      var sb = new StringBuilder(maxCharCount);
      charCount = NativeMethods.GetWindowModuleFileName(hwnd, sb, sb.Capacity);
      return sb.ToString();
    }

    internal static string GetWindowText(IntPtr hwnd, int maxCharCount)
    {
      if (hwnd == IntPtr.Zero)
        return null;

      var sb = new StringBuilder(maxCharCount);
      NativeMethods.GetWindowText(hwnd, sb, sb.Capacity);
      return sb.ToString();
    }

    internal static bool ModifyStyle(IntPtr hwnd, WS removeStyle, WS addStyle)
    {
      var intPtr = NativeMethods.GetWindowLongPtr(hwnd, (int) GWL.STYLE);
      var dwStyle = (WS) (Environment.Is64BitProcess ? intPtr.ToInt64() : intPtr.ToInt32());
      var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
      if (dwStyle == dwNewStyle)
        return false;

      NativeMethods.SetWindowLongPtr(hwnd, (int) GWL.STYLE, new IntPtr((int) dwNewStyle));
      return true;
    }

#endregion

#region  Nested Types

    private class PointerWrapper<T> : IDisposable
    {
#region Ctors

      public PointerWrapper(T structure)
      {
        Pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (T)));
        Marshal.StructureToPtr(structure, Pointer, false);
      }

      public PointerWrapper()
      {
        Pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (T)));
      }

#endregion

#region Properties

      public IntPtr Pointer { get; private set; }
      public T Value => (T) Marshal.PtrToStructure(Pointer, typeof (T));

#endregion

#region  Methods

      public void Dispose()
      {
        if (Pointer == IntPtr.Zero) return;

        Marshal.FreeHGlobal(Pointer);
        Pointer = IntPtr.Zero;
      }

#endregion
    }

    private static class PointerWrapper
    {
#region  Methods

      public static PointerWrapper<T> CreatePointeWrapper<T>(T structure)
      {
        return new PointerWrapper<T>(structure);
      }

      public static PointerWrapper<T> CreatePointeWrapper<T>()
      {
        return new PointerWrapper<T>();
      }

#endregion
    }

#endregion
  }
}

#endif