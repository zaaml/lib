// <copyright file="WrapPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels
{
	public sealed class WrapPanel : Panel, IWrapPanel
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, WrapPanel>
			("Orientation", w => w.InvalidateMeasure, w => OnCoerceOrientation);

		public static readonly DependencyProperty ItemWidthProperty = DPM.Register<double, WrapPanel>
			("ItemWidth", double.NaN, w => w.InvalidateMeasure, w => OnCoerceItemSize);

		public static readonly DependencyProperty ItemHeightProperty = DPM.Register<double, WrapPanel>
			("ItemHeight", double.NaN, w => w.InvalidateMeasure, w => OnCoerceItemSize);

		protected override bool HasLogicalOrientation => true;

		protected override Orientation LogicalOrientation => Orientation;

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return WrapPanelLayout.Arrange(this, finalSize);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return WrapPanelLayout.Measure(this, availableSize);
		}

		private static object OnCoerceItemSize(object baseValue)
		{
			var newItemSize = (double) baseValue;

			if (!newItemSize.IsNaN() && (newItemSize <= 0.0 || newItemSize.IsInfinity()))
				throw new ArgumentOutOfRangeException("Item Width or Height property set to wrong value");

			return newItemSize;
		}

		private static object OnCoerceOrientation(object baseValue)
		{
			var newOrientation = (Orientation) baseValue;

			newOrientation.ValidateValue("Orientation property set to wrong value");

			return newOrientation;
		}

		public double ItemHeight
		{
			get => (double) GetValue(ItemHeightProperty);
			set => SetValue(ItemHeightProperty, value);
		}

		public double ItemWidth
		{
			get => (double) GetValue(ItemWidthProperty);
			set => SetValue(ItemWidthProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
	}
}