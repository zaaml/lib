// <copyright file="DropDownSelectorBase.DropDown.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem>
	{
		private protected override void OnIsDropDownOpenChangedInternal()
		{
			base.OnIsDropDownOpenChangedInternal();

			OnIsDropDownOpenChangedSelector();
		}
	}
}