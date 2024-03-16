// <copyright file="TickBarSubDivision.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	public sealed class TickBarSubDivision : InheritanceContextObject
	{
		public static readonly DependencyProperty DrawingProperty = DPM.Register<Drawing, TickBarSubDivision>
			("Drawing", d => d.OnDrawingPropertyChangedPrivate);

		public static readonly DependencyProperty CountProperty = DPM.Register<int, TickBarSubDivision>
			("Count", 1, d => d.OnCountPropertyChangedPrivate, _ => CoerceCount);

		private static readonly DependencyPropertyKey TickBarControlPropertyKey = DPM.RegisterReadOnly<TickBarControl, TickBarSubDivision>
			("TickBarControl");

		public static readonly DependencyProperty ThresholdLengthProperty = DPM.Register<double, TickBarSubDivision>
			("ThresholdLength", 0.0, d => d.OnThresholdLengthPropertyChangedPrivate);

		public static readonly DependencyProperty TickBarControlProperty = TickBarControlPropertyKey.DependencyProperty;
		private DrawingBrush _brush;

		internal DrawingBrush Brush => _brush ??= new DrawingBrush(Drawing);

		public int Count
		{
			get => (int)GetValue(CountProperty);
			set => SetValue(CountProperty, value);
		}

		public Drawing Drawing
		{
			get => (Drawing)GetValue(DrawingProperty);
			set => SetValue(DrawingProperty, value);
		}

		public double ThresholdLength
		{
			get => (double)GetValue(ThresholdLengthProperty);
			set => SetValue(ThresholdLengthProperty, value);
		}

		public TickBarControl TickBarControl
		{
			get => (TickBarControl)GetValue(TickBarControlProperty);
			internal set => this.SetReadOnlyValue(TickBarControlPropertyKey, value);
		}

		private static int CoerceCount(int value)
		{
			return value < 0 ? 0 : value;
		}

		private void InvalidateSubDivisions()
		{
			_brush = null;

			TickBarControl?.InvalidateDivisionsInternal();
		}

		private void OnCountPropertyChangedPrivate(int oldValue, int newValue)
		{
			InvalidateSubDivisions();
		}

		private void OnDrawingPropertyChangedPrivate(Drawing oldValue, Drawing newValue)
		{
			InvalidateSubDivisions();
		}

		private void OnThresholdLengthPropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidateSubDivisions();
		}
	}
}