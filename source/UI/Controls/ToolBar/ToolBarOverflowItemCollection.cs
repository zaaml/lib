// <copyright file="ToolBarOverflowItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Primitives.Overflow;

namespace Zaaml.UI.Controls.ToolBar
{
	internal sealed class ToolBarOverflowItemCollection : OverflowItemCollection<ToolBarOverflowItemsPresenter, ToolBarItem>
	{
		#region Ctors

		public ToolBarOverflowItemCollection(ToolBarOverflowItemsPresenter overflowItemsPresenter) : base(overflowItemsPresenter)
		{
		}

		#endregion

		#region  Methods

		protected override void OnItemAttached(OverflowItem<ToolBarItem> item)
		{
			Control.OnItemAttached(item);

			base.OnItemAttached(item);
		}

		protected override void OnItemDetached(OverflowItem<ToolBarItem> item)
		{
			base.OnItemDetached(item);

			Control.OnItemDetached(item);
		}

		#endregion
	}
}