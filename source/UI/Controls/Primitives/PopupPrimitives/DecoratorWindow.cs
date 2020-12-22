// <copyright file="DecoratorWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Zaaml.Platform;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal sealed class DecoratorWindow : DependencyObject
	{
		public static readonly DependencyProperty ChildProperty = DPM.Register<FrameworkElement, DecoratorWindow>
			("Child", default, d => d.OnChildPropertyChangedPrivate);

		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, DecoratorWindow>
			("IsOpen", default, d => d.OnIsOpenPropertyChangedPrivate);

		public static readonly DependencyProperty LeftProperty = DPM.Register<double, DecoratorWindow>
			("Left", default, d => d.OnLeftPropertyChangedPrivate);

		public static readonly DependencyProperty TopProperty = DPM.Register<double, DecoratorWindow>
			("Top", default, d => d.OnTopPropertyChangedPrivate);

		public static readonly DependencyProperty WidthProperty = DPM.Register<double, DecoratorWindow>
			("Width", default, d => d.OnWidthPropertyChangedPrivate);

		public static readonly DependencyProperty HeightProperty = DPM.Register<double, DecoratorWindow>
			("Height", default, d => d.OnHeightPropertyChangedPrivate);

		private readonly HwndDpiChangedEventHandler _dpiChangedHandler;
		private readonly HwndSourceHook _messageFilterHook;

		private readonly AutoResizedEventHandler _onAutoResizeHandler;

		public DecoratorWindow()
		{
			DecoratorRoot = new DecoratorWindowRoot {Background = null};

			_onAutoResizeHandler = OnAutoResize;
			_dpiChangedHandler = OnDpiChanged;
			_messageFilterHook = MessageFilter;
		}

		public FrameworkElement Child
		{
			get => (FrameworkElement) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		private DecoratorWindowRoot DecoratorRoot { get; }

		public double Height
		{
			get => (double) GetValue(HeightProperty);
			set => SetValue(HeightProperty, value);
		}

		private HwndSource HwndSource { get; set; }

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		internal static bool IsPerMonitorDpiScalingActive
		{
			get
			{
				if (IsProcessPerMonitorDpiAware.HasValue)
				{
					return IsProcessPerMonitorDpiAware.Value;
				}

				var proc = Process.GetCurrentProcess();

				if (NativeMethods.GetProcessDpiAwareness(proc.Handle, out var value) == 0)
				{
					IsProcessPerMonitorDpiAware = value == PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;

					return IsProcessPerMonitorDpiAware.Value;
				}

				return false;
			}
		}

		private static bool? IsProcessPerMonitorDpiAware { get; set; }

		public double Left
		{
			get => (double) GetValue(LeftProperty);
			set => SetValue(LeftProperty, value);
		}

		public double Top
		{
			get => (double) GetValue(TopProperty);
			set => SetValue(TopProperty, value);
		}

		public double Width
		{
			get => (double) GetValue(WidthProperty);
			set => SetValue(WidthProperty, value);
		}

		private HwndSource BuildWindow()
		{
			var ws = WS.CLIPSIBLINGS | WS.POPUP;
			var wsEx = WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE | WS_EX.TOPMOST | WS_EX.NOACTIVATE | WS_EX.TRANSPARENT;

			var param = new HwndSourceParameters(string.Empty)
			{
				WindowClassStyle = 0,
				WindowStyle = unchecked((int) ws),
				ExtendedWindowStyle = (int) wsEx,
				UsesPerPixelOpacity = true
			};

			param.SetPosition((int) Left, (int) Top);
			param.SetSize((int) Width, (int) Height);

			var newWindow = new HwndSource(param);

			newWindow.AddHook(_messageFilterHook);

			newWindow.AutoResized += _onAutoResizeHandler;
			newWindow.DpiChanged += _dpiChangedHandler;
			newWindow.SizeToContent = SizeToContent.Manual;

			var hwndTarget = newWindow.CompositionTarget;

			hwndTarget.BackgroundColor = Colors.Transparent;

			newWindow.RootVisual = DecoratorRoot;
			UpdateChild();

			return newWindow;
		}

		internal void DestroyWindow()
		{
			var hwnd = HwndSource;

			HwndSource = null;

			if (hwnd == null || hwnd.IsDisposed)
				return;

			hwnd.AutoResized -= _onAutoResizeHandler;
			hwnd.DpiChanged -= _dpiChangedHandler;
			hwnd.RemoveHook(_messageFilterHook);
			hwnd.RootVisual = null;
			hwnd.Dispose();
		}

		private void EnsureWindow()
		{
			if (IsOpen == false)
				return;

			if (IsPerMonitorDpiScalingActive)
				DestroyWindow();

			HwndSource ??= BuildWindow();
		}

		private object HandleDeactivateApp(object arg)
		{
			this.SetCurrentValueInternal(IsOpenProperty, KnownBoxes.BoolFalse);

			return null;
		}

		private IntPtr MessageFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			var wm = (WM) msg;

			if (wm == WM.MOUSEACTIVATE)
			{
				handled = true;

				// MA_NOACTIVATE
				return new IntPtr(3);
			}

			if (wm == WM.WINDOWPOSCHANGING)
			{
			}

			if (wm == WM.ACTIVATEAPP)
			{
				if (wParam == IntPtr.Zero)
					Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(HandleDeactivateApp), null);
			}

			return IntPtr.Zero;
		}

		private void OnAutoResize(object sender, AutoResizedEventArgs e)
		{
		}

		private void OnChildPropertyChangedPrivate(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (HwndSource == null)
				return;

			UpdateChild();
		}

		private void OnDpiChanged(object sender, HwndDpiChangedEventArgs e)
		{
			if (IsOpen)
				e.Handled = true;
		}

		private void OnHeightPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateSize();
		}

		private void OnIsOpenPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			EnsureWindow();

			UpdateVisibility();
		}

		private void OnLeftPropertyChangedPrivate(double oldValue, double newValue)
		{
			if (HwndSource != null)
				UpdatePosition();
		}

		private void OnTopPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdatePosition();
		}

		private void OnWidthPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateSize();
		}

		private void UpdateChild()
		{
			DecoratorRoot.Children.Clear();

			var child = Child;

			if (child != null)
				DecoratorRoot.Children.Add(child);
		}

		private void UpdatePosition()
		{
			if (HwndSource == null)
				return;

			NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, (int) Left, (int) Top, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOOWNERZORDER | SWP.NOZORDER | SWP.NOREDRAW);
		}

		private void UpdateSize()
		{
			if (HwndSource == null)
				return;

			NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, 0, 0, (int) Width, (int) Height, SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOZORDER);
		}

		private void UpdateVisibility()
		{
			if (HwndSource == null)
				return;

			if (IsOpen)
				NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.SHOWWINDOW);
			else
				NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.HIDEWINDOW);
		}

		private sealed class DecoratorWindowRoot : Panel
		{
		}
	}
}