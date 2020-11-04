// <copyright file="DropDownSelectorBase.Keyboard.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;
using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem>
	{
		private ItemsControlNavigatorKeyboardEventProcessor _itemsKeyboard;
		private MainKeyboardEventProcessor _mainKeyboard;

		private protected virtual DropDownSelectorKeyboardEventProcessor ActualKeyboardEventProcessor => MainKeyboard;

		private protected virtual bool HandleApplyCancelByKeyboard => IsDropDownOpen;

		private protected ItemsControlNavigatorKeyboardEventProcessor ItemsKeyboard => _itemsKeyboard ??= CreateItemsControlKeyboard();

		private protected MainKeyboardEventProcessor MainKeyboard => _mainKeyboard ??= CreateMainKeyboard();

		internal bool ShouldHandleFocusNavigationKey { get; set; } = true;

		private protected virtual void ApplyByKeyboard()
		{
			CommitSelection();
		}

		private protected virtual void CancelByKeyboard()
		{
			CancelSelection();
		}

		private protected virtual ItemsControlNavigatorKeyboardEventProcessor CreateItemsControlKeyboard()
		{
			return new ItemsControlNavigatorKeyboardEventProcessor(this);
		}

		private protected virtual MainKeyboardEventProcessor CreateMainKeyboard()
		{
			return new MainKeyboardEventProcessor(this);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			ActualKeyboardEventProcessor.OnKeyDownInternal(e);

			if (e.Handled)
				return;

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			ActualKeyboardEventProcessor.OnKeyUpInternal(e);

			if (e.Handled)
				return;

			base.OnKeyUp(e);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			ActualKeyboardEventProcessor.OnPreviewKeyDownInternal(e);

			if (e.Handled)
				return;

			base.OnPreviewKeyDown(e);
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			ActualKeyboardEventProcessor.OnPreviewKeyUpInternal(e);

			if (e.Handled)
				return;

			base.OnPreviewKeyUp(e);
		}

		private protected abstract class DropDownSelectorKeyboardEventProcessor : KeyboardEventProcessor
		{
			protected DropDownSelectorKeyboardEventProcessor(DropDownSelectorBase<TItemsControl, TItem> dropDownSelector)
			{
				DropDownSelector = dropDownSelector;
			}

			public DropDownSelectorBase<TItemsControl, TItem> DropDownSelector { get; }

			public bool BaseHandleKey(Key key)
			{
				if (DropDownSelector.HandleApplyCancelByKeyboard == false)
					return HandleFocusNavigationKey(key);

				switch (key)
				{
					case Key.Escape:
						DropDownSelector.CancelByKeyboard();

						return true;
					case Key.Enter:
						DropDownSelector.ApplyByKeyboard();

						return true;

					default:
						return HandleFocusNavigationKey(key);
				}
			}

			private bool HandleFocusNavigationKey(Key key)
			{
				if (DropDownSelector.IsDropDownOpen)
					return false;

				if (DropDownSelector.ShouldHandleFocusNavigationKey == false)
					return false;

				return key switch
				{
					Key.Down => true,
					Key.Up => true,
					Key.Left => true,
					Key.Right => true,
					_ => false
				};
			}
		}

		private protected class ItemsControlNavigatorKeyboardEventProcessor : DropDownSelectorKeyboardEventProcessor
		{
			public ItemsControlNavigatorKeyboardEventProcessor(DropDownSelectorBase<TItemsControl, TItem> dropDownSelector) : base(dropDownSelector)
			{
			}

			public bool HandleKey(Key key)
			{
				if (DropDownSelector.IsDropDownOpen == false)
					return BaseHandleKey(key);

				var focusNavigator = DropDownSelector.FocusNavigator;

				return focusNavigator != null && focusNavigator.HandleNavigationKey(key) || BaseHandleKey(key);
			}

			protected override void OnKeyDown(KeyEventArgs e)
			{
				e.Handled = HandleKey(e.Key);
			}

			protected override void OnPreviewKeyDown(KeyEventArgs e)
			{
				e.Handled = HandleKey(e.Key);
			}
		}

		private protected class MainKeyboardEventProcessor : DropDownSelectorKeyboardEventProcessor
		{
			public MainKeyboardEventProcessor(DropDownSelectorBase<TItemsControl, TItem> dropDownSelector) : base(dropDownSelector)
			{
			}

			public bool HandleKey(KeyEventArgs e)
			{
				return e.Key switch
				{
					Key.Up => DropDownSelector.ItemsKeyboard.HandleKey(e.Key),
					Key.Down => DropDownSelector.ItemsKeyboard.HandleKey(e.Key),
					_ => BaseHandleKey(e.Key)
				};
			}

			protected override void OnKeyDown(KeyEventArgs e)
			{
				e.Handled = HandleKey(e);
			}

			protected override void OnPreviewKeyDown(KeyEventArgs e)
			{
				e.Handled = HandleKey(e);
			}
		}
	}
}