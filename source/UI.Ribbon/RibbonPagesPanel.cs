// <copyright file="RibbonPagesPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.Ribbon
{
	public class RibbonPagesPanel : ItemsPanel<RibbonPage>, IFlexPanel
	{
		public static readonly DependencyProperty SpacingProperty = DPM.Register<double, RibbonPagesPanel>
			("Spacing", 0.0, p => p.InvalidateMeasure);

		public RibbonPagesPanel()
		{
			Layout = new FlexPanelLayout(this);
		}

		private FlexPanelLayout Layout { get; }

		public double Spacing
		{
			get => (double) GetValue(SpacingProperty);
			set => SetValue(SpacingProperty, value);
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return Layout.Arrange(finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return Layout.Measure(availableSize);
		}

		IFlexDistributor IFlexPanel.Distributor => FlexDistributor.Equalizer;

		bool IFlexPanel.HasHiddenChildren { get; set; }

		double IFlexPanel.Spacing => Spacing;

		FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

		FlexElement IFlexPanel.GetFlexElement(UIElement child)
		{
			return child.GetFlexElement(this).WithStretchDirection(FlexStretchDirection.Shrink);
		}

		bool IFlexPanel.GetIsHidden(UIElement child)
		{
			return FlexPanel.GetIsHidden(child);
		}

		void IFlexPanel.SetIsHidden(UIElement child, bool value)
		{
			FlexPanel.SetIsHidden(child, value);
		}

		Orientation IOrientedPanel.Orientation => Orientation.Horizontal;
	}
}