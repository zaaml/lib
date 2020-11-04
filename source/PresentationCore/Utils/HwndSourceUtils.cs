// <copyright file="HwndSourceUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Interop;
using Zaaml.Platform;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
	internal static class HwndSourceUtils
	{
		#region  Methods

		public static HwndRectInfo GetRectInfo(HwndSource hwndSource)
		{
			return hwndSource == null ? new HwndRectInfo() : GetRectInfo(hwndSource.Handle);
		}

		public static HwndRectInfo GetRectInfo(IntPtr hwnd)
		{
			var clientNativeRect = NativeMethodsSafe.GetClientRect(hwnd);
			var windowScreenNativeRect = NativeMethodsSafe.GetWindowRect(hwnd);
			var topLeft = NativeMethodsSafe.ClientToScreen(hwnd, clientNativeRect.Position);
			var bottomRight = NativeMethodsSafe.ClientToScreen(hwnd, new POINT { x = clientNativeRect.Right, y = clientNativeRect.Bottom });
			var clientScreenNativeRect = new RECT
			{
				Left = topLeft.x,
				Top = topLeft.y,
				Right = bottomRight.x,
				Bottom = bottomRight.y
			};

			return new HwndRectInfo(windowScreenNativeRect.ToPresentationRect(), clientNativeRect.ToPresentationRect(), clientScreenNativeRect.ToPresentationRect());
		}

		public static HwndSource FromDependencyObject(DependencyObject dependencyObject)
		{
			return dependencyObject == null ? null : PresentationSource.FromDependencyObject(dependencyObject) as HwndSource;
		}

		public static WindowHitTestResult HitTest(IntPtr hwnd)
		{
			if (hwnd == IntPtr.Zero)
				return WindowHitTestResult.Outside;

			var hwndRectInfo = GetRectInfo(hwnd);
			var cursorPoint = NativeMethodsSafe.GetCursorPos().ToPresentationPoint();
			var insideClientArea = hwndRectInfo.ClientScreenRect.Contains(cursorPoint);
			var insideWindow = hwndRectInfo.WindowScreenRect.Contains(cursorPoint);

			if (insideWindow == false)
				return WindowHitTestResult.Outside;

			return insideClientArea ? WindowHitTestResult.ClientArea : WindowHitTestResult.NonClientArea;
		}

		public static WindowHitTestResult HitTest(HwndSource hwndSource)
		{
			return HitTest(hwndSource?.Handle ?? IntPtr.Zero);
		}

		#endregion
	}

	internal struct HwndRectInfo
	{
		public HwndRectInfo(Rect windowScreenRect, Rect clientRect, Rect clientScreenRect)
		{
			WindowScreenRect = windowScreenRect;
			ClientRect = clientRect;
			ClientScreenRect = clientScreenRect;
		}

		public Rect WindowScreenRect;
		public Rect ClientRect;
		public Rect ClientScreenRect;
	}

	internal enum WindowHitTestResult
	{
		Outside,
		ClientArea,
		NonClientArea
	}
}
