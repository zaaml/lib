// <copyright file="DropDownSelectorBase.Selector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem>
	{
		internal SelectorController<TItem> SelectorController => GetSelectorController(ItemsControl);

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
			var selectorController = SelectorController;

			if (selectorController != null)
			{
				//selectorController.RestoreSelection();
				//selectorController.ResumeSelection();
			}

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

				if (selectedItem != null && selectorController.SelectItem(selectedItem) == false)
					return false;

				//selectorController.ResumeSelection();
			}

			CloseDropDown();

			return true;
		}

		private protected abstract SelectorController<TItem> GetSelectorController(TItemsControl control);

		private void OnSelectorControllerChanged(SelectorController<TItem> oldSelectorController, SelectorController<TItem> newSelectorController)
		{
			if (IsDropDownOpen == false)
				return;

			//oldSelectorController?.ResumeSelection();
			//newSelectorController?.SuspendSelection();
		}

		private void OnIsDropDownOpenChangedSelector()
		{
			var selectorController = SelectorController;

			if (selectorController == null)
				return;

			if (IsDropDownOpen)
			{
				//selectorController.SuspendSelection();

				var selectedItem = selectorController.SelectedItem;

				if (selectedItem != null)
				{
					//ItemCollection.BringIntoViewInternal(new BringIntoViewRequest<TItem>(selectedItem, BringIntoViewMode.Top));

					FocusNavigator.FocusedItem = selectedItem;

					FocusHelper.QueryFocus(selectedItem);
				}
				else
				{
					ItemCollection.BringIntoViewInternal(0);
					FocusNavigator.ClearFocus();
				}
			}
			else
			{
				if (selectorController.IsSelectionSuspended)
				{
					//selectorController.RestoreSelection();
					//selectorController.ResumeSelection();
				}
			}
		}
	}
}