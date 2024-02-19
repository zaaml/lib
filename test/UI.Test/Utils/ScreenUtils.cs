// <copyright file="ScreenUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Zaaml.Platform;

namespace Zaaml.UI.Test.Utils
{
	internal static class ScreenUtils
	{
		public static BitmapSource GetWindowImage(HwndSource source)
		{
			var handle = source.Handle;
			var hdcSrc = NativeMethods.GetWindowDC(handle);

			NativeMethods.GetWindowRect(handle, out var windowRect);

			var width = windowRect.Right - windowRect.Left;
			var height = windowRect.Bottom - windowRect.Top;

			var hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
			var hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, width, height);

			var hOld = NativeMethods.SelectObject(hdcDest, hBitmap);

			NativeMethods.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, TernaryRasterOperations.SRCCOPY);
			NativeMethods.SelectObject(hdcDest, hOld);
			NativeMethods.DeleteDC(hdcDest);
			NativeMethods.ReleaseDC(handle, hdcSrc);

			var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, new Int32Rect(0, 0, windowRect.Width, windowRect.Height), BitmapSizeOptions.FromEmptyOptions());

			NativeMethods.DeleteObject(hBitmap);

			return bitmapSource;
		}
	}
}