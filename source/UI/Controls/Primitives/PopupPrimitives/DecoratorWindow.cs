// <copyright file="DecoratorWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Zaaml.Core.Runtime;
using Zaaml.Platform;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
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

		public static readonly DependencyProperty TitleProperty = DPM.Register<string, DecoratorWindow>
			("Title", default, d => d.OnTitlePropertyChangedPrivate);

		private readonly HwndDpiChangedEventHandler _dpiChangedHandler;
		private readonly HwndSourceHook _messageFilterHook;

		private readonly AutoResizedEventHandler _onAutoResizeHandler;
		private HwndSource _hwndSource;

		public event EventHandler HwndSourceChanged;

		public DecoratorWindow()
		{
			DecoratorRoot = new DecoratorWindowRoot { Background = null };

			_onAutoResizeHandler = OnAutoResize;
			_dpiChangedHandler = OnDpiChanged;
			_messageFilterHook = MessageFilter;
		}

		public FrameworkElement Child
		{
			get => (FrameworkElement)GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		private DecoratorWindowRoot DecoratorRoot { get; }

		public double Height
		{
			get => (double)GetValue(HeightProperty);
			set => SetValue(HeightProperty, value);
		}

		internal HwndSource HwndSource
		{
			get => _hwndSource;
			private set
			{
				if (_hwndSource == value)
					return;

				_hwndSource = value;

				HwndSourceChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value.Box());
		}

		public double Left
		{
			get => (double)GetValue(LeftProperty);
			set => SetValue(LeftProperty, value);
		}

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		public double Top
		{
			get => (double)GetValue(TopProperty);
			set => SetValue(TopProperty, value);
		}

		public double Width
		{
			get => (double)GetValue(WidthProperty);
			set => SetValue(WidthProperty, value);
		}

		private HwndSource BuildWindow()
		{
			var ws = WS.CLIPSIBLINGS;
			var wsEx = WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE | WS_EX.TRANSPARENT;

			var param = new HwndSourceParameters(string.Empty)
			{
				WindowClassStyle = 0,
				WindowStyle = unchecked((int)ws),
				ExtendedWindowStyle = (int)wsEx,
				UsesPerPixelOpacity = true
			};

			param.SetPosition((int)Left, (int)Top);
			param.SetSize((int)Width, (int)Height);

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

			if (DpiUtils.IsPerMonitorDpiScalingActive)
				DestroyWindow();

			HwndSource ??= BuildWindow();

			UpdateWindowTitle();
		}

		private object HandleDeactivateApp(object arg)
		{
			this.SetCurrentValueInternal(IsOpenProperty, BooleanBoxes.False);

			return null;
		}

		private IntPtr MessageFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			var wm = (WM)msg;

			if (wm == WM.MOUSEACTIVATE)
			{
				handled = true;

				// MA_NOACTIVATE
				return new IntPtr(3);
			}

			if (wm == WM.WINDOWPOSCHANGING)
			{
				//var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
			}

			if (wm == WM.WINDOWPOSCHANGED)
			{
				//var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
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
			if (HwndSource != null)
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

		private void OnTitlePropertyChangedPrivate(string oldValue, string newValue)
		{
			UpdateWindowTitle();
		}

		private void OnTopmostPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (HwndSource != null)
				UpdatePosition();
		}

		private void OnTopPropertyChangedPrivate(double oldValue, double newValue)
		{
			if (HwndSource != null)
				UpdatePosition();
		}

		private void OnWidthPropertyChangedPrivate(double oldValue, double newValue)
		{
			if (HwndSource != null)
				UpdateSize();
		}

		public void SetZOrder(IntPtr hwnd)
		{
			if (HwndSource == null)
				return;

			if (IsOpen)
				NativeMethods.SetWindowPos(HwndSource.Handle, hwnd, 0, 0, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOMOVE | SWP.SHOWWINDOW);
			else
				NativeMethods.SetWindowPos(HwndSource.Handle, hwnd, 0, 0, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOMOVE | SWP.HIDEWINDOW);
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

			NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, (int)Left, (int)Top, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOOWNERZORDER | SWP.NOZORDER | SWP.NOREDRAW);
		}

		private void UpdateSize()
		{
			if (HwndSource == null)
				return;

			NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, 0, 0, (int)Width, (int)Height, SWP.NOACTIVATE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOZORDER);
		}

		private void UpdateVisibility()
		{
			if (HwndSource == null)
				return;

			if (IsOpen)
				NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOZORDER | SWP.SHOWWINDOW);
			else
				NativeMethods.SetWindowPos(HwndSource.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP.NOACTIVATE | SWP.NOSIZE | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOZORDER | SWP.HIDEWINDOW);
		}

		private void UpdateWindowTitle()
		{
			if (HwndSource != null)
				NativeMethods.SetWindowText(HwndSource.Handle, Title);
		}

		private sealed class DecoratorWindowRoot : Panel
		{
		}
	}
}