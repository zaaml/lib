// <copyright file="StackPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels.Primitives
{
	public class StackPanel : Panel, IStackPanelAdvanced
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, StackPanel>
			("Orientation", Orientation.Vertical, s => s.OnOrientationPropertyChangedPrivate);

		public static readonly DependencyProperty SpacingProperty = DPM.Register<double, StackPanel>
			("Spacing", 0.0, s => s.OnSpacingPropertyChangedPrivate);

		protected override bool HasLogicalOrientation => true;

		protected override Orientation LogicalOrientation => Orientation;

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public double Spacing
		{
			get => (double) GetValue(SpacingProperty);
			set => SetValue(SpacingProperty, value);
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return StackPanelLayout.Arrange(this, finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return StackPanelLayout.Measure(this, availableSize);
		}

		private void OnOrientationPropertyChangedPrivate()
		{
			InvalidateMeasure();
		}

		private void OnSpacingPropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidateMeasure();
		}

		Orientation IOrientedPanel.Orientation => Orientation;

		double IStackPanelAdvanced.Spacing => Spacing;
	}
}