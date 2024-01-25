// <copyright file="ScrollViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ScrollView
{
	[TemplateContractType(typeof(ScrollViewControlTemplateContract))]
	public sealed class ScrollViewControl : ScrollViewControlBase<ScrollViewPresenter, ScrollViewPanel>
	{
		static ScrollViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ScrollViewControl>();
		}

		public ScrollViewControl()
		{
			this.OverrideStyleKey<ScrollViewControl>();
		}

		protected override ScrollViewPanelBase ScrollViewPanelCore => ScrollViewPresenterInternal?.ScrollViewPanel;

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ScrollViewPresenterInternal.ScrollView = this;

			UpdateScrollViewPanelInternal();
		}

		protected override void OnTemplateContractDetaching()
		{
			ScrollViewPresenterInternal.ScrollView = null;

			UpdateScrollViewPanelInternal();

			base.OnTemplateContractDetaching();
		}
	}
}