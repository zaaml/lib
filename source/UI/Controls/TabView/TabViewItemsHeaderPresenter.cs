// <copyright file="TabViewItemsHeaderPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
	public class TabViewItemsHeaderPresenter : ContentPresenter
	{
		static TabViewItemsHeaderPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TabViewItemsHeaderPresenter>();
		}

		public TabViewItemsHeaderPresenter()
		{
			this.OverrideStyleKey<TabViewItemsHeaderPresenter>();
		}
	}
}