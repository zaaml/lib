// <copyright file="MouseService.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.Platform;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Input
{
	internal sealed class MouseService : IMouseService
	{
		private static readonly Lazy<MouseService> LazyInstance = new Lazy<MouseService>(() => new MouseService());

		private WeakReference _lastElementWeak;
		private WeakReference _lastEventArgsWeak;

		private DelegateDisposable _mouseCaptureDisposer;

		private MouseService()
		{
			RuntimeHelpers.RunClassConstructor(typeof(MouseInternal).TypeHandle);

			EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnGlobalMouseDown), true);
			EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseUpEvent, new MouseButtonEventHandler(OnGlobalMouseUp), true);

			EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnGlobalPreviewMouseDown), true);
			EventManager.RegisterClassHandler(typeof(UIElement), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(OnGlobalPreviewMouseUp), true);

			EventManager.RegisterClassHandler(typeof(UIElement), Mouse.MouseMoveEvent, new MouseEventHandler(OnGlobalMouseMove), true);
		}

		public static MouseService Instance => LazyInstance.Value;

		private MouseEventArgs LastEventArgs
		{
			get => _lastEventArgsWeak.GetTarget<MouseEventArgs>();
			set
			{
				if (ReferenceEquals(LastEventArgs, value))
					return;

				_lastEventArgsWeak = value != null ? new WeakReference(value) : null;
			}
		}

		private void MouseCaptureElementOnLostMouseCapture(object sender, MouseEventArgs e)
		{
			_mouseCaptureDisposer = _mouseCaptureDisposer.DisposeExchange();
			MouseCaptureElement = null;
		}

		private void OnGlobalMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (ReferenceEquals(e, LastEventArgs))
				return;

			LastEventArgs = e;

			switch (e.ChangedButton)
			{
				case MouseButton.Left:

					OnMouseLeftButtonDown(e);

					break;

				case MouseButton.Middle:
					break;

				case MouseButton.Right:

					OnMouseRightButtonDown(e);

					break;

				case MouseButton.XButton1:
					break;

				case MouseButton.XButton2:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnGlobalMouseMove(object sender, MouseEventArgs e)
		{
			if (ReferenceEquals(e, LastEventArgs))
				return;

			LastEventArgs = e;

			var visualSource = e.Source as UIElement;

			if (visualSource == null || PresentationSource.FromVisual(visualSource) == null)
				return;

			LastElement = visualSource;

			OnMouseMove(e);
		}

		private void OnGlobalMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (ReferenceEquals(e, LastEventArgs))
				return;

			LastEventArgs = e;

			switch (e.ChangedButton)
			{
				case MouseButton.Left:

					OnMouseLeftButtonUp(e);

					break;

				case MouseButton.Middle:
					break;

				case MouseButton.Right:

					OnMouseRightButtonUp(e);

					break;

				case MouseButton.XButton1:
					break;

				case MouseButton.XButton2:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnGlobalPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (ReferenceEquals(e, LastEventArgs))
				return;

			LastEventArgs = e;

			switch (e.ChangedButton)
			{
				case MouseButton.Left:

					OnPreviewMouseLeftButtonDown(e);

					break;

				case MouseButton.Middle:
					break;

				case MouseButton.Right:

					OnPreviewMouseRightButtonDown(e);

					break;

				case MouseButton.XButton1:
					break;

				case MouseButton.XButton2:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnGlobalPreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (ReferenceEquals(e, LastEventArgs))
				return;

			LastEventArgs = e;

			switch (e.ChangedButton)
			{
				case MouseButton.Left:

					OnPreviewMouseLeftButtonUp(e);

					break;

				case MouseButton.Middle:
					break;

				case MouseButton.Right:

					OnPreviewMouseRightButtonUp(e);

					break;

				case MouseButton.XButton1:
					break;

				case MouseButton.XButton2:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			MouseLeftButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		private void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			MouseLeftButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		private void OnMouseMove(MouseEventArgs e)
		{
			MouseMove?.Invoke(this, e.ToMouseEventArgsInt());
		}

		private void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			MouseRightButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		private void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			MouseRightButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		private void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			PreviewMouseLeftButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		private void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			PreviewMouseLeftButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		private void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
		{
			PreviewMouseRightButtonDown?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		private void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
		{
			PreviewMouseRightButtonUp?.Invoke(this, e.ToMouseButtonEventArgsInt());
		}

		public bool CaptureMouse(UIElement element)
		{
			MouseCaptureElement = element.CaptureMouse() ? element : null;

			if (MouseCaptureElement != null)
			{
				MouseCaptureElement.LostMouseCapture += MouseCaptureElementOnLostMouseCapture;
				//MouseCaptureElement.AddHandler(Mouse.MouseDownEvent, (MouseButtonEventHandler)OnGlobaMouseDown, true);
				//MouseCaptureElement.AddHandler(Mouse.MouseUpEvent, (MouseButtonEventHandler)OnGlobaMouseUp, true);
				//MouseCaptureElement.AddHandler(Mouse.MouseMoveEvent, (MouseEventHandler)OnGlobaMouseMove, true);

				_mouseCaptureDisposer = new DelegateDisposable
				(delegate
				{
					MouseCaptureElement.LostMouseCapture -= MouseCaptureElementOnLostMouseCapture;
					//MouseCaptureElement.RemoveHandler(Mouse.MouseDownEvent, (MouseButtonEventHandler)OnGlobaMouseDown);
					//MouseCaptureElement.RemoveHandler(Mouse.MouseUpEvent, (MouseButtonEventHandler)OnGlobaMouseUp);
					//MouseCaptureElement.RemoveHandler(Mouse.MouseMoveEvent, (MouseEventHandler)OnGlobaMouseMove);
				});
			}

			return IsMouseCaptured;
		}

		public Point GetPosition(UIElement relativeTo)
		{
			return Mouse.GetPosition(relativeTo);
		}

		public UIElement DirectlyOver => Mouse.DirectlyOver as UIElement;

		/// <summary>
		/// Get/Set Mouse cursor position in Device Screen coordinates
		/// </summary>
		public Point ScreenPosition
		{
			get => NativeMethodsSafe.GetCursorPos().ToPresentationPoint();
			set => NativeMethodsSafe.SetCursorPos(value.ToPlatformPoint());
		}

		public bool IsMouseCaptured => MouseCaptureElement != null || Mouse.Captured != null;

		public UIElement LastElement
		{
			get => _lastElementWeak.GetTarget<UIElement>();
			private set
			{
				if (ReferenceEquals(LastElement, value))
					return;

				_lastElementWeak = value != null ? new WeakReference(value) : null;
			}
		}

		public MouseButtonState LeftButtonState => Mouse.LeftButton == MouseButtonState.Pressed ? MouseButtonState.Pressed : MouseButtonState.Released;

		public UIElement MouseCaptureElement { get; private set; }

		public event MouseButtonEventHandlerInt MouseLeftButtonDown;
		public event MouseButtonEventHandlerInt MouseLeftButtonUp;
		public event MouseButtonEventHandlerInt MouseRightButtonDown;
		public event MouseButtonEventHandlerInt MouseRightButtonUp;

		public event MouseButtonEventHandlerInt PreviewMouseLeftButtonDown;
		public event MouseButtonEventHandlerInt PreviewMouseLeftButtonUp;
		public event MouseButtonEventHandlerInt PreviewMouseRightButtonDown;
		public event MouseButtonEventHandlerInt PreviewMouseRightButtonUp;

		public event MouseEventHandlerInt MouseMove;

		public void ReleaseMouseCapture()
		{
			MouseCaptureElement.Do(m => m.ReleaseMouseCapture());
			MouseCaptureElement = null;
		}

		public MouseButtonState RightButtonState => Mouse.RightButton == MouseButtonState.Pressed ? MouseButtonState.Pressed : MouseButtonState.Released;
	}
}