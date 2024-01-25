// <copyright file="NavigationViewSelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	internal sealed class NavigationViewSelectorController : SelectorController<NavigationViewFrame, NavigationViewItem>
	{
		public NavigationViewSelectorController(NavigationViewFrame navigationViewControl) : base(navigationViewControl, new NavigationViewSelectorAdvisor(navigationViewControl))
		{
		}
	}
}