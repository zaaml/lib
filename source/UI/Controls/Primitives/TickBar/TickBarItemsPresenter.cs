// <copyright file="TickBarItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	[TemplateContractType<TickBarItemsPresenterTemplateContract>]
	public sealed class TickBarItemsPresenter : ItemsPresenterBase<TickBarControl, TickBarItem, TickBarItemCollection, TickBarPanel>
	{
		public TickBarControl TickBarControl { get; internal set; }

		protected override void OnItemsHostAttached()
		{
			base.OnItemsHostAttached();

			ItemsHost.ItemsPresenter = this;
		}

		protected override void OnItemsHostDetaching()
		{
			ItemsHost.ItemsPresenter = null;

			base.OnItemsHostDetaching();
		}
	}
}