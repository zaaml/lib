// <copyright file="NavigationViewCommandPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewCommandPanel : NavigationViewPanelBase<NavigationViewCommandItem>, IStackPanel, IWrapPanel
	{
		private NavigationViewControl _navigationViewControl;

		private bool Expanded => NavigationViewControl?.IsPaneOpen != false;

		internal NavigationViewControl NavigationViewControl
		{
			get => _navigationViewControl;
			set
			{
				if (ReferenceEquals(_navigationViewControl, value))
					return;

				if (_navigationViewControl != null)
					_navigationViewControl.IsPaneOpenChanged -= NavigationViewControlOnIsPaneOpenChanged;

				_navigationViewControl = value;

				if (_navigationViewControl != null)
					_navigationViewControl.IsPaneOpenChanged += NavigationViewControlOnIsPaneOpenChanged;

				InvalidateMeasure();
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			if (Expanded)
				return WrapPanelLayout.Arrange(this, finalSize);

			return StackPanelLayout.Arrange(this, finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			if (Expanded)
				return WrapPanelLayout.Measure(this, availableSize);

			return StackPanelLayout.Measure(this, availableSize);
		}

		private void NavigationViewControlOnIsPaneOpenChanged(object sender, EventArgs e)
		{
			InvalidateMeasure();
		}

		Orientation IOrientedPanel.Orientation => Expanded ? Orientation.Horizontal : Orientation.Vertical;

		double IWrapPanel.ItemWidth => double.NaN;

		double IWrapPanel.ItemHeight => double.NaN;
	}
}