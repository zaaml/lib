// <copyright file="DefaultBackstageViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.BackstageView
{
	internal sealed class DefaultBackstageViewItemGenerator : BackstageViewItemGeneratorBase, IDelegatedGenerator<BackstageViewItem>
	{
		#region  Methods

		protected override void AttachItem(BackstageViewItem item, object itemSource)
		{
			Implementation.AttachItem(item, itemSource);
		}

		protected override BackstageViewItem CreateItem(object itemSource)
		{
			return Implementation.CreateItem(itemSource);
		}

		protected override void DetachItem(BackstageViewItem item, object itemSource)
		{
			Implementation.DetachItem(item, itemSource);
		}

		protected override void DisposeItem(BackstageViewItem item, object itemSource)
		{
			Implementation.DisposeItem(item, itemSource);
		}

		#endregion

		#region Interface Implementations

		#region IDelegatedGenerator<BackstageViewItem>

		public IItemGenerator<BackstageViewItem> Implementation { get; set; }

		#endregion

		#endregion
	}
}