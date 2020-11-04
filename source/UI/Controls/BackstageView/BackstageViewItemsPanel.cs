// <copyright file="BackstageViewItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.BackstageView
{
	public class BackstageViewItemsPanel : ItemsPanel<BackstageViewItem>, IStackPanel
	{
		#region Properties

		internal BackstageViewItemsPresenter ItemsPresenter { get; set; }

		#endregion

		#region  Methods

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return StackPanelLayout.Arrange(this, finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return StackPanelLayout.Measure(this, availableSize);
		}

		#endregion

		#region Interface Implementations

		#region IOrientedPanel

		Orientation IOrientedPanel.Orientation => Orientation.Vertical;

		#endregion

		#endregion
	}
}