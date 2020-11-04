// <copyright file="MouseCaptureListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Platform;
#if SILVERLIGHT
#else
using System.Windows.Input;
using Zaaml.PresentationCore.Utils;
#endif

namespace Zaaml.PresentationCore.Input
{
	internal class MouseCaptureListener : IDisposable, IMouseEventListener
  {
		private readonly UIElement _captureElement;

#if SILVERLIGHT

		public MouseCaptureListener(UIElement captureElement)
		{
			_captureElement = captureElement;
			_captureElement.CaptureMouse();
		}

		public void Dispose()
		{
			_captureElement.ReleaseMouseCapture();
		}

    void IMouseEventListener.OnMouseEvent(MouseEventInfo eventInfo)
    {
    }
#else

		private bool _disposed;
		private readonly CaptureMode _captureMode;
		private IInputElement _currentCaptureElement;
		
		public MouseCaptureListener(UIElement captureElement, CaptureMode captureMode)
		{
			_captureElement = captureElement;
			_captureMode = captureMode;

      HwndMouseObserver.AddListener(this);

      CurrentCaptureElement = _captureElement;
		}
		
		void IMouseEventListener.OnMouseEvent(MouseEventInfo eventInfo)
		{
			CheckClientAreaUpdateCapture();
		}

		private void CheckClientAreaUpdateCapture()
		{
			var point = new POINT();

			NativeMethods.GetCursorPos(ref point);

			var handle = NativeMethods.WindowFromPoint(point);

			if (HwndSourceUtils.HitTest(handle) == WindowHitTestResult.ClientArea)
				UpdateCapture();
			else
				RemoveCapture();
		}

		private void UpdateCapture()
		{
			CurrentCaptureElement = _disposed ? null : Mouse.Captured ?? _captureElement;
		}

		private void RemoveCapture()
		{
			if (ReferenceEquals(CurrentCaptureElement, _captureElement))
				CurrentCaptureElement = null;
		}

		private void CurrentCaptureElementOnLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
		{
			UpdateCapture();
		}

		private IInputElement CurrentCaptureElement
		{
			get => _currentCaptureElement;
			set
			{
				if (ReferenceEquals(_currentCaptureElement, value))
					return;

				if (ReferenceEquals(_currentCaptureElement, _captureElement) && value == null)
				{
					_currentCaptureElement.LostMouseCapture -= CurrentCaptureElementOnLostMouseCapture;
					_captureElement.ReleaseMouseCapture();
				}
				else if (_currentCaptureElement != null)
					_currentCaptureElement.LostMouseCapture -= CurrentCaptureElementOnLostMouseCapture;

				_currentCaptureElement = value;

				if (_currentCaptureElement != null)
					_currentCaptureElement.LostMouseCapture += CurrentCaptureElementOnLostMouseCapture;

				if (_disposed == false && ReferenceEquals(_currentCaptureElement, _captureElement))
					Mouse.Capture(_captureElement, _captureMode);
			}
		}

		public void Dispose()
		{
			_disposed = true;
			CurrentCaptureElement = null;

      HwndMouseObserver.RemoveListener(this);
    }
#endif
  }
}