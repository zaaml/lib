// <copyright file="TabViewSelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
	internal sealed class TabViewSelectorAdvisor : ItemCollectionSelectorAdvisor<TabViewControl, TabViewItem>
	{
		public TabViewSelectorAdvisor(TabViewControl tabViewControl) : base(tabViewControl, tabViewControl.Items)
		{
		}
	}
}