// <copyright file="ArtboardSnapGuide.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Control = Zaaml.UI.Controls.Core.Control;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSnapGuide : Control
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ArtboardSnapGuide>
			("Orientation", default, d => d.OnOrientationPropertyChangedPrivate);

		public static readonly DependencyProperty LocationProperty = DPM.Register<double, ArtboardSnapGuide>
			("Location", default, d => d.OnLocationPropertyChangedPrivate);

		static ArtboardSnapGuide()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardSnapGuide>();
		}

		public ArtboardSnapGuide()
		{
			this.OverrideStyleKey<ArtboardSnapGuide>();

			

			UpdateCursor();
		}

		internal ArtboardControl Artboard { get; set; }

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
			(this.GetVisualParent() as ArtboardSnapGuidePanel)?.InvalidateArrange();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			Artboard?.OnSnapGuideMouseEnter(this, e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			Artboard?.OnSnapGuideMouseLeave(this, e);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			Artboard?.OnSnapGuideMouseDown(this, e);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			Artboard?.OnSnapGuideMouseUp(this, e);
		}

		private void OnOrientationPropertyChangedPrivate(Orientation oldValue, Orientation newValue)
		{
			Artboard?.OnSnapGuideOrientationChanged(this);

			UpdateCursor();
		}

		private void UpdateCursor()
		{
			Cursor = Orientation == Orientation.Horizontal ? Cursors.SizeNS : Cursors.SizeWE;
		}
	}
}