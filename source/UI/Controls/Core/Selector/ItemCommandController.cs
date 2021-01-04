// <copyright file="ItemCommandController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class ItemCommandController<TItemsControl, TItem> : IItemCommandController<TItem>
		where TItemsControl : ItemsControlBase
		where TItem : System.Windows.Controls.Control, IManagedCommandItem
	{
		private TItem _currentItem;

		private byte _packedValue;

		public ItemCommandController(TItemsControl itemsControl)
		{
			ItemsControl = itemsControl;
		}

		private bool AllowEnter => false;

		private static bool AllowSpace => true;

		private TItem CurrentItem
		{
			get => _currentItem;
			set
			{
				if (ReferenceEquals(_currentItem, value))
					return;

				if (_currentItem != null)
					_currentItem.IsPressed = false;

				_currentItem = value;

				if (_currentItem != null)
					_currentItem.IsPressed = true;
			}
		}

		private bool IsLeftButtonPressed => Mouse.LeftButton == MouseButtonState.Pressed;

		private bool IsMouseCaptured
		{
			get => PackedValue.IsMouseCaptured.GetValue(_packedValue);
			set => PackedValue.IsMouseCaptured.SetValue(ref _packedValue, value);
		}

		private bool IsSpaceOrEnterKeyDown
		{
			get => PackedValue.IsSpaceKeyDown.GetValue(_packedValue);
			set => PackedValue.IsSpaceKeyDown.SetValue(ref _packedValue, value);
		}

		public TItemsControl ItemsControl { get; }

		private bool MouseButtonDownEventCanClick
		{
			get => PackedValue.DownEventCanClick.GetValue(_packedValue);
			set => PackedValue.DownEventCanClick.SetValue(ref _packedValue, value);
		}

		private void CaptureMouse()
		{
			IsMouseCaptured = Mouse.Capture(ItemsControl, CaptureMode.SubTree);
		}

		private bool CheckMouseEventSource(TItem item, MouseButtonEventArgs e)
		{
			return MouseInternal.IsMouseButtonClick(item, PresentationTreeUtils.GetUIElementEventSource(e.OriginalSource));
		}

		private void InvokeCommand(TItem item)
		{
			if (CommandHelper.CanExecute(item.Command, item.CommandParameter, item.CommandTarget ?? item) == false)
				return;

			CommandHelper.Execute(item.Command, item.CommandParameter, item.CommandTarget ?? item);
		}

		private void OnClick(TItem item)
		{
			try
			{
				item.OnPreClick();

				if (item.InvokeCommandBeforeClick)
				{
					InvokeCommand(item);

					item.OnClick();
				}
				else
				{
					item.OnClick();

					InvokeCommand(item);
				}
			}
			finally
			{
				item.OnPostClick();
			}
		}

		private void OnItemKeyDown(TItem item, KeyEventArgs e)
		{
			if (e.Handled)
				return;

			if (e.Key == Key.Space && AllowSpace)
			{
				if (IsMouseCaptured == false && ReferenceEquals(e.OriginalSource, item))
				{
					IsSpaceOrEnterKeyDown = true;
					item.IsPressed = true;

					CaptureMouse();

					if (item.ClickMode == ClickMode.Press)
						item.OnClick();

					e.Handled = true;
				}
			}
			else if (e.Key == Key.Enter && AllowEnter)
			{
				if (ReferenceEquals(e.OriginalSource, item))
				{
					IsSpaceOrEnterKeyDown = false;
					item.IsPressed = false;

					ReleaseMouseCapture();

					OnClick(item);
					e.Handled = true;
				}
			}
			else
			{
				if (IsSpaceOrEnterKeyDown)
				{
					item.IsPressed = false;
					IsSpaceOrEnterKeyDown = false;
					ReleaseMouseCapture();
				}
			}
		}

		private void OnItemKeyUp(TItem item, KeyEventArgs e)
		{
			if (e.Handled || IsSpaceOrEnterKeyDown == false)
				return;

			if (e.Key == Key.Space && AllowSpace)
			{
				IsSpaceOrEnterKeyDown = false;

				if (IsLeftButtonPressed == false)
				{
					var shouldClick = item.IsPressed && item.ClickMode == ClickMode.Release;

					ReleaseMouseCapture();

					if (shouldClick)
						OnClick(item);
				}
				else
				{
					if (IsMouseCaptured)
						UpdateIsPressed(item);
				}

				e.Handled = true;
			}
		}

		private void OnItemMouseEnter(TItem item, MouseEventArgs e)
		{
			CurrentItem = item;
		}

		private void OnItemMouseLeave(TItem item, MouseEventArgs e)
		{
			if (ReferenceEquals(CurrentItem, item))
				CurrentItem = null;
		}

		private void OnItemMouseLeftButtonDown(TItem item, MouseButtonEventArgs e)
		{
			MouseButtonDownEventCanClick = item.CanClick;

			if (CheckMouseEventSource(item, e) == false)
				return;

			e.Handled = true;

			item.FocusControl();

			// MouseButton could be released during method calls. Check here and after to ensure it is still pressed.
			if (IsLeftButtonPressed)
			{
				CaptureMouse();

				if (IsMouseCaptured)
				{
					if (IsLeftButtonPressed)
					{
						if (!item.IsPressed)
							item.IsPressed = true;
					}
					else
						ReleaseMouseCapture();
				}

				item.IsPressed = true;
			}

			if (item.ClickMode != ClickMode.Press || item.CanClick == false)
				return;

			var exceptionThrown = true;

			try
			{
				OnClick(item);

				exceptionThrown = false;
			}
			finally
			{
				if (exceptionThrown)
				{
					item.IsPressed = false;

					ReleaseMouseCapture();
				}
			}
		}

		private void OnItemMouseLeftButtonUp(TItem item, MouseButtonEventArgs e)
		{
			try
			{
				if (IsMouseCaptured == false)
					return;

				var selfCapture = ReferenceEquals(Mouse.Captured, ItemsControl);

				if (selfCapture == false && CheckMouseEventSource(item, e) == false)
					return;

				e.Handled = true;

				var shouldClick = MouseButtonDownEventCanClick && item.CanClick && IsSpaceOrEnterKeyDown == false && item.IsPressed && item.ClickMode == ClickMode.Release;

				if (shouldClick && e.IsWithin(item))
					OnClick(item);
			}
			finally
			{
				if (IsSpaceOrEnterKeyDown == false)
				{
					ReleaseMouseCapture();
					item.IsPressed = false;
				}
			}
		}

		private void OnItemMouseMove(TItem item, MouseEventArgs e)
		{
			if (!IsMouseCaptured || !IsLeftButtonPressed || IsSpaceOrEnterKeyDown)
				return;

			UpdateIsPressed(item);

			e.Handled = true;
		}

		private void OnLostKeyboardFocus(RoutedEventArgs e)
		{
			if (ReferenceEquals(e.OriginalSource, ItemsControl) == false)
				return;

			CurrentItem = null;

			ReleaseMouseCapture();

			IsSpaceOrEnterKeyDown = false;
		}

		private void OnLostMouseCapture(MouseEventArgs e)
		{
			if (IsMouseCaptured && ReferenceEquals(e.OriginalSource, ItemsControl) && IsSpaceOrEnterKeyDown == false)
				CurrentItem = null;

			IsMouseCaptured = false;
		}

		private void ReleaseMouseCapture()
		{
			if (IsMouseCaptured == false)
				return;

			if (ReferenceEquals(Mouse.Captured, ItemsControl))
				ItemsControl.ReleaseMouseCapture();
		}

		private void UpdateIsPressed(TItem item)
		{
			if (item.GetIsMouseOver())
			{
				if (item.IsPressed == false)
					item.IsPressed = true;
			}
			else if (item.IsPressed)
				item.IsPressed = false;
		}

		public void RaiseOnClick(TItem item)
		{
			OnClick(item);
		}

		void IItemCommandController<TItem>.OnItemMouseMove(TItem item, MouseEventArgs mouseEventArgs)
		{
			OnItemMouseMove(item, mouseEventArgs);
		}

		void IItemCommandController<TItem>.OnItemKeyUp(TItem item, KeyEventArgs e)
		{
			OnItemKeyUp(item, e);
		}

		void IItemCommandController<TItem>.OnLostKeyboardFocus(RoutedEventArgs e)
		{
			OnLostKeyboardFocus(e);
		}

		void IItemCommandController<TItem>.OnLostMouseCapture(MouseEventArgs e)
		{
			OnLostMouseCapture(e);
		}

		void IItemCommandController<TItem>.OnItemMouseEnter(TItem item, MouseEventArgs e)
		{
			OnItemMouseEnter(item, e);
		}

		void IItemCommandController<TItem>.OnItemMouseLeave(TItem item, MouseEventArgs e)
		{
			OnItemMouseLeave(item, e);
		}

		void IItemCommandController<TItem>.OnItemMouseLeftButtonDown(TItem item, MouseButtonEventArgs e)
		{
			OnItemMouseLeftButtonDown(item, e);
		}

		void IItemCommandController<TItem>.OnItemMouseLeftButtonUp(TItem item, MouseButtonEventArgs e)
		{
			OnItemMouseLeftButtonUp(item, e);
		}

		void IItemCommandController<TItem>.OnItemKeyDown(TItem item, KeyEventArgs keyEventArgs)
		{
			OnItemKeyDown(item, keyEventArgs);
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

	internal interface IItemCommandController<in TItem>
		where TItem : System.Windows.Controls.Control, IManagedCommandItem
	{
		void OnItemKeyDown(TItem item, KeyEventArgs e);

		void OnItemKeyUp(TItem item, KeyEventArgs e);

		void OnItemMouseEnter(TItem item, MouseEventArgs e);

		void OnItemMouseLeave(TItem item, MouseEventArgs e);

		void OnItemMouseLeftButtonDown(TItem item, MouseButtonEventArgs e);

		void OnItemMouseLeftButtonUp(TItem item, MouseButtonEventArgs e);

		void OnItemMouseMove(TItem item, MouseEventArgs e);

		void OnLostKeyboardFocus(RoutedEventArgs e);

		void OnLostMouseCapture(MouseEventArgs e);

		void RaiseOnClick(TItem item);
	}

	internal interface IManagedCommandItem : ICommandControl
	{
		bool CanClick { get; }
		ClickMode ClickMode { get; }

		bool InvokeCommandBeforeClick { get; }

		bool IsMouseOver { get; }

		bool IsPressed { get; set; }

		void FocusControl();

		void OnClick();

		void OnPostClick();

		void OnPreClick();
	}
}