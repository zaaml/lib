// <copyright file="StackItemsPanelBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels.Primitives
{
	public class StackItemsPanelBase<TItem> : ItemsPanel<TItem>, IStackPanel
		where TItem : Control
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, StackItemsPanelBase<TItem>>
			("Orientation", Orientation.Vertical, s => s.OnOrientationChanged);

		protected override bool HasLogicalOrientation => true;

		protected override Orientation LogicalOrientation => Orientation;

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return StackPanelLayout.Arrange(this, finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return StackPanelLayout.Measure(this, availableSize);
		}

		private void OnOrientationChanged()
		{
			InvalidateMeasure();
		}

		Orientation IOrientedPanel.Orientation => Orientation;
	}
}