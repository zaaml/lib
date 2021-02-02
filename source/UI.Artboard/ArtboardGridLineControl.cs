// <copyright file="ArtboardGridLineControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[ContentProperty("Model")]
	public sealed class ArtboardGridLineControl : FixedTemplateControl<ArtboardGridLineRendererPanel>, IArtboardComponentControl
	{
		public static readonly DependencyProperty ScrollOffsetXProperty = DPM.Register<double, ArtboardGridLineControl>
			("ScrollOffsetX", 0.0, g => g.OnOffsetXChanged);

		public static readonly DependencyProperty ScrollOffsetYProperty = DPM.Register<double, ArtboardGridLineControl>
			("ScrollOffsetY", 0.0, g => g.OnOffsetYChanged);

		public static readonly DependencyProperty ModelProperty = DPM.Register<ArtboardGridLineModel, ArtboardGridLineControl>
			("Model", null, c => c.OnModelChanged);

		public static readonly DependencyProperty ShowVerticalLinesProperty = DPM.Register<bool, ArtboardGridLineControl>
			("ShowVerticalLines", true, d => d.OnShowVerticalLinesPropertyChangedPrivate);

		public static readonly DependencyProperty ShowHorizontalLinesProperty = DPM.Register<bool, ArtboardGridLineControl>
			("ShowHorizontalLines", true, d => d.OnShowHorizontalLinesPropertyChangedPrivate);

		public static readonly DependencyProperty SyncGridStepProperty = DPM.Register<double, ArtboardGridLineControl>
			("SyncGridStep", default, d => d.OnSyncGridStepPropertyChangedPrivate);

		public static readonly DependencyProperty ZoomProperty = DPM.Register<double, ArtboardGridLineControl>
			("Zoom", 1.0, d => d.OnZoomPropertyChangedPrivate);

		static ArtboardGridLineControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardGridLineControl>();
		}

		public ArtboardGridLineControl()
		{
			this.OverrideStyleKey<ArtboardGridLineControl>();
		}

		private ArtboardControl Artboard { get; set; }

		public ArtboardGridLineModel Model
		{
			get => (ArtboardGridLineModel) GetValue(ModelProperty);
			set => SetValue(ModelProperty, value);
		}

		public double OffsetX
		{
			get => (double) GetValue(ScrollOffsetXProperty);
			set => SetValue(ScrollOffsetXProperty, value);
		}

		public double OffsetY
		{
			get => (double) GetValue(ScrollOffsetYProperty);
			set => SetValue(ScrollOffsetYProperty, value);
		}

		public bool ShowHorizontalLines
		{
			get => (bool) GetValue(ShowHorizontalLinesProperty);
			set => SetValue(ShowHorizontalLinesProperty, value);
		}

		public bool ShowVerticalLines
		{
			get => (bool) GetValue(ShowVerticalLinesProperty);
			set => SetValue(ShowVerticalLinesProperty, value);
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
			TemplateRoot.OffsetX = new GridLineStepLength(OffsetX, GridLineStepUnit.Pixel);
			TemplateRoot.OffsetY = new GridLineStepLength(OffsetY, GridLineStepUnit.Pixel);
			TemplateRoot.VerticalLines = ShowVerticalLines;
			TemplateRoot.HorizontalLines = ShowHorizontalLines;
			TemplateRoot.SyncGridStep = SyncGridStep;
			TemplateRoot.Scale = Zoom;
		}

		private void OnModelChanged()
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.Model = Model;
		}

		private void OnOffsetXChanged()
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.OffsetX = new GridLineStepLength(OffsetX, GridLineStepUnit.Pixel);
		}

		private void OnOffsetYChanged()
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.OffsetY = new GridLineStepLength(OffsetY, GridLineStepUnit.Pixel);
		}

		private void OnShowHorizontalLinesPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.VerticalLines = newValue;
		}

		private void OnShowVerticalLinesPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.HorizontalLines = newValue;
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

			base.UndoTemplateOverride();
		}

		double IArtboardComponentControl.Zoom
		{
			get => Zoom;
			set => Zoom = value;
		}

		double IArtboardComponentControl.ScrollOffsetX
		{
			get => OffsetX;
			set => OffsetX = value;
		}

		double IArtboardComponentControl.ScrollOffsetY
		{
			get => OffsetY;
			set => OffsetY = value;
		}

		ArtboardControl IArtboardComponentControl.Artboard
		{
			get => Artboard;
			set => Artboard = value;
		}
	}

	public sealed class ArtboardGridLineRendererPanel : GridLineRendererPanel<ArtboardGridLineModel, ArtboardGridLineCollection, ArtboardGridLine>
	{
	}
}