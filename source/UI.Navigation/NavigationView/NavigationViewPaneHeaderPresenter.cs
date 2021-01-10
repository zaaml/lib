// <copyright file="NavigationViewPaneHeaderPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewPaneHeaderPresenter : Control
	{
		static NavigationViewPaneHeaderPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewPaneHeaderPresenter>();
		}

		public NavigationViewPaneHeaderPresenter()
		{
			this.OverrideStyleKey<NavigationViewPaneHeaderPresenter>();
		}
	}
}