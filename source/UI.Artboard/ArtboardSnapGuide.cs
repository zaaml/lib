// <copyright file="ArtboardSnapGuide.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Control = Zaaml.UI.Controls.Core.Control;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSnapGuide : Control
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ArtboardSnapGuide>
			("Orientation", default, d => d.OnOrientationPropertyChangedPrivate);

		public static readonly DependencyProperty LocationProperty = DPM.Register<double, ArtboardSnapGuide>
			("Location", default, d => d.OnLocationPropertyChangedPrivate);

		public double Location
		{
			get => (double) GetValue(LocationProperty);
			set => SetValue(LocationProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private void OnLocationPropertyChangedPrivate(double oldValue, double newValue)
		{
		}

		private void OnOrientationPropertyChangedPrivate(Orientation oldValue, Orientation newValue)
		{
		}
	}
}