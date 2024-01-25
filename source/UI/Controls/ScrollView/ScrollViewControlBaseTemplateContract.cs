// <copyright file="ScrollViewControlBaseTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.ScrollView
{
	public abstract class ScrollViewControlBaseTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ScrollBar HorizontalScrollBar { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ScrollBar VerticalScrollBar { get; [UsedImplicitly] private set; }
	}

	public abstract class ScrollViewControlBaseTemplateContract<TScrollViewPresenter, TScrollContentPanel> : ScrollViewControlBaseTemplateContract
		where TScrollViewPresenter : ScrollViewPresenterBase<TScrollContentPanel> where TScrollContentPanel : ScrollViewPanelBase
	{
		protected abstract TScrollViewPresenter ScrollViewPresenterCore { get; }

		internal TScrollViewPresenter ScrollViewPresenterInternal => ScrollViewPresenterCore;
	}
}