// <copyright file="GridRow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Panels.Primitives
{
	public class GridRow : GridDefinition
	{
		public static readonly DependencyProperty MaxHeightProperty = DPM.Register<double, GridRow>
			("MaxHeight", double.PositiveInfinity, d => d.OnMaxHeightPropertyChangedPrivate);

		public static readonly DependencyProperty MinHeightProperty = DPM.Register<double, GridRow>
			("MinHeight", 0.0, d => d.OnMinHeightPropertyChangedPrivate);

		public static readonly DependencyProperty HeightProperty = DPM.Register<FlexLength, GridRow>
			("Height", FlexLength.Star, d => d.OnHeightPropertyChangedPrivate);

		public GridRow() : base(false)
		{
		}

		public double ActualHeight => GridPanel?.GetFinalRowDefinitionHeight(Index) ?? 0d;

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength Height
		{
			get => (FlexLength) GetValue(HeightProperty);
			set => SetValue(HeightProperty, value);
		}

		public double MaxHeight
		{
			get => (double) GetValue(MaxHeightProperty);
			set => SetValue(MaxHeightProperty, value);
		}

		public double MinHeight
		{
			get => (double) GetValue(MinHeightProperty);
			set => SetValue(MinHeightProperty, value);
		}

		private void OnHeightPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			Invalidate();
		}

		private void OnMaxHeightPropertyChangedPrivate(double oldValue, double newValue)
		{
			Invalidate();
		}

		private void OnMinHeightPropertyChangedPrivate(double oldValue, double newValue)
		{
			Invalidate();
		}
	}
}