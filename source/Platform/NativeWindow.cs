// <copyright file="NativeWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Zaaml.Platform
{
	internal class NativeWindow
	{
		private readonly IntPtr _hwnd;

		public NativeWindow(IntPtr hwnd)
		{
			_hwnd = hwnd;
		}

		public RECT GetClientRect()
		{
			NativeMethods.GetClientRect(_hwnd, out var rect);

			return rect;
		}

		public RECT GetWindowRect()
		{
			return NativeMethodsSafe.GetWindowRect(_hwnd);
		}

		public void Invalidate()
		{
			var rect = GetWindowRect();

			rect.Left = 0;
			rect.Top = 0;
			rect.Right = 500;
			rect.Bottom = 500;

			NativeMethods.InvalidateRect(_hwnd, ref rect, true);
		}
	}

	internal struct NativeRegion
	{
		private readonly IntPtr _hrgn;

		public NativeRegion(IntPtr hrgn)
		{
			_hrgn = hrgn;
		}

		public RECT GetRegionRect()
		{
			return NativeMethods.GetRgnBox(_hrgn, out var rect) != CombineRgnResult.ERROR ? rect : new RECT();
		}
	}

	internal class NativeBitmap : IDisposable
	{
		private IntPtr _memoryBlockPointer;

		private NativeBitmap(BitmapSource source)
		{
			Source = source;
			_memoryBlockPointer = IntPtr.Zero;

			var width = source.PixelWidth;
			var height = source.PixelHeight;
			var stride = width * ((source.Format.BitsPerPixel + 7) / 8);

			_memoryBlockPointer = Marshal.AllocHGlobal(height * stride);

			source.CopyPixels(new Int32Rect(0, 0, width, height), _memoryBlockPointer, height * stride, stride);

			Handle = NativeMethods.CreateBitmap(width, height, 1, 32, _memoryBlockPointer);
		}

		public IntPtr Handle { get; private set; }

		public BitmapSource Source { get; }

		public static NativeBitmap Create(BitmapSource bitmap)
		{
			return new NativeBitmap(bitmap);
		}

		private void DisposeInt()
		{
			if (_memoryBlockPointer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(_memoryBlockPointer);

				_memoryBlockPointer = IntPtr.Zero;
			}

			if (Handle != IntPtr.Zero)
			{
				NativeMethods.DeleteObject(Handle);

				Handle = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			DisposeInt();
		}

		~NativeBitmap()
		{
			DisposeInt();
		}
	}
}