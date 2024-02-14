// <copyright file="WindowChromeBehavior.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.Core.Weak.Collections;
using Zaaml.Platform;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Behaviors.Resizable;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PlatformInterop;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using HANDLE_MESSAGE = System.Collections.Generic.KeyValuePair<Zaaml.Platform.WM, Zaaml.Platform.MessageHandler>;

namespace Zaaml.UI.Windows
{
	internal sealed class WindowChromeBehavior : Behavior<WindowBase>
	{
		private const SWP SwpFlags = SWP.FRAMECHANGED | SWP.NOACTIVATE | SWP.NOCOPYBITS | SWP.NOMOVE | SWP.NOOWNERZORDER | SWP.NOREPOSITION | SWP.NOSENDCHANGING | SWP.NOSIZE | SWP.NOZORDER;

		private static int TaskBarMagic = 10;

		public static readonly DependencyProperty CornerRadiusProperty = DPM.Register<CornerRadius, WindowChromeBehavior>
			("CornerRadius", w => w.OnChromePropertyChangedThatRequiresRepaint);

		private static readonly DependencyProperty GlassFrameThicknessProperty = DPM.Register<Thickness, WindowChromeBehavior>
			("GlassFrameThickness", new Thickness(0), w => w.OnChromePropertyChangedThatRequiresRepaint);

		private readonly List<HANDLE_MESSAGE> _messageTable;

		private NativeBrush _backgroundWindowClassBrush;
		private int _currentDpi;
		private bool _customSizing;

		[SecurityCritical] private IntPtr _hwnd;

		[SecurityCritical] private HwndSource _hwndSource;

		private bool _isGlassEnabled;
		private bool _isHooked;
		private WindowState _lastRoundingState;

		private bool _maximized;
		private NativeBitmap _nativeRenderBitmap;
		private NativeWindow _nativeWindow;

#if DPI_EXP
		private bool _perMonitorEnabled = true;
#endif

		private double _scaleFactor;
		private double _systemDpi = DpiUtils.DpiX;
		private WindowPresenter _windowPresenterControl;
		private double _wpfDpi;

		[SecurityCritical]
		static WindowChromeBehavior()
		{
		}

		public WindowChromeBehavior()
		{
			ApplicationColors.Instance.ChromeBehaviors.Add(this);

			GlassFrameThickness = new Thickness(1);

			_messageTable = new List<HANDLE_MESSAGE>
			{
				new(WM.SETTEXT, HandleSetTextOrIcon),
				new(WM.SETICON, HandleSetTextOrIcon),
				new(WM.NCACTIVATE, HandleNCActivate),
				new(WM.NCCALCSIZE, HandleNCCalcSize),

				//new HANDLE_MESSAGE(WM.NCUAHDRAWCAPTION, HandleNCPaint),
				//new HANDLE_MESSAGE(WM.NCUAHDRAWFRAME, HandleNCPaint),
				//new HANDLE_MESSAGE(WM.NCPAINT, HandleNCPaint),

				new(WM.SYSCOMMAND, HandleSysCommand),
				new(WM.PAINT, HandlePaint),
				new(WM.ERASEBKGND, HandleEraseBKGND),
				new(WM.NCHITTEST, HandleNCHitTest),
				new(WM.SIZE, HandleSize),
				new(WM.WINDOWPOSCHANGED, HandleWindowPosChanged),
				new(WM.DWMCOMPOSITIONCHANGED, HandleDwmCompositionChanged),
				new(WM.ENTERSIZEMOVE, HandleEnterSizeMove),
				new(WM.EXITSIZEMOVE, HandleExitSizeMove),
				new(WM.MOVING, HandleMoving),
				new(WM.DPICHANGED, HandleDpiChanged),
			};
		}

		public CornerRadius CornerRadius
		{
			get => (CornerRadius) GetValue(CornerRadiusProperty);
			set => SetValue(CornerRadiusProperty, value);
		}

		public Thickness GlassFrameThickness
		{
			get => (Thickness) GetValue(GlassFrameThicknessProperty);
			set => SetValue(GlassFrameThicknessProperty, value);
		}

		public bool IsDraggable { get; set; }

		private bool IsDragging { get; set; }

		private bool IsDwmCompositionEnabled => NativeMethods.DwmIsCompositionEnabled();

		public bool IsResizable { get; set; }

		private bool IsResizing { get; set; }

		internal BitmapSource RenderBitmap
		{
			get => _nativeRenderBitmap?.Source;
			set
			{
				if (ReferenceEquals(RenderBitmap, value))
					return;

				_nativeRenderBitmap = value != null ? NativeBitmap.Create(value) : null;

				var windowRect = _nativeWindow.GetWindowRect();

				windowRect.Left = 0;
				windowRect.Top = 0;

				NativeMethods.InvalidateRect(_hwnd, ref windowRect, true);
				NativeMethods.UpdateWindow(_hwnd);
			}
		}

		public WindowPresenter WindowPresenterControl
		{
			get => _windowPresenterControl;
			set
			{
				_windowPresenterControl = value;

				FixupWindowPresenter(null);
				UpdateDpi();
			}
		}

		internal void BeginDragMove(bool async)
		{
			if (IsDraggable == false)
				return;

			if (async)
			{
				var modalCancellationToken = new ModalStateCancellationToken();

				Target.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
				{
					BeginDragMoveImpl();

					modalCancellationToken.Cancel();
				}));

				ModalState.Enter(modalCancellationToken);
			}
			else
				BeginDragMoveImpl();
		}

		private void BeginDragMoveImpl()
		{
			Target.Activate();

			IsDragging = true;

			Target.OnBeginDragMoveInternal();

			NativeMethods.SendMessage(_hwnd, (IntPtr) WM.SYSCOMMAND, (IntPtr) SC.MOUSEMOVE, IntPtr.Zero);

			Target.OnEndDragMoveInternal();

			IsDragging = false;
		}

		internal void BeginResize(ResizableHandleKind handleKind, bool async)
		{
			BeginResize(GetSizingAction(handleKind), async);
		}

		private void BeginResize(SizingAction sizingAction, bool async)
		{
			if (IsResizable == false)
				return;

			if (async)
			{
				var modalCancellationToken = new ModalStateCancellationToken();

				Target.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
				{
					BeginResizeImpl(sizingAction);

					modalCancellationToken.Cancel();
				}));

				ModalState.Enter(modalCancellationToken);
			}
			else
				BeginResizeImpl(sizingAction);
		}

		private void BeginResizeImpl(SizingAction sizingAction)
		{
			IsResizing = true;

			Target.OnBeginResizeInternal();

			NativeMethods.SendMessage(_hwnd, (IntPtr) WM.SYSCOMMAND, (IntPtr) (SC.SIZE + (int) sizingAction), IntPtr.Zero);

			Target.OnEndResizeInternal();

			IsResizing = false;
		}

		private RECT CalcNC(RECT rcClientArea)
		{
			var b = Target.AllowsTransparency == false;

			var thickness = new Thickness(0, 0, 0, IsDwmCompositionEnabled && b ? -1 : 0);
			var thicknessDevice = DpiUtils.LogicalThicknessToDevice(thickness);

			rcClientArea.Top += (int) thicknessDevice.Top;
			rcClientArea.Left += (int) thicknessDevice.Left;
			rcClientArea.Bottom -= (int) thicknessDevice.Bottom;
			rcClientArea.Right -= (int) thicknessDevice.Right;

			if (Target.WindowState == WindowState.Maximized)
			{
				int edge;
				if (TaskBarHelper.IsAutoHide(out edge, _hwndSource.Handle))
				{
					if (edge == (int) ABEdge.ABE_BOTTOM)
						rcClientArea.Bottom -= TaskBarMagic;

					if (edge == (int) ABEdge.ABE_RIGHT)
						rcClientArea.Right -= TaskBarMagic;

					if (edge == (int) ABEdge.ABE_TOP)
						rcClientArea.Top += TaskBarMagic;

					if (edge == (int) ABEdge.ABE_LEFT)
						rcClientArea.Left += TaskBarMagic;
				}
			}

			return rcClientArea;
		}

		private void ChangeRenderMode(RenderMode mode)
		{
			var hwndSource = PresentationSource.FromVisual(Target) as HwndSource;

			if (hwndSource?.CompositionTarget != null)
				hwndSource.CompositionTarget.RenderMode = mode;
		}

		[SecurityCritical]
		private void ClearRoundingRegion()
		{
			NativeMethods.SetWindowRgn(_hwnd, IntPtr.Zero, NativeMethods.IsWindowVisible(_hwnd));
		}

		private void Deinitialize()
		{
			try
			{
				Target.SourceInitialized -= WindowSourceInitialized;
				Target.Closed -= OnWindowClosed;
				Target.StateChanged -= TargetOnStateChanged;

				RestoreStandardChromeState(true);
			}
			finally
			{
				_hwnd = IntPtr.Zero;
				_hwndSource = null;
				_nativeWindow = null;
			}
		}

		[SecurityCritical]
		private void ExtendGlassFrame()
		{
			if (PlatformUtil.IsOSVistaOrNewer == false)
				return;

			if (IntPtr.Zero == _hwnd || _hwndSource?.CompositionTarget == null)
				return;

			if (IsDwmCompositionEnabled == false)
				_hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
			else
			{
				_hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;

				var actualGlassFrameThickness = GlassFrameThickness;
				var deviceGlassThickness = DpiUtils.LogicalThicknessToDevice(actualGlassFrameThickness);

				var dwmMargin = new MARGINS();

				NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref dwmMargin);

				dwmMargin = new MARGINS
				{
					cxLeftWidth = (int) Math.Ceiling(deviceGlassThickness.Left),
					cxRightWidth = (int) Math.Ceiling(deviceGlassThickness.Right),
					cyTopHeight = (int) Math.Ceiling(deviceGlassThickness.Top),
					cyBottomHeight = (int) Math.Ceiling(deviceGlassThickness.Bottom)
				};

				NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref dwmMargin);
			}
		}

		private void FillBackground()
		{
			var rect = _nativeWindow.GetWindowRect();

			rect.Left = 0;
			rect.Top = 0;

			var hdc = NativeMethods.GetWindowDC(_hwnd);
			var clientRect = _nativeWindow.GetClientRect();

			// Adjust non client area

			//clientRect.Left += 100;
			//clientRect.Top += 100;
			//clientRect.Bottom += 100 + 1;
			//clientRect.Right += 100 + 1;

			var hrgn = NativeMethods.CreateRectRgn(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom);

			NativeMethods.ExtSelectClipRgn(hdc, hrgn, CombineRgnStyles.RGN_DIFF);

			NativeMethods.FillRect(hdc, ref rect, ApplicationColors.Instance.NativeBackgroundBrush.HBrush);
			NativeMethods.ReleaseDC(_hwnd, hdc);
		}

		private void FixupWindowPresenter(WindowState? windowState)
		{
			if (WindowPresenterControl == null || _hwnd == IntPtr.Zero)
				return;

			switch (windowState ?? GetHwndState())
			{
				case WindowState.Normal:
					WindowPresenterControl.Margin = new Thickness(0);
					break;
				case WindowState.Maximized:
					var thickness = new Thickness(8);

					if (TaskBarHelper.IsAutoHide(out var edge, _hwndSource.Handle))
					{
						if (edge == (int) ABEdge.ABE_BOTTOM)
							thickness.Bottom -= TaskBarMagic;

						if (edge == (int) ABEdge.ABE_RIGHT)
							thickness.Right -= TaskBarMagic;

						if (edge == (int) ABEdge.ABE_TOP)
							thickness.Top += TaskBarMagic;

						if (edge == (int) ABEdge.ABE_LEFT)
							thickness.Left += TaskBarMagic;
					}

					WindowPresenterControl.Margin = thickness;
					break;
			}
		}

		[SecurityCritical]
		private RECT GetAdjustedWindowRect(RECT rcWindow)
		{
			var style = (WS) NativeMethods.GetWindowLongPtr(_hwnd, GWL.STYLE);
			var exstyle = (WS_EX) NativeMethods.GetWindowLongPtr(_hwnd, GWL.EXSTYLE);

			return NativeMethods.AdjustWindowRectEx(rcWindow, style, false, exstyle);
		}

		[SecurityCritical]
		private WindowState GetHwndState()
		{
			var wpl = NativeMethods.GetWindowPlacement(_hwnd);

			switch (wpl.ShowCmd)
			{
				case SW.SHOWMINIMIZED:
					return WindowState.Minimized;
				case SW.SHOWMAXIMIZED:
					return WindowState.Maximized;
			}

			return WindowState.Normal;
		}

		private static SizingAction GetSizingAction(ResizableHandleKind part)
		{
			switch (part)
			{
				case ResizableHandleKind.Undefined:
					return SizingAction.Undefined;
				case ResizableHandleKind.Left:
					return SizingAction.West;
				case ResizableHandleKind.Top:
					return SizingAction.North;
				case ResizableHandleKind.Right:
					return SizingAction.East;
				case ResizableHandleKind.Bottom:
					return SizingAction.South;
				case ResizableHandleKind.TopLeft:
					return SizingAction.NorthWest;
				case ResizableHandleKind.TopRight:
					return SizingAction.NorthEast;
				case ResizableHandleKind.BottomRight:
					return SizingAction.SouthEast;
				case ResizableHandleKind.BottomLeft:
					return SizingAction.SouthWest;
				default:
					throw new ArgumentOutOfRangeException(nameof(part));
			}
		}

		[SecurityCritical]
		private Rect GetWindowRect()
		{
			return _nativeWindow.GetWindowRect().ToPresentationRect();
		}

		private IntPtr Handle(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = true;

			return IntPtr.Zero;
		}

		private IntPtr HandleDpiChanged(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = true;

			var lprNewRect = (RECT) Marshal.PtrToStructure(lparam, typeof(RECT));

			NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, lprNewRect.Left, lprNewRect.Top, lprNewRect.Width, lprNewRect.Height,
				SWP.NOZORDER | SWP.NOOWNERZORDER | SWP.NOACTIVATE);

			var oldDpi = _currentDpi;

			_currentDpi = wparam.ToInt32() & 0xFFFF;

			if (oldDpi != _currentDpi)
				OnDpiChanged();

			return IntPtr.Zero;
		}

		[SecurityCritical]
		private IntPtr HandleDwmCompositionChanged(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			UpdateCaptionStyle();
			UpdateFrameState(false);

			handled = false;

			return IntPtr.Zero;
		}

		[SecurityCritical]
		private IntPtr HandleEnterSizeMove(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			OnSizeMoveStarted();

			handled = false;

			return IntPtr.Zero;
		}

		private IntPtr HandleEraseBKGND(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = true;

			if (Target.IsContentRendered == false)
			{
				var hdc = wparam;
				var windowRect = _nativeWindow.GetWindowRect();

				windowRect.Left = 0;
				windowRect.Top = 0;

				NativeMethods.FillRect(hdc, ref windowRect, ApplicationColors.Instance.NativeBackgroundBrush.HBrush);
			}

			return new IntPtr(1);
		}

		[SecurityCritical]
		private IntPtr HandleExitSizeMove(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			OnSizeMoveFinished();

			handled = false;

			return IntPtr.Zero;
		}

		private IntPtr HandleMoving(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = true;

			Target.OnDragMoveInternal();

			return IntPtr.Zero;
		}

		private IntPtr HandleNCActivate(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = true;

			var lRet = NativeMethods.DefWindowProc(_hwnd, WM.NCACTIVATE, wparam, new IntPtr(-1));

			if (IsDwmCompositionEnabled)
			{
				var rect = _nativeWindow.GetWindowRect();

				rect.Top = 0;
				rect.Left = 0;

				NativeMethods.InvalidateRect(_hwnd, ref rect, false);
			}

			return lRet;
		}

		[SecurityCritical]
		private IntPtr HandleNCCalcSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			if (wParam == new IntPtr(1))
			{
				var rcClientArea = (NCCALCSIZE_PARAMS) Marshal.PtrToStructure(lParam, typeof(NCCALCSIZE_PARAMS));
				var nc = CalcNC(rcClientArea.rgrc[0]);

				rcClientArea.rgrc[0] = nc;

				Marshal.StructureToPtr(rcClientArea, lParam, false);
			}
			else if (wParam == IntPtr.Zero)
			{
				var rcClientArea = (RECT) Marshal.PtrToStructure(lParam, typeof(RECT));

				rcClientArea = CalcNC(rcClientArea);

				Marshal.StructureToPtr(rcClientArea, lParam, false);
			}

			handled = true;

			return new IntPtr((int) (WVR.VALIDRECTS | WVR.REDRAW));
		}

		private IntPtr HandleNCHitTest(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = true;

			var mousePosScreen = new POINT
			{
				x = PlatformUtil.GET_X_LPARAM(lparam),
				y = PlatformUtil.GET_Y_LPARAM(lparam)
			};

			var windowPosition = _nativeWindow.GetWindowRect();
			var mousePosWindow = mousePosScreen;

			mousePosWindow.x -= windowPosition.Left;
			mousePosWindow.y -= windowPosition.Top;

			var hitTestResizeBorder = IsResizable ? HitTestResizeBorder(mousePosWindow, windowPosition.Height, windowPosition.Width, 4, 1) : HT.NOWHERE;

			if (hitTestResizeBorder != HT.NOWHERE)
				return (IntPtr) hitTestResizeBorder;

			return (IntPtr) HT.CLIENT;
		}

		private IntPtr HandleNCPaint(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = true;

			PaintNCArea(wparam);

			return new IntPtr(1);
		}

		private IntPtr HandlePaint(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			if (_nativeRenderBitmap == null)
			{
				handled = false;

				return IntPtr.Zero;
			}

			var hdc = NativeMethods.BeginPaint(_hwnd, out var paintStruct);

			if (hdc == IntPtr.Zero)
			{
				handled = false;

				return IntPtr.Zero;
			}

			handled = true;

			var windowRect = _nativeWindow.GetWindowRect();

			windowRect.Left = 0;
			windowRect.Top = 0;

			var width = _nativeRenderBitmap.Source.PixelWidth;
			var height = _nativeRenderBitmap.Source.PixelHeight;

			var hdcMem = NativeMethods.CreateCompatibleDC(hdc);
			var hbmOld = NativeMethods.SelectObject(hdcMem, _nativeRenderBitmap.Handle);

			NativeMethods.BitBlt(hdc, 0, 0, width, height, hdcMem, 0, 0, TernaryRasterOperations.SRCCOPY);
			NativeMethods.SelectObject(hdcMem, hbmOld);
			NativeMethods.DeleteDC(hdcMem);

			NativeMethods.EndPaint(_hwnd, ref paintStruct);

			return IntPtr.Zero;
		}

		private IntPtr HandleSetTextOrIcon(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			var modified = PlatformUtil.ModifyStyle(_hwnd, WS.VISIBLE, 0);

			var lRet = NativeMethods.DefWindowProc(_hwnd, umsg, wparam, lparam);

			if (modified)
				PlatformUtil.ModifyStyle(_hwnd, 0, WS.VISIBLE);

			handled = true;

			return lRet;
		}

		[SecurityCritical]
		private IntPtr HandleSize(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			const int SIZE_MAXIMIZED = 2;
			const int SIZE_RESTORED = 0;

			var maximized = _maximized;

			switch (wParam.ToInt32())
			{
				case SIZE_MAXIMIZED:

					_maximized = true;

					FixupWindowPresenter(WindowState.Maximized);

					break;
				case SIZE_RESTORED:

					if (_maximized)
						FixupWindowPresenter(WindowState.Normal);

					_maximized = false;

					break;
			}

			if (maximized != _maximized)
				UpdateFrameState(true);

			handled = false;

			return IntPtr.Zero;
		}

		private IntPtr HandleSysCommand(WM umsg, IntPtr wparam, IntPtr lparam, out bool handled)
		{
			handled = false;

			var sysCommand = wparam.ToInt32();

			if (sysCommand >= (int) SC.SIZE + (int) SizingAction.MinAction && sysCommand <= (int) SC.SIZE + (int) SizingAction.MaxAction)
			{
				if (_customSizing)
					return IntPtr.Zero;

				try
				{
					_customSizing = true;

					handled = true;

					BeginResize((SizingAction) (sysCommand - SC.SIZE), false);
				}
				finally
				{
					_customSizing = false;
				}
			}

			return IntPtr.Zero;
		}

		[SecurityCritical]
		private IntPtr HandleWindowPosChanged(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
		{
			var wp = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

			if (CommonUtils.IsFlagSet(wp.flags, (int) SWP.NOSIZE) == false)
			{
				if (_isGlassEnabled == false)
					SetRoundingRegion(wp);
			}

			handled = false;

			return IntPtr.Zero;
		}

		private static HT HitTestResizeBorder(POINT position, int height, int width, int thickness, int dc)
		{
			var resPart = ResizableHandleKind.Undefined;

			resPart |= position.y >= 0 && position.y <= dc * thickness ? ResizableHandleKind.Top : 0;
			resPart |= position.y >= height - dc * thickness && position.y <= height ? ResizableHandleKind.Bottom : 0;
			resPart |= position.x >= 0 && position.x <= dc * thickness ? ResizableHandleKind.Left : 0;
			resPart |= position.x >= width - dc * thickness && position.x <= width ? ResizableHandleKind.Right : 0;

			switch (resPart)
			{
				case ResizableHandleKind.Left:
					return HT.LEFT;
				case ResizableHandleKind.Top:
					return HT.TOP;
				case ResizableHandleKind.Right:
					return HT.RIGHT;
				case ResizableHandleKind.Bottom:
					return HT.BOTTOM;
				case ResizableHandleKind.TopLeft:
					return HT.TOPLEFT;
				case ResizableHandleKind.TopRight:
					return HT.TOPRIGHT;
				case ResizableHandleKind.BottomRight:
					return HT.BOTTOMRIGHT;
				case ResizableHandleKind.BottomLeft:
					return HT.BOTTOMLEFT;
			}

			return HT.NOWHERE;
		}

		private void Initialize()
		{
			var wih = new WindowInteropHelper(Target);

			Target.StateChanged += TargetOnStateChanged;
			Target.Closed += OnWindowClosed;

			if (IntPtr.Zero != wih.Handle)
				InitializeStep2(wih);
			else
				Target.SourceInitialized += WindowSourceInitialized;
		}

		private void InitializeStep2(WindowInteropHelper wih)
		{
			if (_hwnd != IntPtr.Zero)
				return;

			if (wih.Handle == IntPtr.Zero)
				return;

			_hwndSource = HwndSource.FromHwnd(wih.Handle);

			if (_hwndSource == null || _hwndSource.IsDisposed)
				return;

			_hwnd = wih.Handle;

#if DPI_EXP
			_perMonitorEnabled = DpiUtils.SetPerMonitorDPIAware();
#endif
			//var hrgn = NativeMethods.CreateRectRgn(0, 0, 0, 0);

			//NativeMethods.SetWindowRgn(_hwnd, hrgn, true);

			_nativeWindow = new NativeWindow(_hwnd);

			if (_isHooked == false)
			{
				_hwndSource.AddHook(WndProc);
				_isHooked = true;
			}

			UpdateDpi();
			UpdateBackgroundColor();
			UpdateBorderColor();
			UpdateCaptionStyle();

			Target.ApplyTemplate();
			FixupWindowPresenter(null);
			UpdateFrameState(true);

			NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpFlags);

			Target.SourceInitialized -= WindowSourceInitialized;
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			Initialize();
		}

		[SecuritySafeCritical]
		private void OnChromePropertyChangedThatRequiresRepaint()
		{
			UpdateFrameState(true);
		}

		protected override void OnDetaching()
		{
			Deinitialize();

			base.OnDetaching();
		}

		private void OnDpiChanged()
		{
			_scaleFactor = _currentDpi / _wpfDpi;

			UpdateDpiLayoutTransform(_scaleFactor);
		}

		public void OnMeasure()
		{
			if (_hwnd != IntPtr.Zero)
				return;

			// TODO: Why Target is null here?
			if (Target == null)
				return;

			InitializeStep2(new WindowInteropHelper(Target));
		}

		private void OnSizeMoveFinished()
		{
			//ChangeRenderMode(RenderMode.Default);
		}

		private void OnSizeMoveStarted()
		{
			//ChangeRenderMode(RenderMode.SoftwareOnly);
		}

		private void OnWindowClosed(object sender, EventArgs e)
		{
		}

		private void PaintNCArea(IntPtr hRgn)
		{
			var windowRect = _nativeWindow.GetWindowRect();

			if (windowRect.Width == 0 || windowRect.Height == 0)
				return;

			windowRect = new RECT {Left = 0, Top = 0, Right = windowRect.Width, Bottom = windowRect.Bottom};

			var flags = DeviceContextValues.Window | DeviceContextValues.IntersectRgn | DeviceContextValues.Cache | DeviceContextValues.ClipSiblings;
			var hDC = NativeMethods.GetDCEx(_hwnd, hRgn == new IntPtr(1) ? IntPtr.Zero : hRgn, hRgn == new IntPtr(1) ? DeviceContextValues.Window : flags);

			if (hDC == IntPtr.Zero)
				return;

			try
			{
			}
			finally
			{
				NativeMethods.ReleaseDC(_hwnd, hDC);
			}
		}

		private void RedrawFrame()
		{
			NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpFlags);
		}

		[SecurityCritical]
		private void RestoreGlassFrame()
		{
			if (PlatformUtil.IsOSVistaOrNewer == false || _hwnd == IntPtr.Zero || null == _hwndSource.CompositionTarget)
				return;

			_hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;

			if (NativeMethods.DwmIsCompositionEnabled() == false)
				return;

			var dwmMargin = new MARGINS();
			NativeMethods.DwmExtendFrameIntoClientArea(_hwnd, ref dwmMargin);
		}

		[SecurityCritical]
		private void RestoreHrgn()
		{
			ClearRoundingRegion();
			NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpFlags);
		}

		[SecurityCritical]
		private void RestoreStandardChromeState(bool isClosing)
		{
			VerifyAccess();

			UnhookCustomChrome();

			if (!isClosing && !_hwndSource.IsDisposed)
			{
				RestoreGlassFrame();
				RestoreHrgn();

				Target.InvalidateMeasure();
			}
		}

		[SecurityCritical]
		private void SetRoundingRegion(WINDOWPOS? wp)
		{
			var wpl = NativeMethods.GetWindowPlacement(_hwnd);
			var hrgn = IntPtr.Zero;

			try
			{
				if (wpl.ShowCmd == SW.SHOWMAXIMIZED)
				{
					var location = (wp?.GetRect() ?? _nativeWindow.GetWindowRect()).ToPresentationRect().GetTopLeft();
					var hMon = NativeMethods.MonitorFromWindow(_hwnd, MonitorOptions.MONITOR_DEFAULTTONEAREST);
					var mi = NativeMethods.GetMonitorInfo(hMon);
					var rcMax = mi.rcWork.ToPresentationRect().WithOffset(location.Negate());

					hrgn = NativeMethods.CreateRectRgnIndirect(rcMax.ToPlatformRect());
					NativeMethods.SetWindowRgn(_hwnd, hrgn, NativeMethods.IsWindowVisible(_hwnd));
					hrgn = IntPtr.Zero;
				}
				else
				{
					Size windowSize;

					if (null != wp && CommonUtils.IsFlagSet(wp.Value.flags, (int) SWP.NOSIZE) == false)
						windowSize = new Size(wp.Value.cx, wp.Value.cy);
					else if (null != wp && _lastRoundingState == Target.WindowState)
						return;
					else
						windowSize = GetWindowRect().Size;

					_lastRoundingState = Target.WindowState;

					var windowRect = new Rect(windowSize);

					hrgn = RegionHelper.CreateRoundedCornerRegion(windowRect, DpiUtils.LogicalCornerRadiusToDevice(CornerRadius));
					NativeMethods.SetWindowRgn(_hwnd, hrgn, NativeMethods.IsWindowVisible(_hwnd));
					hrgn = IntPtr.Zero;
				}
			}
			finally
			{
				PlatformUtil.SafeDeleteObject(ref hrgn);
			}
		}

		private void TargetOnStateChanged(object sender, EventArgs eventArgs)
		{
			RedrawFrame();
		}

		[SecurityCritical]
		private void UnhookCustomChrome()
		{
			if (_isHooked == false) return;

			_hwndSource.RemoveHook(WndProc);
			_isHooked = false;
		}

		private void UpdateBackgroundColor()
		{
			if (_isHooked)
			{
				var backgroundWindowClassBrush = ApplicationColors.Instance.BackgroundColor.ToNativeBrush();

				// Do not use ApplicationColors.Instance.NativeBackgroundBrush.HBrush !!! It will be disposed when window using brush handle get closed.
				NativeMethods.SetClassLong(_hwnd, ClassLongFlags.GCLP_HBRBACKGROUND, backgroundWindowClassBrush.HBrush);

				_backgroundWindowClassBrush = _backgroundWindowClassBrush.DisposeExchange(backgroundWindowClassBrush);
			}
		}

		private void UpdateBorderColor()
		{
		}

		private void UpdateCaptionStyle()
		{
			PlatformUtil.ModifyStyle(_hwnd, 0, WS.CAPTION);
		}

		private void UpdateDpi()
		{
			if (_hwndSource == null)
				return;

			_wpfDpi = 96.0 * _hwndSource.CompositionTarget.TransformToDevice.M11;
			_currentDpi = DpiUtils.GetMonitorDpi(_hwnd).DpiX;
			_scaleFactor = _currentDpi / _wpfDpi;

			UpdateDpiLayoutTransform(_scaleFactor);
		}

		private void UpdateDpiLayoutTransform(double scaleFactor)
		{
#if DPI_EXP
			if (_perMonitorEnabled == false)
				return;
			
			if (WindowPresenterControl == null)
				return;
			
			if (_scaleFactor != 1.0)
				WindowPresenterControl.SetValue(FrameworkElement.LayoutTransformProperty, new ScaleTransform(scaleFactor, scaleFactor));
			else
				WindowPresenterControl.SetValue(FrameworkElement.LayoutTransformProperty, null);
#endif
		}

		internal void UpdateFrameState(bool force)
		{
			if (IntPtr.Zero == _hwnd || _hwndSource.IsDisposed)
				return;

			var frameState = IsDwmCompositionEnabled;

			if (force == false && frameState == _isGlassEnabled)
				return;

			var isGlassEnabled = _isGlassEnabled;

			_isGlassEnabled = frameState;

			ClearRoundingRegion();

			if (isGlassEnabled != _isGlassEnabled)
			{
				if (_isGlassEnabled == false)
					SetRoundingRegion(null);
				else
					ExtendGlassFrame();
			}

			RedrawFrame();
		}

		[SecuritySafeCritical]
#if !NET5_0
    //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
#endif
		private void WindowSourceInitialized(object sender, EventArgs e)
		{
			InitializeStep2(new WindowInteropHelper(Target));
		}

		[SecurityCritical]
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			var message = (WM) msg;

			foreach (var handlePair in _messageTable.Where(handlePair => handlePair.Key == message))
				return handlePair.Value(message, wParam, lParam, out handled);

			return IntPtr.Zero;
		}

		private static class TaskBarHelper
		{
			private const int ABS_AUTOHIDE = 1;

			[SecurityCritical]
			public static bool IsAutoHide(out int edge, IntPtr handle)
			{
				edge = 0;
				var data = new APPBARDATA();

				data.cbSize = Marshal.SizeOf(data);
				data.hWnd = handle;

				var res = NativeMethods.SHAppBarMessage((int) ABMsg.ABM_GETSTATE, ref data);

				if ((res & ABS_AUTOHIDE) == 0)
					return false;

				for (var n = 0; n < 4; n++)
				{
					edge = n;
					data.uEdge = (uint) n;

					if (NativeMethods.SHAppBarMessage((int) ABMsg.ABM_GETAUTOHIDEBAR, ref data) != 0)
						return true;
				}

				edge = (int) ABEdge.ABE_BOTTOM;
				return true;
			}
		}

		private class ApplicationColors : DependencyObject
		{
			private static readonly Lazy<ApplicationColors> LazyInstance = new Lazy<ApplicationColors>(() => new ApplicationColors());

			private static readonly DependencyProperty BackgroundColorProperty = DPM.Register<Color, ApplicationColors>
				("BackgroundColor", a => a.OnBackgroundColorChanged);

			public static readonly DependencyProperty BorderColorProperty = DPM.Register<Color, ApplicationColors>
				("BorderColor", a => a.OnBorderColorChanged);

			private ApplicationColors()
			{
				ThemeManager.AssignThemeResource(this, BackgroundColorProperty, ThemeKeyword.ApplicationBackgroundColor);
				ThemeManager.AssignThemeResource(this, BorderColorProperty, ThemeKeyword.ApplicationBorderColor);
			}

			public Color BackgroundColor => (Color) GetValue(BackgroundColorProperty);

			public Color BorderColor => (Color) GetValue(BorderColorProperty);

			public WeakLinkedList<WindowChromeBehavior> ChromeBehaviors { get; } = new WeakLinkedList<WindowChromeBehavior>();

			public static ApplicationColors Instance => LazyInstance.Value;

			public NativeBrush NativeBackgroundBrush { get; private set; } = Colors.White.ToNativeBrush();

			public NativeBrush NativeBorderBrush { get; private set; } = Colors.White.ToNativeBrush();

			private void OnBackgroundColorChanged()
			{
				NativeBackgroundBrush = NativeBackgroundBrush.DisposeExchange(BackgroundColor.ToNativeBrush());

				foreach (var chromeBehavior in ChromeBehaviors)
					chromeBehavior.UpdateBackgroundColor();
			}

			private void OnBorderColorChanged()
			{
				NativeBorderBrush = NativeBorderBrush.DisposeExchange(BorderColor.ToNativeBrush());

				foreach (var chromeBehavior in ChromeBehaviors)
					chromeBehavior.UpdateBorderColor();
			}
		}

		private enum SizingAction
		{
			Undefined = -1,
			MinAction = 1,
			North = 3,
			South = 6,
			East = 2,
			West = 1,
			NorthEast = 5,
			NorthWest = 4,
			SouthEast = 8,
			SouthWest = 7,
			MaxAction = 8
		}
	}
}