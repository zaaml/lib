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
		private bool AllowEnter
		{
			get
			{
#if SILVERLIGHT
				return true;
#else
				return (bool) Control.GetValue(KeyboardNavigation.AcceptsReturnProperty);
#endif
			}
		}

		private static bool AllowSpace => (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.Alt;

		private bool CanClick => Control.CanClick;

		private ClickMode ClickMode => Control.ClickMode;

		private bool IsLeftButtonPressed
		{
#if SILVERLIGHT
			get { return PackedValue.IsLeftButtonPressed.GetValue(_packedValue); }
			set { PackedValue.IsLeftButtonPressed.SetValue(ref _packedValue, value); }
#else
			get { return Mouse.LeftButton == MouseButtonState.Pressed; }
			// ReSharper disable once ValueParameterNotUsed
			set { }
#endif
		}

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

#if SILVERLIGHT
			if ((e.Key == Key.Space && AllowSpace) || (e.Key == Key.Enter && AllowEnter))
#else
			if (e.Key == Key.Space && AllowSpace)
#endif
			{
				if (!IsMouseCaptured && ReferenceEquals(e.OriginalSource, Control))
				{
					IsSpaceOrEnterKeyDown = true;
					IsPressed = true;

#if !SILVERLIGHT
					CaptureMouse();
#endif
					if (ClickMode == ClickMode.Press)
						OnClick();

					e.Handled = true;
				}
			}
#if !SILVERLIGHT
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
#endif
			else
			{
				if (IsSpaceOrEnterKeyDown)
				{
					IsPressed = false;
					IsSpaceOrEnterKeyDown = false;
#if !SILVERLIGHT
					ReleaseMouseCapture();
#endif
				}
			}
		}

		private void OnKeyUp(KeyEventArgs e)
		{
			if (ClickMode == ClickMode.Hover || e.Handled || IsSpaceOrEnterKeyDown == false)
				return;

#if SILVERLIGHT
			if ((e.Key == Key.Space && AllowSpace) || (e.Key == Key.Enter && AllowEnter))
#else
			if (e.Key == Key.Space && AllowSpace)
#endif
			{
				IsSpaceOrEnterKeyDown = false;

				if (IsLeftButtonPressed == false)
				{
					var shouldClick = IsPressed && ClickMode == ClickMode.Release;

#if !SILVERLIGHT
					ReleaseMouseCapture();
#endif
					if (shouldClick)
						OnClick();

#if SILVERLIGHT
						IsPressed = false;
#endif
				}
#if !SILVERLIGHT
				else
				{
					if (IsMouseCaptured)
						UpdateIsPressed();
				}
#endif
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
#if !SILVERLIGHT
				e.Handled = true;
#endif
			}
		}

		private void OnMouseLeave(MouseEventArgs e)
		{
			if (HandleIsMouseOverChanged())
			{
#if !SILVERLIGHT
				e.Handled = true;
#endif
			}
		}

		private void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			IsLeftButtonPressed = true;

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
			IsLeftButtonPressed = false;

			if (ClickMode == ClickMode.Hover)
				return;

			try
			{
				if (IsMouseCaptured == false)
					return;

#if SILVERLIGHT
				if (MouseInt.IsMouseInsideEventHelper(Control, PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource)) == false)
					return;
#else
				var selfCapture = ReferenceEquals(Mouse.Captured, Control);

				if (selfCapture == false && CheckMouseEventSource(e) == false)
					return;
#endif

				e.Handled = true;

				var shouldClick = MouseButtonDownEventCanClick && CanClick && IsSpaceOrEnterKeyDown == false && IsPressed && ClickMode == ClickMode.Release;

				if (shouldClick && e.IsWithin(Control))
					OnClick();
			}
			finally
			{
				if (e.IsWithin(Control) == false)
				{
				}

				if (IsSpaceOrEnterKeyDown == false)
				{
					ReleaseMouseCapture();
					IsPressed = false;
				}
			}
		}

		private void OnMouseMove(MouseEventArgs e)
		{
			if (ClickMode == ClickMode.Hover || !IsMouseCaptured || !IsLeftButtonPressed || IsSpaceOrEnterKeyDown) return;

			UpdateIsPressed();

#if !SILVERLIGHT
			e.Handled = true;
#endif
		}

		private void RaiseOnClick()
		{
			OnClick();
		}

		private void ReleaseMouseCapture()
		{
			if (IsMouseCaptured == false)
				return;

#if SILVERLIGHT
      Control.ReleaseMouseCapture();
#else
			if (ReferenceEquals(Mouse.Captured, Control))
				Control.ReleaseMouseCapture();
#endif
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