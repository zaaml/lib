// <copyright file="ArtboardSnapGuidePresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSnapGuidePresenter : ArtboardControlBase<ArtboardSnapGuidePanel>
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ArtboardSnapGuidePresenter>
			("Orientation", default, d => d.OnOrientationPropertyChangedPrivate);

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private void OnOrientationPropertyChangedPrivate(Orientation oldValue, Orientation newValue)
		{
		}
	}
}