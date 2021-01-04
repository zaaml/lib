// <copyright file="ButtonController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Primitives
{
	internal class ButtonController<T> : CommandController<T>, IButtonController where T : Control, IManagedButton
	{
		private byte _packedValue;

		public ButtonController(T control) : base(control)
		{
		}

		// ReSharper disable once MemberCanBeMadeStatic.Local
		private bool AllowEnter => (bool) Control.GetValue(KeyboardNavigation.AcceptsReturnProperty);

		private static bool AllowSpace => (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.Alt;

		private bool CanClick => Control.CanClick;

		private ClickMode ClickMode => Control.ClickMode;

		private bool IsLeftButtonPressed => Mouse.LeftButton == MouseButtonState.Pressed;

		private bool IsMouseCaptured
		{
			get => PackedValue.IsMouseCaptured.GetValue(_packedValue);
			set => PackedValue.IsMouseCaptured.SetValue(ref _packedValue, value);
		}

		private bool IsMouseOver => Control.IsMouseOver;

		private bool IsPressed
		{
			get => Control.IsPressed;
			set => Control.IsPressed = value;
		}

		private bool IsSpaceOrEnterKeyDown
		{
			get => PackedValue.IsSpaceKeyDown.GetValue(_packedValue);
			set => PackedValue.IsSpaceKeyDown.SetValue(ref _packedValue, value);
		}

		private bool MouseButtonDownEventCanClick
		{
			get => PackedValue.DownEventCanClick.GetValue(_packedValue);
			set => PackedValue.DownEventCanClick.SetValue(ref _packedValue, value);
		}

		private void CaptureMouse()
		{
			IsMouseCaptured = Control.CaptureMouse();
		}

		private bool CheckMouseEventSource(MouseButtonEventArgs e)
		{
			return MouseInternal.IsMouseButtonClick(Control, PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource));
		}

		private bool HandleIsMouseOverChanged()
		{
			if (ClickMode != ClickMode.Hover)
				return false;

			if (IsMouseOver)
			{
				IsPressed = true;
				OnClick();
			}
			else
				IsPressed = false;

			return true;
		}

		private void OnClick()
		{
			try
			{
				Control.OnPreClick();

				if (Control.InvokeCommandBeforeClick)
				{
					InvokeCommand();
					Control.OnClick();
				}
				else
				{
					Control.OnClick();
					InvokeCommand();
				}

				if (Control.IsInPopupTree() == false)
					return;

				IsPressed = false;

				Control.ClosePopupTree();
			}
			finally
			{
				Control.OnPostClick();
			}
		}

		private void OnKeyDown(KeyEventArgs e)
		{
			if (ClickMode == ClickMode.Hover || e.Handled)
				return;

			if (e.Key == Key.Space && AllowSpace)
			{
				if (!IsMouseCaptured && ReferenceEquals(e.OriginalSource, Control))
				{
					IsSpaceOrEnterKeyDown = true;
					IsPressed = true;

					CaptureMouse();

					if (ClickMode == ClickMode.Press)
						OnClick();

					e.Handled = true;
				}
			}
			else if (e.Key == Key.Enter && AllowEnter)
			{
				if (ReferenceEquals(e.OriginalSource, Control))
				{
					IsSpaceOrEnterKeyDown = false;
					IsPressed = false;

					ReleaseMouseCapture();

					OnClick();
					e.Handled = true;
				}
			}
			else
			{
				if (IsSpaceOrEnterKeyDown)
				{
					IsPressed = false;
					IsSpaceOrEnterKeyDown = false;
					ReleaseMouseCapture();
				}
			}
		}

		private void OnKeyUp(KeyEventArgs e)
		{
			if (ClickMode == ClickMode.Hover || e.Handled || IsSpaceOrEnterKeyDown == false)
				return;

			if (e.Key == Key.Space && AllowSpace)
			{
				IsSpaceOrEnterKeyDown = false;

				if (IsLeftButtonPressed == false)
				{
					var shouldClick = IsPressed && ClickMode == ClickMode.Release;

					ReleaseMouseCapture();

					if (shouldClick)
						OnClick();
				}
				else
				{
					if (IsMouseCaptured)
						UpdateIsPressed();
				}

				e.Handled = true;
			}
		}

		private void OnLostKeyboardFocus(RoutedEventArgs e)
		{
			if (ClickMode == ClickMode.Hover)
				return;

			if (ReferenceEquals(e.OriginalSource, Control) == false) return;

			if (IsPressed)
				IsPressed = false;

			ReleaseMouseCapture();

			IsSpaceOrEnterKeyDown = false;
		}

		private void OnLostMouseCapture(MouseEventArgs e)
		{
			if (IsMouseCaptured && ReferenceEquals(e.OriginalSource, Control) && ClickMode != ClickMode.Hover && !IsSpaceOrEnterKeyDown)
				IsPressed = false;

			IsMouseCaptured = false;
		}

		private void OnMouseEnter(MouseEventArgs e)
		{
			if (HandleIsMouseOverChanged())
			{
				e.Handled = true;
			}
		}

		private void OnMouseLeave(MouseEventArgs e)
		{
			if (HandleIsMouseOverChanged())
			{
				e.Handled = true;
			}
		}

		private void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			MouseButtonDownEventCanClick = CanClick;

			if (ClickMode == ClickMode.Hover)
				return;

			if (CheckMouseEventSource(e) == false)
				return;

			e.Handled = true;

			Control.FocusControl();

			// MouseButton could be released during method calls. Check here and after to ensure it is still pressed.
			if (IsLeftButtonPressed)
			{
				CaptureMouse();

				if (IsMouseCaptured)
				{
					if (IsLeftButtonPressed)
					{
						if (!IsPressed)
							IsPressed = true;
					}
					else
						ReleaseMouseCapture();
				}

				IsPressed = true;
			}

			if (ClickMode != ClickMode.Press || CanClick == false)
				return;

			var exceptionThrown = true;

			try
			{
				OnClick();
				exceptionThrown = false;
			}
			finally
			{
				if (exceptionThrown)
				{
					IsPressed = false;
					ReleaseMouseCapture();
				}
			}
		}

		private void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (ClickMode == ClickMode.Hover)
				return;

			try
			{
				if (IsMouseCaptured == false)
					return;

				var selfCapture = ReferenceEquals(Mouse.Captured, Control);

				if (selfCapture == false && CheckMouseEventSource(e) == false)
					return;

				e.Handled = true;

				var shouldClick = MouseButtonDownEventCanClick && CanClick && IsSpaceOrEnterKeyDown == false && IsPressed && ClickMode == ClickMode.Release;

				if (shouldClick && e.IsWithin(Control))
					OnClick();
			}
			finally
			{
				if (IsSpaceOrEnterKeyDown == false)
				{
					ReleaseMouseCapture();
					IsPressed = false;
				}
			}
		}

		private void OnMouseMove(MouseEventArgs e)
		{
			if (ClickMode == ClickMode.Hover || !IsMouseCaptured || !IsLeftButtonPressed || IsSpaceOrEnterKeyDown) 
				return;

			UpdateIsPressed();

			e.Handled = true;
		}

		private void RaiseOnClick()
		{
			OnClick();
		}

		private void ReleaseMouseCapture()
		{
			if (IsMouseCaptured == false)
				return;

			if (ReferenceEquals(Mouse.Captured, Control))
				Control.ReleaseMouseCapture();
		}

		private void UpdateIsPressed()
		{
			if (Control.GetIsMouseOver())
			{
				if (!Control.IsPressed)
					IsPressed = true;
			}
			else if (IsPressed)
				IsPressed = false;
		}

		void IButtonController.UpdateCanExecute()
		{
			UpdateCanExecuteInternal();
		}

		void IButtonController.OnMouseMove(MouseEventArgs mouseEventArgs)
		{
			OnMouseMove(mouseEventArgs);
		}

		void IButtonController.RaiseOnClick()
		{
			RaiseOnClick();
		}

		void IButtonController.OnKeyUp(KeyEventArgs e)
		{
			OnKeyUp(e);
		}

		void IButtonController.OnLostKeyboardFocus(RoutedEventArgs e)
		{
			OnLostKeyboardFocus(e);
		}

		void IButtonController.OnLostMouseCapture(MouseEventArgs e)
		{
			OnLostMouseCapture(e);
		}

		void IButtonController.OnMouseEnter(MouseEventArgs e)
		{
			OnMouseEnter(e);
		}

		void IButtonController.OnMouseLeave(MouseEventArgs e)
		{
			OnMouseLeave(e);
		}

		void IButtonController.OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			OnMouseLeftButtonDown(e);
		}

		void IButtonController.OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			OnMouseLeftButtonUp(e);
		}

		void IButtonController.OnKeyDown(KeyEventArgs keyEventArgs)
		{
			OnKeyDown(keyEventArgs);
		}

		private static class PackedValue
		{
			static PackedValue()
			{
				var allocator = new PackedValueAllocator();

				IsMouseCaptured = allocator.AllocateBoolItem();
				IsSpaceKeyDown = allocator.AllocateBoolItem();
				IsLeftButtonPressed = allocator.AllocateBoolItem();
				DownEventCanClick = allocator.AllocateBoolItem();
			}

			// ReSharper disable StaticMemberInGenericType
			public static readonly PackedBoolItemDefinition IsMouseCaptured;

			public static readonly PackedBoolItemDefinition IsSpaceKeyDown;

			// ReSharper disable once NotAccessedField.Local
			public static readonly PackedBoolItemDefinition IsLeftButtonPressed;

			public static readonly PackedBoolItemDefinition DownEventCanClick;
			// ReSharper restore StaticMemberInGenericType
		}
	}
}