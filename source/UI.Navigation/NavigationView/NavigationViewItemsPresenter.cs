// <copyright file="NavigationViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewItemsPresenterTemplateContract))]
	public class NavigationViewItemsPresenter : NavigationViewItemsPresenterBase<NavigationViewControl, NavigationViewItemBase, NavigationViewItemCollection, NavigationViewPanel>
	{
		static NavigationViewItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewItemsPresenter>();
		}

		public NavigationViewItemsPresenter()
		{
			this.OverrideStyleKey<NavigationViewItemsPresenter>();
		}
	}
}