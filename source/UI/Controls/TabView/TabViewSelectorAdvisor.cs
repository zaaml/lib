// <copyright file="TabViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
	internal sealed class TabViewSelectorAdvisor : SelectorBaseControllerAdvisor<TabViewControl, TabViewItem, TabViewItemCollection, TabViewItemsPresenter, TabViewItemsPanel>
	{
		public TabViewSelectorAdvisor(TabViewControl tabViewControl) : base(tabViewControl)
		{
		}
	}
}