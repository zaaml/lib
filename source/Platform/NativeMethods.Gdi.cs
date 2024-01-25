// <copyright file="NativeMethods.Gdi.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

#if !NET5_0_OR_GREATER
using System.Runtime.ConstrainedExecution;
#endif

namespace Zaaml.Platform
{
	internal static partial class NativeMethods
	{
		private const string Gdi32 = "gdi32.dll";

		[SecurityCritical]
		[DllImport(Gdi32, EntryPoint = "CreateRectRgn", SetLastError = true)]
		private static extern IntPtr _CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		[SecurityCritical]
		[DllImport(Gdi32, EntryPoint = "CreateRectRgnIndirect", SetLastError = true)]
		private static extern IntPtr _CreateRectRgnIndirect([In] ref RECT lprc);

		[SecurityCritical]
		[DllImport(Gdi32, EntryPoint = "CreateRoundRectRgn", SetLastError = true)]
		private static extern IntPtr _CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		[DllImport(Gdi32, SetLastError = true)]
		public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

		[SecurityCritical]
		[DllImport(Gdi32)]
		public static extern CombineRgnResult CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, RGN fnCombineMode);

		[DllImport(Gdi32)]
		public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

		[DllImport(Gdi32)]
		public static extern IntPtr CreateBitmapIndirect([In] ref BITMAP lpbm);

		[DllImport(Gdi32)]
		public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

		[DllImport(Gdi32, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

		[DllImport(Gdi32)]
		public static extern IntPtr CreatePen(PenStyle fnPenStyle, int nWidth, uint crColor);

		[SecurityCritical]
		public static IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect)
		{
			var ret = _CreateRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect);
			if (IntPtr.Zero == ret)
				throw new Win32Exception();

			return ret;
		}

		[SecurityCritical]
		public static IntPtr CreateRectRgnIndirect(RECT lprc)
		{
			var ret = _CreateRectRgnIndirect(ref lprc);
			if (IntPtr.Zero == ret)
				throw new Win32Exception();

			return ret;
		}

		[SecurityCritical]
		public static IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse)
		{
			var ret = _CreateRoundRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect, nWidthEllipse, nHeightEllipse);
			if (IntPtr.Zero == ret)
				throw new Win32Exception();

			return ret;
		}

		[DllImport(Gdi32)]
		public static extern IntPtr CreateSolidBrush(uint crColor);

		[DllImport(Gdi32)]
		public static extern bool DeleteDC([In] IntPtr hdc);

		[SecurityCritical]
		[DllImport(Gdi32)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport(Gdi32)]
		public static extern int ExtSelectClipRgn(IntPtr hdc, IntPtr hrgn, CombineRgnStyles mode);

		[DllImport(Gdi32)]
		public static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);

		[DllImport(Gdi32)]
		public static extern int GetObject(IntPtr hgdiobj, int cbBuffer, IntPtr lpvObject);


		[DllImport(Gdi32)]
		public static extern CombineRgnResult GetRgnBox(IntPtr hrgn, out RECT lprc);

		[DllImport(Gdi32)]
		public static extern IntPtr GetStockObject(StockObjects fnObject);

		[DllImport(Gdi32)]
		public static extern bool PaintRgn(IntPtr hdc, IntPtr hrgn);

		[DllImport(Gdi32)]
		public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		[DllImport(Gdi32)]
		public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

		[DllImport(Gdi32, EntryPoint = "SelectObject")]
		public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
	}

	[SecurityCritical]
	internal sealed class SafeHBITMAP : SafeHandleZeroOrMinusOneIsInvalid
	{
		[SecurityCritical]
		private SafeHBITMAP() : base(true)
		{
		}

		[SecurityCritical]
#if !NET5_0_OR_GREATER
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
		protected override bool ReleaseHandle()
		{
			return NativeMethods.DeleteObject(handle);
		}
	}

	internal enum TernaryRasterOperations : uint
	{
		/// <summary>dest = source</summary>
		SRCCOPY = 0x00CC0020,

		/// <summary>dest = source OR dest</summary>
		SRCPAINT = 0x00EE0086,

		/// <summary>dest = source AND dest</summary>
		SRCAND = 0x008800C6,

		/// <summary>dest = source XOR dest</summary>
		SRCINVERT = 0x00660046,

		/// <summary>dest = source AND (NOT dest)</summary>
		SRCERASE = 0x00440328,

		/// <summary>dest = (NOT source)</summary>
		NOTSRCCOPY = 0x00330008,

		/// <summary>dest = (NOT src) AND (NOT dest)</summary>
		NOTSRCERASE = 0x001100A6,

		/// <summary>dest = (source AND pattern)</summary>
		MERGECOPY = 0x00C000CA,

		/// <summary>dest = (NOT source) OR dest</summary>
		MERGEPAINT = 0x00BB0226,

		/// <summary>dest = pattern</summary>
		PATCOPY = 0x00F00021,

		/// <summary>dest = DPSnoo</summary>
		PATPAINT = 0x00FB0A09,

		/// <summary>dest = pattern XOR dest</summary>
		PATINVERT = 0x005A0049,

		/// <summary>dest = (NOT dest)</summary>
		DSTINVERT = 0x00550009,

		/// <summary>dest = BLACK</summary>
		BLACKNESS = 0x00000042,

		/// <summary>dest = WHITE</summary>
		WHITENESS = 0x00FF0062,

		/// <summary>
		///   Capture window as seen on screen.  This includes layered windows
		///   such as WPF windows with AllowsTransparency="true"
		/// </summary>
		CAPTUREBLT = 0x40000000
	}
}