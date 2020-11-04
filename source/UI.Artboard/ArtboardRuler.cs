// <copyright file="ArtboardRuler.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[ContentProperty("Model")]
	public class ArtboardRuler : FixedTemplateControl<ArtboardRulerPanel>
	{
		public static readonly DependencyProperty OffsetProperty = DPM.Register<double, ArtboardRuler>
			("Offset", r => r.OnOffsetChanged);

		public static readonly DependencyProperty ModelProperty = DPM.Register<ArtboardRulerModel, ArtboardRuler>
			("Model", r => r.OnModelChanged);

		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ArtboardRuler>
			("Orientation", Orientation.Horizontal, r => r.OnOrientationChanged);

		public static readonly DependencyProperty SyncGridStepProperty = DPM.Register<double, ArtboardRuler>
			("SyncGridStep", default, d => d.OnSyncGridStepPropertyChangedPrivate);

		public static readonly DependencyProperty ZoomProperty = DPM.Register<double, ArtboardRuler>
			("Zoom", 1.0, d => d.OnZoomPropertyChangedPrivate);

		static ArtboardRuler()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardRuler>();
		}

		public ArtboardRuler()
		{
			this.OverrideStyleKey<ArtboardRuler>();
		}

		public ArtboardRulerModel Model
		{
			get => (ArtboardRulerModel) GetValue(ModelProperty);
			set => SetValue(ModelProperty, value);
		}

		public double Offset
		{
			get => (double) GetValue(OffsetProperty);
			set => SetValue(OffsetProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public double SyncGridStep
		{
			get => (double) GetValue(SyncGridStepProperty);
			set => SetValue(SyncGridStepProperty, value);
		}

		public double Zoom
		{
			get => (double) GetValue(ZoomProperty);
			set => SetValue(ZoomProperty, value);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Model = Model;
			TemplateRoot.SyncGridStep = SyncGridStep;
			TemplateRoot.Scale = Zoom;

			UpdateOffset();
			UpdateOrientation();

			TemplateRoot.ArtboardRuler = this;
		}

		private void OnModelChanged()
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.Model = Model;
		}

		private void OnOffsetChanged()
		{
			UpdateOffset();
		}

		private void OnOrientationChanged()
		{
			UpdateOrientation();
		}

		private void OnSyncGridStepPropertyChangedPrivate(double oldValue, double newValue)
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.SyncGridStep = newValue;
		}

		private void OnZoomPropertyChangedPrivate(double oldValue, double newValue)
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.Scale = newValue;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Model = null;
			TemplateRoot.ArtboardRuler = null;

			base.UndoTemplateOverride();
		}

		private void UpdateOffset()
		{
			if (TemplateRoot == null)
				return;

			if (Orientation == Orientation.Horizontal)
				TemplateRoot.OffsetX = new GridLineStepLength(Offset, GridLineStepUnit.Pixel);
			else
				TemplateRoot.OffsetY = new GridLineStepLength(Offset, GridLineStepUnit.Pixel);
		}

		private void UpdateOrientation()
		{
			if (TemplateRoot == null)
				return;

			var orientation = Orientation;

			TemplateRoot.VerticalLines = orientation == Orientation.Horizontal;
			TemplateRoot.HorizontalLines = orientation == Orientation.Vertical;
			TemplateRoot.OnOrientationChanged();
		}
	}
}