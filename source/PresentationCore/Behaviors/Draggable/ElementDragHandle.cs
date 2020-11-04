// <copyright file="ElementDragHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal sealed class ElementDragHandle : IDisposable
	{
		private readonly FrameworkElement _element;
		private readonly Func<MouseEventArgs, bool> _canStartDrag;
		private readonly Action<MouseEventArgs> _onDragEnded;
		private readonly Action<MouseEventArgs> _onDragMove;
		private readonly Action<MouseEventArgs> _onDragStarted;

		public ElementDragHandle(FrameworkElement element, Func<MouseEventArgs, bool> canStartDrag, Action<MouseEventArgs> onDragStarted, Action<MouseEventArgs> onDragEnded, Action<MouseEventArgs> onDragMove)
		{
			_element = element;
			_canStartDrag = canStartDrag;
			_onDragStarted = onDragStarted;
			_onDragEnded = onDragEnded;
			_onDragMove = onDragMove;

			_element.GotMouseCapture += OnElementGotMouseCapture;
			_element.LostMouseCapture += OnElementLostMouseCapture;
			_element.MouseLeftButtonDown += OnElementMouseLeftButtonDown;
			_element.MouseLeftButtonUp += OnElementMouseLeftButtonUp;
			_element.MouseMove += OnElementMouseMove;
		}

		public bool IsDragging { get; private set; }

		private void OnElementGotMouseCapture(object sender, MouseEventArgs e)
		{
		}

		private void OnElementLostMouseCapture(object sender, MouseEventArgs e)
		{
			StopDrag(e);
		}

		private void OnElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_canStartDrag(e) == false)
				return;

			IsDragging = _element.CaptureMouse();

			if (IsDragging == false)
				return;

			_onDragStarted(e);
		}

		private void OnElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StopDrag(e);
		}

		private void OnElementMouseMove(object sender, MouseEventArgs e)
		{
			if (IsDragging)
				_onDragMove(e);
		}

		private void StopDrag(MouseEventArgs e)
		{
			if (IsDragging == false)
				return;

			_element.ReleaseMouseCapture();
			_onDragEnded(e);

			IsDragging = false;
		}

		public void Dispose()
		{
			if (IsDragging)
				_element.ReleaseMouseCapture();

			_element.GotMouseCapture -= OnElementGotMouseCapture;
			_element.LostMouseCapture -= OnElementLostMouseCapture;
			_element.MouseLeftButtonDown -= OnElementMouseLeftButtonDown;
			_element.MouseLeftButtonUp -= OnElementMouseLeftButtonUp;
			_element.MouseMove -= OnElementMouseMove;
		}
	}
}