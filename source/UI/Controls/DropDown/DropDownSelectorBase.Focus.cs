// <copyright file="DropDownSelectorBase.Focus.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem>
	{
		internal FocusNavigator<TItem> FocusNavigator => ItemsControl != null ? GetFocusNavigator(ItemsControl) : default;

		private protected abstract FocusNavigator<TItem> GetFocusNavigator(TItemsControl control);

		private bool IsKeyboardFocusWithinDropDown()
		{
			var dropDownElement = (FrameworkElement) ItemsControl;

			if (dropDownElement == null)
				return false;

			return dropDownElement.IsKeyboardFocusWithin;
		}

		private void OnFocusNavigatorChanged(FocusNavigator<TItem> oldFocusNavigator, FocusNavigator<TItem> newFocusNavigator)
		{
			if (oldFocusNavigator != null)
				oldFocusNavigator.IsLogical = false;

			if (newFocusNavigator != null)
				newFocusNavigator.IsLogical = true;
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);

			if (IsDropDownOpen)
			{
				var selectorController = SelectorController;
				var selectedItem = selectorController?.SelectedItem;

				if (selectedItem != null)
					FocusHelper.SetKeyboardFocusedElement(selectedItem);
			}
		}
	}
}