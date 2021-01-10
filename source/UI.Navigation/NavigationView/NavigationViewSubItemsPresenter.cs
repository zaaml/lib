// <copyright file="NavigationViewSubItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewSubItemsPresenterTemplateContract))]
	public class NavigationViewSubItemsPresenter : NavigationViewItemsPresenterBase<NavigationViewMenuItem, NavigationViewItemBase, NavigationViewSubItemCollection, NavigationViewPanel>
	{
		static NavigationViewSubItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewSubItemsPresenter>();
		}

		public NavigationViewSubItemsPresenter()
		{
			this.OverrideStyleKey<NavigationViewSubItemsPresenter>();
		}
	}
}