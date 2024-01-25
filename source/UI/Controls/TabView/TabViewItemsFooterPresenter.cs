// <copyright file="TabViewItemsFooterPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
	public class TabViewItemsFooterPresenter : ContentPresenter
	{
		static TabViewItemsFooterPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TabViewItemsFooterPresenter>();
		}

		public TabViewItemsFooterPresenter()
		{
			this.OverrideStyleKey<TabViewItemsFooterPresenter>();
		}
	}
}