// <copyright file="DropDownSelectorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem> : DropDownItemsControl<TItemsControl, TItem>
		where TItemsControl : Control
		where TItem : Control
	{
		internal bool IsSelectionHandling { get; set; }

		private protected override void OnItemsControlChanged(TItemsControl oldControl, TItemsControl newControl)
		{
			base.OnItemsControlChanged(oldControl, newControl);

			OnSelectorControllerChanged(GetSelectorController(oldControl), GetSelectorController(newControl));
			OnFocusNavigatorChanged(GetFocusNavigator(oldControl), GetFocusNavigator(newControl));
		}
	}
}