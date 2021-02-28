// <copyright file="GridColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.Core
{
	public class GridColumn : InheritanceContextObject
	{
		internal static readonly FlexLength DefaultWidth = FlexLength.Star;

		internal static readonly FlexLength DefaultMinWidth = new FlexLength(0, FlexLengthUnitType.Pixel);

		internal static readonly FlexLength DefaultMaxWidth = new FlexLength(double.PositiveInfinity, FlexLengthUnitType.Pixel);

		private static readonly DependencyPropertyKey ActualWidthPropertyKey = DPM.RegisterReadOnly<double, GridColumn>
			("ActualWidth", 0.0);

		public static readonly DependencyProperty WidthProperty = DPM.Register<FlexLength, GridColumn>
			("Width", DefaultWidth, d => d.OnWidthPropertyChangedPrivate);

		public static readonly DependencyProperty MinWidthProperty = DPM.Register<FlexLength, GridColumn>
			("MinWidth", DefaultMinWidth, d => d.OnMinWidthPropertyChangedPrivate);

		public static readonly DependencyProperty MaxWidthProperty = DPM.Register<FlexLength, GridColumn>
			("MaxWidth", DefaultMaxWidth, d => d.OnMaxWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;
		private double _autoDesiredWidth;

		public GridColumn()
		{
		}

		internal GridColumn(GridController controller)
		{
			Controller = controller;
		}

		internal GridColumnWidthConstraints ActualColumnWidthConstraints => Controller?.GetColumnWidthConstraintsInternal(this) ?? new GridColumnWidthConstraints(Width, MinWidth, MaxWidth);

		public double ActualWidth
		{
			get => (double) GetValue(ActualWidthProperty);
			internal set => SetValue(ActualWidthPropertyKey, value);
		}

		internal double AutoDesiredWidth
		{
			get => _autoDesiredWidth;
			set
			{
				if (_autoDesiredWidth.IsCloseTo(value))
					return;

				_autoDesiredWidth = value;

				Controller?.OnAutoDesiredWidthChanged(this);
			}
		}

		public virtual GridController Controller { get; }

		internal double FinalWidth { get; set; }

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength MaxWidth
		{
			get => (FlexLength) GetValue(MaxWidthProperty);
			set => SetValue(MaxWidthProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength MinWidth
		{
			get => (FlexLength) GetValue(MinWidthProperty);
			set => SetValue(MinWidthProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength Width
		{
			get => (FlexLength) GetValue(WidthProperty);
			set => SetValue(WidthProperty, value);
		}

		private void OnMaxWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			Controller?.OnColumnWidthConstraintsChanged();
		}

		private void OnMinWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			Controller?.OnColumnWidthConstraintsChanged();
		}

		private void OnWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			Controller?.OnColumnWidthConstraintsChanged();
		}
	}
}