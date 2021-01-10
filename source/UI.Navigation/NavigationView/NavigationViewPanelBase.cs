// <copyright file="NavigationViewPanelBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewPanelBase<TItem> : ItemsPanel<TItem>
		where TItem : NavigationViewItemBase
	{
	}
}