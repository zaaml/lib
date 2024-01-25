// // <copyright file="NativeStructs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
// //   Copyright (c) zaaml. All rights reserved.
// // </copyright>

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Zaaml.Platform
{
	
	internal enum MonitorDpiTypes
	{
		EffectiveDPI = 0,
		AngularDPI = 1,
		RawDPI = 2,
	}
	
	internal struct DWM_COLORIZATION_PARAMS
  {
    public uint clrColor;
    public uint clrAfterGlow;
    public uint nIntensity;
    public uint clrAfterGlowBalance;
    public uint clrBlurBalance;
    public uint clrGlassReflectionIntensity;
    public bool fOpaque;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct MENUITEMINFO
  {
    public uint cbSize;
    public uint fMask;
    public uint fType;
    public uint fState;
    public uint wID;
    public IntPtr hSubMenu;
    public IntPtr hbmpChecked;
    public IntPtr hbmpUnchecked;
    public IntPtr dwItemData;
    public string dwTypeData;
    public uint cch;
    public IntPtr hbmpItem;

    // return the size of the structure
    public static uint sizeOf => (uint)Marshal.SizeOf(typeof(MENUITEMINFO));
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct BLENDFUNCTION
  {
    public AC BlendOp;
    public byte BlendFlags;
    public byte SourceConstantAlpha;
    public AC AlphaFormat;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct HIGHCONTRAST
  {
    public int cbSize;
    public HCF dwFlags;
    public IntPtr lpszDefaultScheme;

		public static HIGHCONTRAST Default => new HIGHCONTRAST { cbSize = Marshal.SizeOf(typeof(HIGHCONTRAST)) };
	}

  [StructLayout(LayoutKind.Sequential)]
  internal struct RGBQUAD
  {
    public byte rgbBlue;
    public byte rgbGreen;
    public byte rgbRed;
    public byte rgbReserved;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 2)]
  internal struct BITMAPINFOHEADER
  {
    public int cbSize;
    public int biWidth;
    public int biHeight;
    public short biPlanes;
    public short biBitCount;
    public BI biCompression;
    public int biSizeImage;
    public int biXPelsPerMeter;
    public int biYPelsPerMeter;
    public int biClrUsed;
    public int biClrImportant;

		public static BITMAPINFOHEADER Default => new BITMAPINFOHEADER { cbSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER)) };
	}

  [StructLayout(LayoutKind.Sequential)]
  internal struct BITMAPINFO
  {
    public BITMAPINFOHEADER bmiHeader;
    public RGBQUAD bmiColors;
  }

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  internal struct BITMAP
  {
    public int bmType;
    public int bmWidth;
    public int bmHeight;
    public int bmWidthBytes;
    public int bmPlanes;
    public int bmBitsPixel;
    public IntPtr bmBits;
  }

  internal struct ICONINFO
  {
    public bool fIcon;
    public int xHotspot;
    public int yHotspot;
    public IntPtr hbmMask;
    public IntPtr hbmColor;
  }


  // Win7 only.
  [StructLayout(LayoutKind.Sequential)]
  internal struct CHANGEFILTERSTRUCT
  {
    public uint cbSize;
    public MSGFLTINFO ExtStatus;

		public static CHANGEFILTERSTRUCT Default => new CHANGEFILTERSTRUCT { cbSize = (uint)Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT)) };
	}

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct CREATESTRUCT
  {
    public IntPtr lpCreateParams;
    public IntPtr hInstance;
    public IntPtr hMenu;
    public IntPtr hwndParent;
    public int cy;
    public int cx;
    public int y;
    public int x;
    public WS style;
    [MarshalAs(UnmanagedType.LPWStr)] public string lpszName;
    [MarshalAs(UnmanagedType.LPWStr)] public string lpszClass;
    public WS_EX dwExStyle;
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
  internal struct SHFILEOPSTRUCT
  {
    public IntPtr hwnd;
    [MarshalAs(UnmanagedType.U4)] public FO wFunc;
    // double-null terminated arrays of LPWSTRS
    public string pFrom;
    public string pTo;
    [MarshalAs(UnmanagedType.U2)] public FOF fFlags;
    [MarshalAs(UnmanagedType.Bool)] public int fAnyOperationsAborted;
    public IntPtr hNameMappings;
    public string lpszProgressTitle;
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  internal struct SHFILEINFO
  {
    public IntPtr hIcon;
    public int iIcon;
    public uint dwAttributes;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szDisplayName;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
    public string szTypeName;

    public static SHFILEINFO Default = new SHFILEINFO
    {
      hIcon = IntPtr.Zero,
      iIcon = 0,
      dwAttributes = 0,
      szDisplayName = "",
      szTypeName = "",
    };
  };

  [StructLayout(LayoutKind.Sequential)]
  internal struct TITLEBARINFO
  {
    public int cbSize;
    public RECT rcTitleBar;
    public STATE_SYSTEM rgstate_TitleBar;
    public STATE_SYSTEM rgstate_Reserved;
    public STATE_SYSTEM rgstate_MinimizeButton;
    public STATE_SYSTEM rgstate_MaximizeButton;
    public STATE_SYSTEM rgstate_HelpButton;
    public STATE_SYSTEM rgstate_CloseButton;

		public static TITLEBARINFO Default => new TITLEBARINFO { cbSize = Marshal.SizeOf(typeof(TITLEBARINFO)) };
	}

  // New to Vista.
  [StructLayout(LayoutKind.Sequential)]
  internal struct TITLEBARINFOEX
  {
    public int cbSize;
    public RECT rcTitleBar;
    public STATE_SYSTEM rgstate_TitleBar;
    public STATE_SYSTEM rgstate_Reserved;
    public STATE_SYSTEM rgstate_MinimizeButton;
    public STATE_SYSTEM rgstate_MaximizeButton;
    public STATE_SYSTEM rgstate_HelpButton;
    public STATE_SYSTEM rgstate_CloseButton;
    public RECT rgrect_TitleBar;
    public RECT rgrect_Reserved;
    public RECT rgrect_MinimizeButton;
    public RECT rgrect_MaximizeButton;
    public RECT rgrect_HelpButton;
    public RECT rgrect_CloseButton;

		public static TITLEBARINFOEX Default => new TITLEBARINFOEX { cbSize = Marshal.SizeOf(typeof(TITLEBARINFOEX)) };
	}

  [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
  [StructLayout(LayoutKind.Sequential)]
  internal struct NOTIFYICONDATA
  {
    public int cbSize;
    public IntPtr hwnd;
    public int uID;
    public int uFlags;
    public int uCallbackMessage;
    public IntPtr hIcon;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szTip;
    public int dwState;
    public int dwStateMask;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string szInfo;
    public int uVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string szInfoTitle;
    public int dwInfoFlags;

    public static NOTIFYICONDATA Default => new NOTIFYICONDATA { cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA)) };
  }

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct LOGFONT
  {
    public int lfHeight;
    public int lfWidth;
    public int lfEscapement;
    public int lfOrientation;
    public int lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string lfFaceName;
  }

	[StructLayout(LayoutKind.Sequential)]
	internal struct LOGBRUSH
	{
		int Style;
		COLORREF Color;
		int Hatch;
	}

	[StructLayout(LayoutKind.Sequential)]
  internal struct MINMAXINFO
  {
    public POINT ptReserved;
    public POINT ptMaxSize;
    public POINT ptMaxPosition;
    public POINT ptMinTrackSize;
    public POINT ptMaxTrackSize;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct NONCLIENTMETRICS
  {
    public int cbSize;
    public int iBorderWidth;
    public int iScrollWidth;
    public int iScrollHeight;
    public int iCaptionWidth;
    public int iCaptionHeight;
    public LOGFONT lfCaptionFont;
    public int iSmCaptionWidth;
    public int iSmCaptionHeight;
    public LOGFONT lfSmCaptionFont;
    public int iMenuWidth;
    public int iMenuHeight;
    public LOGFONT lfMenuFont;
    public LOGFONT lfStatusFont;
    public LOGFONT lfMessageFont;
    // Vista only
    public int iPaddedBorderWidth;

    public static NONCLIENTMETRICS VistaMetricsStruct
      => new NONCLIENTMETRICS {cbSize = Marshal.SizeOf(typeof (NONCLIENTMETRICS))};

    public static NONCLIENTMETRICS XPMetricsStruct
      => new NONCLIENTMETRICS {cbSize = Marshal.SizeOf(typeof (NONCLIENTMETRICS)) - sizeof (int)};
  }

  [StructLayout(LayoutKind.Explicit)]
  internal struct WTA_OPTIONS
  {
    public const uint Size = 8;

    [FieldOffset(0)] public WTNCA dwFlags;

    [FieldOffset(4)] public WTNCA dwMask;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct MARGINS
  {
    public int cxLeftWidth;
    public int cxRightWidth;
    public int cyTopHeight;
    public int cyBottomHeight;
  };

  [StructLayout(LayoutKind.Sequential)]
  internal struct MONITORINFO
  {
	  public int cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public int dwFlags;
    //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    //public string DeviceName;
    //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    //private string DeviceName1;

    public static MONITORINFO Default => new MONITORINFO { cbSize = Marshal.SizeOf(typeof (MONITORINFO)) };
	}


  //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  //internal struct MonitorInfoEx
  //{
  //  public int Size;
  //  public RECT Monitor;
  //  public RECT WorkArea;
  //  public uint Flags;
  //  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
  //  public string DeviceName;

  //  public void Init()
  //  {
  //    Size = 40 + 2 * 32;
  //    DeviceName = string.Empty;
  //  }
  //}

  [StructLayout(LayoutKind.Sequential)]
  internal struct POINT
  {
    public int x;
    public int y;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal class RefPOINT
  {
    public int x;
    public int y;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct RECT
  {
    private int _left;
    private int _top;
    private int _right;
    private int _bottom;

    public void Offset(int dx, int dy)
    {
      _left += dx;
      _top += dy;
      _right += dx;
      _bottom += dy;
    }

    public int Left
    {
      get => _left;
      set => _left = value;
    }

    public int Right
    {
      get => _right;
      set => _right = value;
    }

    public int Top
    {
      get => _top;
      set => _top = value;
    }

    public int Bottom
    {
      get => _bottom;
      set => _bottom = value;
    }

    public int Width => _right - _left;

    public int Height => _bottom - _top;

    public POINT Position => new POINT {x = _left, y = _top};

    public SIZE Size => new SIZE {cx = Width, cy = Height};

    public static RECT Union(RECT rect1, RECT rect2)
    {
      return new RECT
      {
        Left = Math.Min(rect1.Left, rect2.Left),
        Top = Math.Min(rect1.Top, rect2.Top),
        Right = Math.Max(rect1.Right, rect2.Right),
        Bottom = Math.Max(rect1.Bottom, rect2.Bottom)
      };
    }

    public override bool Equals(object obj)
    {
      try
      {
        var rc = (RECT) obj;
        return rc._bottom == _bottom
               && rc._left == _left
               && rc._right == _right
               && rc._top == _top;
      }
      catch (InvalidCastException)
      {
        return false;
      }
    }

    public override int GetHashCode()
    {
      return (_left << 16 | PlatformUtil.LOWORD(_right)) ^ (_top << 16 | PlatformUtil.LOWORD(_bottom));
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  internal class RefRECT
  {
    private int _left;
    private int _top;
    private int _right;
    private int _bottom;

    public RefRECT(int left, int top, int right, int bottom)
    {
      _left = left;
      _top = top;
      _right = right;
      _bottom = bottom;
    }

    public int Width => _right - _left;

    public int Height => _bottom - _top;

    public int Left
    {
      get => _left;
      set => _left = value;
    }

    public int Right
    {
      get => _right;
      set => _right = value;
    }

    public int Top
    {
      get => _top;
      set => _top = value;
    }

    public int Bottom
    {
      get => _bottom;
      set => _bottom = value;
    }

    public void Offset(int dx, int dy)
    {
      _left += dx;
      _top += dy;
      _right += dx;
      _bottom += dy;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct SIZE
  {
    public int cx;
    public int cy;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct StartupOutput
  {
    public IntPtr hook;
    public IntPtr unhook;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal class StartupInput
  {
    public int GdiplusVersion = 1;
    public IntPtr DebugEventCallback;
    public bool SuppressBackgroundThread;
    public bool SuppressExternalCodecs;
  }

	[StructLayout(LayoutKind.Sequential)]
	internal struct WINDOWPLACEMENT
	{
		public int cbSize;
		public int Flags;
		public SW ShowCmd;
		public POINT MinPosition;
		public POINT MaxPosition;
		public RECT NormalPosition;

		public static WINDOWPLACEMENT Default => new WINDOWPLACEMENT { cbSize = Marshal.SizeOf(typeof(WINDOWPLACEMENT)) };
	}


	[StructLayout(LayoutKind.Sequential)]
  internal struct WINDOWPOS
  {
    public IntPtr hwnd;
    public IntPtr hwndInsertAfter;
    public int x;
    public int y;
    public int cx;
    public int cy;
    public int flags;

    public RECT GetRect()
    {
      return new RECT
      {
        Left = x,
        Top = y,
        Right = x + cx,
        Bottom = y + cy
      };
    }
  }

	[StructLayout(LayoutKind.Sequential)]
	internal struct WNDCLASSEX
	{
		[MarshalAs(UnmanagedType.U4)]
		public int cbSize;
		[MarshalAs(UnmanagedType.U4)]
		public int style;
		public WndProc lpfnWndProc;
		public int cbClsExtra;
		public int cbWndExtra;
		public IntPtr hInstance;
		public IntPtr hIcon;
		public IntPtr hCursor;
		public IntPtr hbrBackground;
		public string lpszMenuName;
		public string lpszClassName;
		public IntPtr hIconSm;
	}


	[StructLayout(LayoutKind.Sequential)]
  internal struct MOUSEINPUT
  {
    public int dx;
    public int dy;
    public int mouseData;
    public int dwFlags;
    public int time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct INPUT
  {
    public uint type;
    public MOUSEINPUT mi;
  };

  [StructLayout(LayoutKind.Sequential)]
  internal struct UNSIGNED_RATIO
  {
    public uint uiNumerator;
    public uint uiDenominator;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct DWM_TIMING_INFO
  {
    public int cbSize;
    public UNSIGNED_RATIO rateRefresh;
    public ulong qpcRefreshPeriod;
    public UNSIGNED_RATIO rateCompose;
    public ulong qpcVBlank;
    public ulong cRefresh;
    public uint cDXRefresh;
    public ulong qpcCompose;
    public ulong cFrame;
    public uint cDXPresent;
    public ulong cRefreshFrame;
    public ulong cFrameSubmitted;
    public uint cDXPresentSubmitted;
    public ulong cFrameConfirmed;
    public uint cDXPresentConfirmed;
    public ulong cRefreshConfirmed;
    public uint cDXRefreshConfirmed;
    public ulong cFramesLate;
    public uint cFramesOutstanding;
    public ulong cFrameDisplayed;
    public ulong qpcFrameDisplayed;
    public ulong cRefreshFrameDisplayed;
    public ulong cFrameComplete;
    public ulong qpcFrameComplete;
    public ulong cFramePending;
    public ulong qpcFramePending;
    public ulong cFramesDisplayed;
    public ulong cFramesComplete;
    public ulong cFramesPending;
    public ulong cFramesAvailable;
    public ulong cFramesDropped;
    public ulong cFramesMissed;
    public ulong cRefreshNextDisplayed;
    public ulong cRefreshNextPresented;
    public ulong cRefreshesDisplayed;
    public ulong cRefreshesPresented;
    public ulong cRefreshStarted;
    public ulong cPixelsReceived;
    public ulong cPixelsDrawn;
    public ulong cBuffersEmpty;

		public static DWM_TIMING_INFO Default => new DWM_TIMING_INFO { cbSize = Marshal.SizeOf(typeof(DWM_TIMING_INFO)) };
	}

  [StructLayout(LayoutKind.Sequential)]
  internal struct Msg
  {
    public IntPtr Hwnd;
    public int Message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint Time;
    public POINT Point;
  }

  internal static class SpecialHwnd
  {
    #region Static Fields

    public static IntPtr NoTopMost = new IntPtr(-2);
    public static IntPtr TopMost = new IntPtr(-1);
    public static IntPtr Top = new IntPtr(0);
    public static IntPtr Bottom = new IntPtr(1);

    #endregion
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct PaintStruct
  {
    public IntPtr hdc;
    public bool fErase;
    public RECT rcPaint;
    public bool fRestore;
    public bool fIncUpdate;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] rgbReserved;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct WindowInfo
  {
    public uint cbSize;
    public RECT WindowRect;
    public RECT ClientRect;
    public uint Style;
    public uint ExStyle;
    public uint WindowStatus;
    public uint CxWindowBorders;
    public uint CyWindowBorders;
    public ushort AtomWindowType;
    public ushort CreatorVersion;

		public static WindowInfo Default => new WindowInfo { cbSize = (uint)Marshal.SizeOf(typeof(WindowInfo)) };
	}

	[Flags]
  internal enum LWA : uint
  {
	  ALPHA = 0x00000002,
	  COLORKEY = 0x00000001
	}

	[StructLayout(LayoutKind.Explicit, Size = 4)]
  internal struct COLORREF
  {
    public COLORREF(byte r, byte g, byte b)
    {
      Value = 0;
      R = r;
      G = g;
      B = b;
    }

    public COLORREF(uint value)
    {
      R = 0;
      G = 0;
      B = 0;
      Value = value & 0x00FFFFFF;
    }

    [FieldOffset(0)]
    public byte R;
    [FieldOffset(1)]
    public byte G;
    [FieldOffset(2)]
    public byte B;

    [FieldOffset(0)]
    public uint Value;
  }

	internal enum Facility
	{
		/// <summary>FACILITY_NULL</summary>
		Null = 0,
		/// <summary>FACILITY_RPC</summary>
		Rpc = 1,
		/// <summary>FACILITY_DISPATCH</summary>
		Dispatch = 2,
		/// <summary>FACILITY_STORAGE</summary>
		Storage = 3,
		/// <summary>FACILITY_ITF</summary>
		Itf = 4,
		/// <summary>FACILITY_WIN32</summary>
		Win32 = 7,
		/// <summary>FACILITY_WINDOWS</summary>
		Windows = 8,
		/// <summary>FACILITY_CONTROL</summary>
		Control = 10,
		/// <summary>MSDN doced facility code for ESE errors.</summary>
		Ese = 0xE5E,
		/// <summary>FACILITY_WINCODEC (WIC)</summary>
		WinCodec = 0x898,
	}

	/// <summary>Wrapper for HRESULT status codes.</summary>
	[StructLayout(LayoutKind.Explicit)]
	internal struct HRESULT
	{
		[FieldOffset(0)]
		private readonly uint _value;


		public static readonly HRESULT S_OK = new HRESULT(0x00000000);
		public static readonly HRESULT S_FALSE = new HRESULT(0x00000001);
		public static readonly HRESULT E_PENDING = new HRESULT(0x8000000A);
		public static readonly HRESULT E_NOTIMPL = new HRESULT(0x80004001);
		public static readonly HRESULT E_NOINTERFACE = new HRESULT(0x80004002);
		public static readonly HRESULT E_POINTER = new HRESULT(0x80004003);
		public static readonly HRESULT E_ABORT = new HRESULT(0x80004004);
		public static readonly HRESULT E_FAIL = new HRESULT(0x80004005);
		public static readonly HRESULT E_UNEXPECTED = new HRESULT(0x8000FFFF);
		public static readonly HRESULT STG_E_INVALIDFUNCTION = new HRESULT(0x80030001);
		public static readonly HRESULT REGDB_E_CLASSNOTREG = new HRESULT(0x80040154);
		public static readonly HRESULT DESTS_E_NO_MATCHING_ASSOC_HANDLER = new HRESULT(0x80040F03);
		public static readonly HRESULT DESTS_E_NORECDOCS = new HRESULT(0x80040F04);
		public static readonly HRESULT DESTS_E_NOTALLCLEARED = new HRESULT(0x80040F05);
		public static readonly HRESULT E_ACCESSDENIED = new HRESULT(0x80070005);
		public static readonly HRESULT E_OUTOFMEMORY = new HRESULT(0x8007000E);
		public static readonly HRESULT E_INVALIDARG = new HRESULT(0x80070057);
		public static readonly HRESULT INTSAFE_E_ARITHMETIC_OVERFLOW = new HRESULT(0x80070216);
		public static readonly HRESULT COR_E_OBJECTDISPOSED = new HRESULT(0x80131622);
		public static readonly HRESULT WC_E_GREATERTHAN = new HRESULT(0xC00CEE23);
		public static readonly HRESULT WC_E_SYNTAX = new HRESULT(0xC00CEE2D);

		public HRESULT(uint i)
		{
			_value = i;
		}

		public static HRESULT Make(bool severe, Facility facility, int code)
		{
			return new HRESULT((uint)((severe ? (1 << 31) : 0) | ((int)facility << 16) | code));
		}

		public Facility Facility => GetFacility((int)_value);

		public static Facility GetFacility(int errorCode)
		{
			return (Facility)((errorCode >> 16) & 0x1fff);
		}


		public int Code => GetCode((int)_value);

		public static int GetCode(int error)
		{
			return (int)(error & 0xFFFF);
		}

		#region Object class override members



		public override bool Equals(object obj)
		{
			try
			{
				return ((HRESULT)obj)._value == _value;
			}
			catch (InvalidCastException)
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		#endregion

		public static bool operator ==(HRESULT hrLeft, HRESULT hrRight)
		{
			return hrLeft._value == hrRight._value;
		}

		public static bool operator !=(HRESULT hrLeft, HRESULT hrRight)
		{
			return !(hrLeft == hrRight);
		}

		public bool Succeeded => (int)_value >= 0;

		public bool Failed => (int)_value < 0;

		public void ThrowIfFailed()
		{
			ThrowIfFailed(null);
		}

		public void ThrowIfFailed(string message)
		{
			if (Failed)
			{
				if (string.IsNullOrEmpty(message))
				{
					message = ToString();
				}
#if DEBUG
				else
				{
					message += " (" + ToString() + ")";
				}
#endif

				Exception e = Marshal.GetExceptionForHR((int)_value, new IntPtr(-1));


				if (e.GetType() == typeof(COMException))
				{
					switch (Facility)
					{
						case Facility.Win32:
							e = new Win32Exception(Code, message);
							break;
						default:
							e = new COMException(message, (int)_value);
							break;
					}
				}
				else
				{
					ConstructorInfo cons = e.GetType().GetConstructor(new[] { typeof(string) });
					if (null != cons)
					{
						e = cons.Invoke(new object[] { message }) as Exception;
					}
				}
				throw e;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CWPSTRUCT
	{
		public IntPtr lparam;
		public IntPtr wparam;
		public int message;
		public IntPtr hwnd;
	}


	[StructLayout(LayoutKind.Sequential)]
	internal struct MOUSEHOOKSTRUCT
	{
		public POINT pt;
		public IntPtr hwnd;
		public uint wHitTestCode;
		public IntPtr dwExtraInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct NCCALCSIZE_PARAMS
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public RECT[] rgrc;
		public WINDOWPOS lppos;
	}
}