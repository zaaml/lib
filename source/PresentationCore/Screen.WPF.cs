// <copyright file="Screen.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using Zaaml.Platform;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore
{
	internal sealed class Screen
	{
		private const int PrimaryMonitor = unchecked((int) 0xFAADD00D);
		private const uint MONITORINFOF_PRIMARY = 1;
		private static readonly object SyncLock = new();
		private static int _desktopChangedCount = -1;
		private static readonly bool MultiMonitorSupport = NativeMethods.GetSystemMetrics(SM.CMONITORS) != 0;
		private static Screen[] _screens;

		private readonly IntPtr _hMonitor;
		private readonly bool _primary;
		private int _currentDesktopChangedCount = -1;
		private Rect _workingArea = Rect.Empty;

		internal Screen(IntPtr monitor)
		{
			if (MultiMonitorSupport == false || monitor == (IntPtr) PrimaryMonitor)
			{
				Bounds = new Rect(new Point(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop), new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight));
				_primary = true;
			}
			else
			{
				var monitorInfo = NativeMethods.GetMonitorInfo(monitor);

				Bounds = monitorInfo.rcMonitor.ToPresentationRect().FromDeviceToLogical();
				_primary = (monitorInfo.dwFlags & MONITORINFOF_PRIMARY) != 0;
			}

			_hMonitor = monitor;
		}

		public static Screen[] AllScreens
		{
			get
			{
				if (_screens != null)
					return _screens;

				if (MultiMonitorSupport)
				{
					MonitorEnumCallback closure = new MonitorEnumCallback();
					NativeMethods.MonitorEnumProc proc = closure.Callback;
					NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, proc, IntPtr.Zero);

					if (closure.Screens.Count > 0)
					{
						Screen[] temp = new Screen[closure.Screens.Count];
						closure.Screens.CopyTo(temp, 0);
						_screens = temp;
					}
					else
						_screens = new[] {new Screen((IntPtr) PrimaryMonitor)};
				}
				else
					_screens = new[] {PrimaryScreen};

				SystemEvents.DisplaySettingsChanging += OnDisplaySettingsChanging;

				return _screens;
			}
		}

		public Rect Bounds { get; }

		private static int DesktopChangedCount
		{
			get
			{
				if (_desktopChangedCount != -1) 
					return _desktopChangedCount;

				lock (SyncLock)
				{
					if (_desktopChangedCount != -1) return _desktopChangedCount;
					SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;

					_desktopChangedCount = 0;
				}

				return _desktopChangedCount;
			}
		}

		public DpiValue Dpi => DpiUtils.GetMonitorDpi(_hMonitor);

		public int DpiX => Dpi.DpiX;

		public int DpiY => Dpi.DpiY;

		public bool Primary => _primary;

		public static Screen PrimaryScreen => MultiMonitorSupport ? AllScreens.FirstOrDefault(t => t._primary) : new Screen((IntPtr) PrimaryMonitor);

		public static Rect VirtualScreenRect => new(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);

		public static Size VirtualScreenSize => new(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);

		public Rect WorkingArea
		{
			get
			{
				if (_currentDesktopChangedCount == DesktopChangedCount)
					return _workingArea;

				Interlocked.Exchange(ref _currentDesktopChangedCount, DesktopChangedCount);

				if (!MultiMonitorSupport || _hMonitor == (IntPtr) PrimaryMonitor)
					_workingArea = SystemParameters.WorkArea;
				else
				{
					var monitorInfo = NativeMethods.GetMonitorInfo(_hMonitor);

					_workingArea = monitorInfo.rcWork.ToPresentationRect().FromDeviceToLogical();
				}

				return _workingArea;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Screen))
				return false;

			var comp = (Screen) obj;

			return _hMonitor == comp._hMonitor;
		}

		public static Screen FromElement(UIElement element)
		{
			var hwndSource = (HwndSource) PresentationSource.FromVisual(element);

			return hwndSource == null ? PrimaryScreen : FromHandleInternal(hwndSource.Handle);
		}

		private static Screen FromHandleInternal(IntPtr hwnd)
		{
			return MultiMonitorSupport ? new Screen(NativeMethods.MonitorFromWindow(hwnd, MonitorOptions.MONITOR_DEFAULTTONEAREST)) : PrimaryScreen;
		}

		public static Screen FromPoint(Point point)
		{
			if (MultiMonitorSupport == false)
				return new Screen((IntPtr) PrimaryMonitor);

			var pt = point.FromLogicalToDevice().ToPlatformPoint();

			return new Screen(NativeMethods.MonitorFromPoint(pt, MonitorOptions.MONITOR_DEFAULTTONEAREST));
		}

		public static Screen FromRectangle(Rect rect)
		{
			if (MultiMonitorSupport == false)
				return new Screen((IntPtr) PrimaryMonitor);

			var rc = rect.FromLogicalToDevice().ToPlatformRect();

			return new Screen(NativeMethods.MonitorFromRect(ref rc, MonitorOptions.MONITOR_DEFAULTTONEAREST));
		}

		public static Rect GetBounds(Point pt)
		{
			return FromPoint(pt).Bounds;
		}

		public static Rect GetBounds(Rect rect)
		{
			return FromRectangle(rect).Bounds;
		}

		public override int GetHashCode()
		{
			return (int) _hMonitor;
		}

		public static Rect GetWorkingArea(Point pt)
		{
			return FromPoint(pt).WorkingArea;
		}

		public static Rect GetWorkingArea(Rect rect)
		{
			return FromRectangle(rect).WorkingArea;
		}

		private static void OnDisplaySettingsChanging(object sender, EventArgs e)
		{
			SystemEvents.DisplaySettingsChanging -= OnDisplaySettingsChanging;

			_screens = null;
		}

		private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category == UserPreferenceCategory.Desktop)
			{
				Interlocked.Increment(ref _desktopChangedCount);
			}
		}

		private class MonitorEnumCallback
		{
			public readonly ArrayList Screens = new();

			public bool Callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lparam)
			{
				Screens.Add(new Screen(monitor));

				return true;
			}
		}

		public static event EventHandler VirtualScreenSizeChanged
		{
			// ReSharper disable ValueParameterNotUsed
			add { }
			remove { }
			// ReSharper restore ValueParameterNotUsed
		}
	}
}