// <copyright file="PopupPanel.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed partial class PopupPanel
	{
		internal bool HandleKeyboardEvents => Popup.HandleKeyboardEvents;

		internal bool HandleMouseEvents => Popup.HandleMouseEvents;

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			e.Handled = true;
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			e.Handled = true;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (HandleKeyboardEvents)
				e.Handled = true;
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (HandleKeyboardEvents)
				e.Handled = true;
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			OnMouseEvent();

			if (HandleMouseEvents)
				e.Handled = true;
		}
	}
}