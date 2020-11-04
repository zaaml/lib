// <copyright file="NativeBrush.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Platform
{
	internal sealed class NativeBrush : IDisposable
	{
		private IntPtr _hBrush;

		public IntPtr HBrush => _hBrush;

		public static NativeBrush FromColor(COLORREF colorRef)
		{
			return new NativeBrush
			{
				_hBrush = NativeMethods.CreateSolidBrush(colorRef.Value)
			};
		}

		public static explicit operator IntPtr(NativeBrush brush)
		{
			return brush._hBrush;
		}

		public void Release()
		{
			if (_hBrush == IntPtr.Zero)
				return;

			NativeMethods.DeleteObject(_hBrush);

			_hBrush = IntPtr.Zero;
		}

		public void Dispose()
		{
			Release();
		}

		~NativeBrush()
		{
			Release();
		}
	}
}