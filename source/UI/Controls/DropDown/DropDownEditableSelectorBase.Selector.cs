// <copyright file="DropDownEditableSelectorBase.Selector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem>
	{
		protected override bool CancelSelectionCore()
		{
			return CancelEdit();
		}

		protected override bool CommitSelectionCore()
		{
			return CommitEdit();
		}
	}
}