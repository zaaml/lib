// <copyright file="NavigationViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewPanel : NavigationViewPanelBase<NavigationViewItemBase>, IStackPanelAdvanced
	{
		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return StackPanelLayout.Arrange(this, finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return StackPanelLayout.Measure(this, availableSize);
		}

		Orientation IOrientedPanel.Orientation => Orientation.Vertical;

		double IStackPanelAdvanced.Spacing => 0;
	}
}