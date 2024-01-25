// <copyright file="GridViewColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.Core.GridView
{
	public class GridViewColumn : InheritanceContextObject
	{
		internal static readonly FlexLength DefaultWidth = FlexLength.Star;

		internal static readonly FlexLength DefaultMinWidth = new(0, FlexLengthUnitType.Pixel);

		internal static readonly FlexLength DefaultMaxWidth = new(double.PositiveInfinity, FlexLengthUnitType.Pixel);

		private static readonly DependencyPropertyKey ActualWidthPropertyKey = DPM.RegisterReadOnly<double, GridViewColumn>
			("ActualWidth", 0.0);

		public static readonly DependencyProperty WidthProperty = DPM.Register<FlexLength, GridViewColumn>
			("Width", DefaultWidth, d => d.OnWidthPropertyChangedPrivate);

		public static readonly DependencyProperty MinWidthProperty = DPM.Register<FlexLength, GridViewColumn>
			("MinWidth", DefaultMinWidth, d => d.OnMinWidthPropertyChangedPrivate);

		public static readonly DependencyProperty MaxWidthProperty = DPM.Register<FlexLength, GridViewColumn>
			("MaxWidth", DefaultMaxWidth, d => d.OnMaxWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;
		private double _autoDesiredWidth;

		public GridViewColumn()
		{
		}

		internal GridViewColumn(GridViewColumnController columnController)
		{
			ColumnController = columnController;
		}

		internal GridViewColumnWidthConstraints ActualColumnWidthConstraints => ColumnController?.GetColumnWidthConstraintsInternal(this) ?? new GridViewColumnWidthConstraints(Width, MinWidth, MaxWidth);

		public double ActualWidth
		{
			get => (double)GetValue(ActualWidthProperty);
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

				ColumnController?.OnAutoDesiredWidthChanged(this);
			}
		}

		public virtual GridViewColumnController ColumnController { get; }

		internal double FinalWidth { get; set; }

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength MaxWidth
		{
			get => (FlexLength)GetValue(MaxWidthProperty);
			set => SetValue(MaxWidthProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength MinWidth
		{
			get => (FlexLength)GetValue(MinWidthProperty);
			set => SetValue(MinWidthProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength Width
		{
			get => (FlexLength)GetValue(WidthProperty);
			set => SetValue(WidthProperty, value);
		}

		internal void AttachCellInternal(GridViewCell cell)
		{
			OnCellAttachedCore(cell);
		}

		internal void DetachCellInternal(GridViewCell cell)
		{
			OnCellDetachedCore(cell);
		}

		private protected virtual void OnCellAttachedCore(GridViewCell cellCore)
		{
		}

		private protected virtual void OnCellDetachedCore(GridViewCell cellCore)
		{
		}

		private void OnMaxWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			ColumnController?.OnColumnWidthConstraintsChanged();
		}

		private void OnMinWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			ColumnController?.OnColumnWidthConstraintsChanged();
		}

		private void OnWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			ColumnController?.OnColumnWidthConstraintsChanged();
		}
	}
}