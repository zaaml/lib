// <copyright file="TabViewSelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
	internal sealed class TabViewSelectorController : SelectorController<TabViewControl, TabViewItem>
	{
		public TabViewSelectorController(TabViewControl tabViewControl) : base(tabViewControl, new TabViewSelectorAdvisor(tabViewControl))
		{
		}
	}
}