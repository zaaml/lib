// <copyright file="GridPanelColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Panels.Primitives
{
	public class GridPanelColumn : GridPanelDefinition
	{
		public static readonly DependencyProperty MaxWidthProperty = DPM.Register<double, GridPanelColumn>
			("MaxWidth", double.PositiveInfinity, d => d.OnMaxWidthPropertyChangedPrivate);

		public static readonly DependencyProperty MinWidthProperty = DPM.Register<double, GridPanelColumn>
			("MinWidth", 0.0, d => d.OnMinWidthPropertyChangedPrivate);

		public static readonly DependencyProperty WidthProperty = DPM.Register<FlexLength, GridPanelColumn>
			("Width", FlexLength.Star, d => d.OnWidthPropertyChangedPrivate);

		public GridPanelColumn() : base(true)
		{
		}

		public double ActualWidth => GridPanel?.GetFinalColumnDefinitionWidth(Index) ?? 0d;

		public double MaxWidth
		{
			get => (double) GetValue(MaxWidthProperty);
			set => SetValue(MaxWidthProperty, value);
		}

		public double MinWidth
		{
			get => (double) GetValue(MinWidthProperty);
			set => SetValue(MinWidthProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength Width
		{
			get => (FlexLength) GetValue(WidthProperty);
			set => SetValue(WidthProperty, value);
		}

		private void OnMaxWidthPropertyChangedPrivate(double oldValue, double newValue)
		{
			Invalidate();
		}

		private void OnMinWidthPropertyChangedPrivate(double oldValue, double newValue)
		{
			Invalidate();
		}

		private void OnWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			Invalidate();
		}
	}
}