// <copyright file="OverflowItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Primitives.Overflow
{
	internal sealed class OverflowItemGenerator<TItem> : ItemGenerator<OverflowItem<TItem>>
		where TItem : Control, IOverflowableItem<TItem>
	{
		#region  Methods

		protected override void AttachItem(OverflowItem<TItem> overflowItem, object itemSource)
		{
		}

		protected override OverflowItem<TItem> CreateItem(object itemSource)
		{
			var item = (TItem) itemSource;

			return item.OverflowController.OverflowHost;
		}

		protected override void DetachItem(OverflowItem<TItem> overflowItem, object itemSource)
		{
		}

		protected override void DisposeItem(OverflowItem<TItem> item, object itemSource)
		{
		}

		#endregion
	}
}