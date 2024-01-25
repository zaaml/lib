// <copyright file="PlatformUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Security;
using Zaaml.Core;

// ReSharper disable InconsistentNaming

namespace Zaaml.Platform
{
	internal static class PlatformUtil
	{
		#region Static Fields and Constants

		private static readonly Version _osVersion = Environment.OSVersion.Version;

		#endregion

		#region Properties

		public static bool IsOSVistaOrNewer => _osVersion >= new Version(6, 0);
		public static bool IsOSWindows7OrNewer => _osVersion >= new Version(6, 1);

		#endregion

		#region  Methods

		public static int GET_X_LPARAM(IntPtr lParam)
		{
			return LOWORD(lParam.ToInt32());
		}

		public static int GET_Y_LPARAM(IntPtr lParam)
		{
			return HIWORD(lParam.ToInt32());
		}

		public static int HIWORD(int i)
		{
			return (short) (i >> 16);
		}

		public static bool IsForegroundWindowFromCurrentProcess()
		{
			try
			{
				var foregroundWindow = NativeMethods.GetForegroundWindow();
				int processId;

				NativeMethods.GetWindowThreadProcessId(foregroundWindow, out processId);

				return processId == Process.GetCurrentProcess().Id;
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);

				return false;
			}
		}

		public static int LOWORD(int i)
		{
			return (short) (i & 0xFFFF);
		}

		internal static bool ModifyStyle(IntPtr hwnd, WS removeStyle, WS addStyle)
		{
			var intPtr = NativeMethods.GetWindowLongPtr(hwnd, GWL.STYLE);
			var dwStyle = (WS) (Environment.Is64BitProcess ? intPtr.ToInt64() : intPtr.ToInt32());
			var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;

			if (dwStyle == dwNewStyle)
				return false;

			NativeMethods.SetWindowLongPtr(hwnd, GWL.STYLE, new IntPtr((int) dwNewStyle));

			return true;
		}

		internal static bool ModifyStyleEx(IntPtr hwnd, WS_EX removeStyle, WS_EX addStyle)
		{
			var intPtr = NativeMethods.GetWindowLongPtr(hwnd, GWL.EXSTYLE);
			var dwStyle = (WS_EX) (Environment.Is64BitProcess ? intPtr.ToInt64() : intPtr.ToInt32());
			var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;

			if (dwStyle == dwNewStyle)
				return false;

			var result = NativeMethods.SetWindowLongPtr(hwnd, GWL.EXSTYLE, new IntPtr((int) dwNewStyle));

			return true;
		}

		[SecurityCritical]
		public static void SafeDeleteObject(ref IntPtr gdiObject)
		{
			var p = gdiObject;
			gdiObject = IntPtr.Zero;

			if (IntPtr.Zero != p)
				NativeMethods.DeleteObject(p);
		}

		#endregion
	}
}