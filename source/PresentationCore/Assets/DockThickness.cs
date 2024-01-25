// <copyright file="DockThickness.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	public sealed class DockThickness : ThicknessAssetBase
	{
		public static readonly DependencyProperty DockProperty = DPM.Register<Dock?, DockThickness>
			("Dock", null, d => d.OnDockPropertyChangedPrivate);

		public static readonly DependencyProperty ThicknessValueProperty = DPM.Register<double, DockThickness>
			("ThicknessValue", 0.0, d => d.OnThicknessValuePropertyChangedPrivate);

		public Dock? Dock
		{
			get => (Dock?) GetValue(DockProperty);
			set => SetValue(DockProperty, value);
		}

		public double ThicknessValue
		{
			get => (double) GetValue(ThicknessValueProperty);
			set => SetValue(ThicknessValueProperty, value);
		}

		private Thickness CalculateThickness()
		{
			return Dock switch
			{
				System.Windows.Controls.Dock.Left => new Thickness(ThicknessValue, 0, 0, 0),
				System.Windows.Controls.Dock.Top => new Thickness(0, ThicknessValue, 0, 0),
				System.Windows.Controls.Dock.Right => new Thickness(0, 0, ThicknessValue, 0),
				System.Windows.Controls.Dock.Bottom => new Thickness(0, 0, 0, ThicknessValue),
				null => new Thickness(ThicknessValue),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private void OnDockPropertyChangedPrivate(Dock? oldValue, Dock? newValue)
		{
			UpdateActualThickness();
		}

		private void OnThicknessValuePropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateActualThickness();
		}

		private void UpdateActualThickness()
		{
			var thickness = CalculateThickness();

			if (ActualThickness != thickness)
				ActualThickness = thickness;
		}
	}
}