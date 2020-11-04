// <copyright file="DropDownSelectorBase.Selector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem>
	{
		internal SelectorController<TItem> SelectorController => GetSelectorController(ItemsControl);

		private protected void BindSelectedContent(Binding binding)
		{
			SetBinding(SelectedContentProperty, binding);
		}

		private protected void BindSelectedIcon(Binding binding)
		{
			SetBinding(SelectedIconProperty, binding);
		}

		protected void CancelSelection()
		{
			try
			{
				IsSelectionHandling = true;

				CancelSelectionCore();
			}
			finally
			{
				IsSelectionHandling = false;
			}
		}

		protected virtual bool CancelSelectionCore()
		{
			SelectorController?.ResumeSelectionChange(false);

			CloseDropDown();

			return true;
		}

		protected void CommitSelection()
		{
			try
			{
				IsSelectionHandling = true;

				CommitSelectionCore();
			}
			finally
			{
				IsSelectionHandling = false;
			}
		}

		protected virtual bool CommitSelectionCore()
		{
			var selectorController = SelectorController;

			if (selectorController != null)
			{
				var selectedItem = FocusNavigator.FocusedItem;

				if (selectedItem != null)
				{
					selectorController.SelectedItem = selectedItem;

					if (ReferenceEquals(selectorController.CurrentSelectedItem, selectedItem) == false)
						return false;
				}

				selectorController.ResumeSelectionChange(true);
			}

			CloseDropDown();

			return true;
		}

		private protected abstract SelectorController<TItem> GetSelectorController(TItemsControl control);

		private void OnSelectorControllerChanged(SelectorController<TItem> oldSelectorController, SelectorController<TItem> newSelectorController)
		{
			if (IsDropDownOpen == false)
				return;

			oldSelectorController?.ResumeSelectionChange();
			newSelectorController?.SuspendSelectionChange();
		}
	}
}