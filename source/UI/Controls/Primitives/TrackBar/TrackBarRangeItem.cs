// <copyright file="TrackBarRangeItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
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
			("Range");

		public static readonly DependencyProperty RangeProperty = RangePropertyKey.DependencyProperty;

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

		protected override void ClampCore()
		{
		}
	}
}