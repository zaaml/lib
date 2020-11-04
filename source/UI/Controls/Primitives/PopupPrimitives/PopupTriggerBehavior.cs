// <copyright file="PopupTriggerBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public class PopupTriggerBehavior : Behavior<FrameworkElement>
	{
		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, PopupTriggerBehavior>
			("IsOpen");

		public static readonly DependencyProperty ClickModeProperty = DPM.Register<ClickMode, PopupTriggerBehavior>
			("ClickMode", ClickMode.Press);

		private bool _isMouseCaptured;
		private bool _pressedState;

		public ClickMode ClickMode
		{
			get => (ClickMode) GetValue(ClickModeProperty);
			set => SetValue(ClickModeProperty, value);
		}

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		private bool CheckMouseEventSource(MouseButtonEventArgs e)
		{
			return MouseInternal.IsMouseButtonClick(FrameworkElement, PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource));
		}

		private void FrameworkElementOnLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
		{
			_isMouseCaptured = false;
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			FrameworkElement.AddHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnTargetMouseDown, false);
			FrameworkElement.AddHandler(UIElement.MouseLeftButtonUpEvent, (MouseButtonEventHandler) OnTargetMouseUp, false);

			FrameworkElement.MouseEnter += OnTargetMouseEnter;
			FrameworkElement.MouseLeave += OnTargetMouseLeave;

			FrameworkElement.LostMouseCapture += FrameworkElementOnLostMouseCapture;
		}

		protected override void OnDetaching()
		{
			FrameworkElement.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnTargetMouseDown);
			FrameworkElement.RemoveHandler(UIElement.MouseLeftButtonUpEvent, (MouseButtonEventHandler) OnTargetMouseUp);

			FrameworkElement.MouseEnter -= OnTargetMouseEnter;
			FrameworkElement.MouseLeave -= OnTargetMouseLeave;

			FrameworkElement.LostMouseCapture -= FrameworkElementOnLostMouseCapture;

			ReleaseMouseCapture();

			base.OnDetaching();
		}

		private void OnTargetMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (ClickMode == ClickMode.Hover)
				return;

			if (e.Handled)
				return;

			if (CheckMouseEventSource(e) == false)
				return;

			if (ClickMode == ClickMode.Press)
			{
				e.Handled = true;

				IsOpen = !IsOpen;

				return;
			}

			_isMouseCaptured = FrameworkElement.CaptureMouse();

			if (_isMouseCaptured == false)
				return;

			_pressedState = IsOpen;
			e.Handled = true;
		}

		private void OnTargetMouseEnter(object sender, MouseEventArgs mouseEventArgs)
		{
			if (ClickMode == ClickMode.Hover)
				IsOpen = true;
		}

		private void OnTargetMouseLeave(object sender, MouseEventArgs e)
		{
			if (ClickMode == ClickMode.Hover)
				IsOpen = false;
		}

		private void OnTargetMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (ClickMode != ClickMode.Release)
				return;

			if (_isMouseCaptured == false)
				return;

			if (e.Handled)
				return;

			ReleaseMouseCapture();

			if (CheckMouseEventSource(e) == false)
				return;

			e.Handled = true;

			if (IsOpen == _pressedState)
				IsOpen = !IsOpen;
		}

		private void ReleaseMouseCapture()
		{
			if (_isMouseCaptured)
				FrameworkElement.ReleaseMouseCapture();

			_isMouseCaptured = false;
		}
	}
}