// <copyright file="NavigationViewItemSeparator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewItemSeparator : NavigationViewItemBase
	{
		static NavigationViewItemSeparator()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewItemSeparator>();
		}

		public NavigationViewItemSeparator()
		{
			this.OverrideStyleKey<NavigationViewItemSeparator>();
		}
	}
}