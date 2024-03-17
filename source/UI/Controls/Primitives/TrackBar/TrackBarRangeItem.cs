// <copyright file="TrackBarRangeItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
	public class TrackBarRangeItem : TrackBarItem
	{
		private static readonly DependencyPropertyKey ActualCornerRadiusPropertyKey = DPM.RegisterReadOnly<CornerRadius, TrackBarRangeItem>
			("ActualCornerRadius");

		public static readonly DependencyProperty ActualCornerRadiusProperty = ActualCornerRadiusPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey RangePropertyKey = DPM.RegisterReadOnly<double, TrackBarRangeItem>
			("Range", t => t.OnRangeChangedPrivate);

		public static readonly DependencyProperty RangeProperty = RangePropertyKey.DependencyProperty;

		public event EventHandler<ValueChangedEventArgs<double>> RangeChanged;

		static TrackBarRangeItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TrackBarRangeItem>();
		}

		public TrackBarRangeItem()
		{
			this.OverrideStyleKey<TrackBarRangeItem>();
		}

		public CornerRadius ActualCornerRadius
		{
			get => this.GetReadOnlyValue<CornerRadius>(ActualCornerRadiusPropertyKey);
			internal set => this.SetReadOnlyValue(ActualCornerRadiusPropertyKey, value);
		}

		public double Range
		{
			get => this.GetReadOnlyValue<double>(RangePropertyKey);
			internal set => this.SetReadOnlyValue(RangePropertyKey, value);
		}

		private protected override void ClampCore()
		{
		}

		protected virtual void OnRangeChanged(double oldRange, double newRange)
		{
		}

		private void OnRangeChangedPrivate(double oldRange, double newRange)
		{
			OnRangeChanged(oldRange, newRange);

			RangeChanged?.Invoke(this, new ValueChangedEventArgs<double>(oldRange, newRange));
		}
	}
}