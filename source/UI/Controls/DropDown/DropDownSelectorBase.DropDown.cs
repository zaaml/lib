// <copyright file="DropDownSelectorBase.DropDown.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem>
	{
		public virtual void CloseDropDown()
		{
			SetIsDropDownOpen(false);
		}

		internal override void OnIsDropDownOpenChangedInternal()
		{
			base.OnIsDropDownOpenChangedInternal();

			var selectorController = SelectorController;

			if (selectorController == null)
				return;

			if (IsDropDownOpen)
			{
				selectorController.SuspendSelectionChange();

				var selectedItem = selectorController.SelectedItem;

				if (selectedItem != null)
				{
					ItemCollection.BringIntoViewInternal(new BringIntoViewRequest<TItem>(selectedItem, BringIntoViewMode.Top));

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
				if (selectorController.IsSelectionChangeSuspended)
					selectorController.ResumeSelectionChange(false);
			}
		}

		public virtual void OpenDropDown()
		{
			SetIsDropDownOpen(true);
		}

		private void SetIsDropDownOpen(bool value)
		{
			this.SetCurrentValueInternal(IsDropDownOpenProperty, value ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
		}
	}
}