// <copyright file="LineDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	public class GridLine : InheritanceContextObject
	{
		public static readonly DependencyProperty StepProperty = DPM.Register<int, GridLine>
			("Step", d => d.OnStepChanged);

		public static readonly DependencyProperty StrokeProperty = DPM.Register<Brush, GridLine>
			("Stroke");

		public static readonly DependencyProperty StrokeThicknessProperty = DPM.Register<double, GridLine>
			("StrokeThickness", 1.0);

		public event EventHandler StepChanged;

		public int Step
		{
			get => (int) GetValue(StepProperty);
			set => SetValue(StepProperty, value);
		}

		public Brush Stroke
		{
			get => (Brush) GetValue(StrokeProperty);
			set => SetValue(StrokeProperty, value);
		}

		public double StrokeThickness
		{
			get => (double) GetValue(StrokeThicknessProperty);
			set => SetValue(StrokeThicknessProperty, value);
		}

		protected virtual void OnStepChanged()
		{
			StepChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}