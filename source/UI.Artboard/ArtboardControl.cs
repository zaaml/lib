// <copyright file="ArtboardControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[ContentProperty("CanvasCollection")]
	[TemplateContractType(typeof(ArtboardControlTemplateContract))]
	public class ArtboardControl : TemplateContractControl
	{
		public static readonly DependencyProperty DesignWidthProperty = DPM.Register<double, ArtboardControl>
			("DesignWidth", 640.0, d => d.OnDesignWidthPropertyChangedPrivate);

		public static readonly DependencyProperty DesignHeightProperty = DPM.Register<double, ArtboardControl>
			("DesignHeight", 480.0, d => d.OnDesignHeightPropertyChangedPrivate);

		public static readonly DependencyProperty DesignTopContentProperty = DPM.Register<object, ArtboardControl>
			("DesignTopContent");

		public static readonly DependencyProperty DesignBottomContentProperty = DPM.Register<object, ArtboardControl>
			("DesignBottomContent");

		public static readonly DependencyProperty DesignTopContentTemplateProperty = DPM.Register<DataTemplate, ArtboardControl>
			("DesignTopContentTemplate");

		public static readonly DependencyProperty DesignBottomContentTemplateProperty = DPM.Register<DataTemplate, ArtboardControl>
			("DesignBottomContentTemplate");

		public static readonly DependencyProperty DesignBackgroundProperty = DPM.Register<Brush, ArtboardControl>
			("DesignBackground");

		public static readonly DependencyProperty DesignBorderBrushProperty = DPM.Register<Brush, ArtboardControl>
			("DesignBorderBrush");

		public static readonly DependencyProperty DesignBorderThicknessProperty = DPM.Register<Thickness, ArtboardControl>
			("DesignBorderThickness");

		public static readonly DependencyProperty ShowGridProperty = DPM.Register<bool, ArtboardControl>
			("ShowGrid", true);

		public static readonly DependencyProperty ZoomProperty = DPM.Register<double, ArtboardControl>
			("Zoom", 1.0, a => a.OnZoomPropertyChangedPrivate);

		private static readonly DependencyPropertyKey CanvasCollectionPropertyKey = DPM.RegisterReadOnly<ArtboardCanvasCollection, ArtboardControl>
			("CanvasCollectionPrivate");

		private static readonly DependencyPropertyKey AdornersPropertyKey = DPM.RegisterAttachedReadOnly<ArtboardAdornerCollection, ArtboardControl>
			("AdornersPrivate");

		private static readonly DependencyPropertyKey AdornerFactoriesPropertyKey = DPM.RegisterReadOnly<ArtboardAdornerFactoryCollection, ArtboardControl>
			("AdornerFactoriesPrivate");

		public static readonly DependencyProperty SnapEngineProperty = DPM.Register<ArtboardSnapEngine, ArtboardControl>
			("SnapEngine", d => d.OnSnapEnginePropertyChangedPrivate);

		private static readonly DependencyPropertyKey SnapGuidesPropertyKey = DPM.RegisterReadOnly<ArtboardSnapGuideCollection, ArtboardControl>
			("SnapGuidesPrivate");

		public static readonly DependencyProperty SnapGuidesProperty = SnapGuidesPropertyKey.DependencyProperty;

		public static readonly DependencyProperty AdornerFactoriesProperty = AdornerFactoriesPropertyKey.DependencyProperty;

		public static readonly DependencyProperty AdornersProperty = AdornersPropertyKey.DependencyProperty;

		public static readonly DependencyProperty CanvasCollectionProperty = CanvasCollectionPropertyKey.DependencyProperty;

		static ArtboardControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardControl>();
		}

		public ArtboardControl()
		{
			this.OverrideStyleKey<ArtboardControl>();
		}

		public ArtboardAdornerFactoryCollection AdornerFactories => this.GetValueOrCreate(AdornerFactoriesPropertyKey, CreateArtboardAdornerFactoryCollection);

		private ArtboardAdornerPresenter AdornerPresenter => TemplateContract.AdornerPresenter;

		internal ArtboardAdornerPresenter AdornerPresenterInternal => AdornerPresenter;

		public ArtboardCanvasCollection CanvasCollection => this.GetValueOrCreate(CanvasCollectionPropertyKey, () => new ArtboardCanvasCollection(this));

		public Brush DesignBackground
		{
			get => (Brush) GetValue(DesignBackgroundProperty);
			set => SetValue(DesignBackgroundProperty, value);
		}

		public Brush DesignBorderBrush
		{
			get => (Brush) GetValue(DesignBorderBrushProperty);
			set => SetValue(DesignBorderBrushProperty, value);
		}

		public Thickness DesignBorderThickness
		{
			get => (Thickness) GetValue(DesignBorderThicknessProperty);
			set => SetValue(DesignBorderThicknessProperty, value);
		}

		public object DesignBottomContent
		{
			get => GetValue(DesignBottomContentProperty);
			set => SetValue(DesignBottomContentProperty, value);
		}

		private ArtboardDesignContentControl DesignBottomContentControl => TemplateContract.DesignBottomContentControl;

		public DataTemplate DesignBottomContentTemplate
		{
			get => (DataTemplate) GetValue(DesignBottomContentTemplateProperty);
			set => SetValue(DesignBottomContentTemplateProperty, value);
		}

		public double DesignHeight
		{
			get => (double) GetValue(DesignHeightProperty);
			set => SetValue(DesignHeightProperty, value);
		}

		public object DesignTopContent
		{
			get => GetValue(DesignTopContentProperty);
			set => SetValue(DesignTopContentProperty, value);
		}

		private ArtboardDesignContentControl DesignTopContentControl => TemplateContract.DesignTopContentControl;

		public DataTemplate DesignTopContentTemplate
		{
			get => (DataTemplate) GetValue(DesignTopContentTemplateProperty);
			set => SetValue(DesignTopContentTemplateProperty, value);
		}

		public double DesignWidth
		{
			get => (double) GetValue(DesignWidthProperty);
			set => SetValue(DesignWidthProperty, value);
		}

		private ArtboardGridLineControl GridLineControl => TemplateContract.GridLineControl;

		internal ArtboardGridLineControl GridLineControlInternal => GridLineControl;

		private ArtboardRuler HorizontalRuler => TemplateContract.HorizontalRuler;

		private ArtboardPresenter Presenter => TemplateContract.Presenter;

		internal double ScrollPanelOffsetX { get; set; }

		internal double ScrollPanelOffsetY { get; set; }

		private ArtboardScrollViewControl ScrollView => TemplateContract.ScrollView;

		internal ArtboardScrollViewControl ScrollViewInternal => ScrollView;

		public bool ShowGrid
		{
			get => (bool) GetValue(ShowGridProperty);
			set => SetValue(ShowGridProperty, value);
		}

		public ArtboardSnapEngine SnapEngine
		{
			get => (ArtboardSnapEngine) GetValue(SnapEngineProperty);
			set => SetValue(SnapEngineProperty, value);
		}

		public ArtboardSnapGuideCollection SnapGuides => this.GetValueOrCreate(SnapGuidesPropertyKey, CreateSnapGuidesCollection);

		private ArtboardControlTemplateContract TemplateContract => (ArtboardControlTemplateContract) TemplateContractInternal;

		private ArtboardRuler VerticalRuler => TemplateContract.VerticalRuler;

		public double Zoom
		{
			get => (double) GetValue(ZoomProperty);
			set => SetValue(ZoomProperty, value);
		}

		internal void ArrangeAdorners(UIElement child, Rect rect)
		{
			var adorners = GetAdornersInternal(child);

			if (adorners == null)
				return;

			foreach (var adorner in adorners)
				adorner.ArrangeAdorner(rect);
		}

		private void AttachSnapEngine(ArtboardSnapEngine snapEngine)
		{
			if (snapEngine.Artboard != null)
				throw new InvalidOperationException("SnapEngine is already attached");

			snapEngine.Artboard = this;
		}

		private ArtboardAdornerFactoryCollection CreateArtboardAdornerFactoryCollection()
		{
			return new ArtboardAdornerFactoryCollection(this);
		}

		private ArtboardSnapGuideCollection CreateSnapGuidesCollection()
		{
			return new ArtboardSnapGuideCollection(this);
		}

		private void DetachSnapEngine(ArtboardSnapEngine snapEngine)
		{
			if (ReferenceEquals(snapEngine.Artboard, this))
				throw new InvalidOperationException();

			snapEngine.Artboard = null;
		}

		public static ArtboardAdornerCollection GetAdorners(UIElement element)
		{
			return element.GetValueOrCreate(AdornersPropertyKey, () => new ArtboardAdornerCollection(element));
		}

		public static ArtboardAdornerCollection GetAdornersInternal(UIElement element)
		{
			return (ArtboardAdornerCollection) element.GetValue(AdornersProperty);
		}

		internal void OnAdornerFactoryAdded(ArtboardAdornerFactory adornerFactory)
		{
			foreach (var canvas in CanvasCollection)
				canvas.OnAdornerFactoryAdded(adornerFactory);
		}

		internal void OnAdornerFactoryRemoved(ArtboardAdornerFactory adornerFactory)
		{
			foreach (var canvas in CanvasCollection)
				canvas.OnAdornerFactoryRemoved(adornerFactory);
		}

		private void OnDesignHeightPropertyChangedPrivate()
		{
			OnDesignSizeChanged();
		}

		private void OnDesignSizeChanged()
		{
			UpdateDesignSize();
		}

		private void OnDesignWidthPropertyChangedPrivate()
		{
			OnDesignSizeChanged();
		}

		private void OnSnapEnginePropertyChangedPrivate(ArtboardSnapEngine oldValue, ArtboardSnapEngine newValue)
		{
			if (oldValue != null)
				DetachSnapEngine(oldValue);

			if (newValue != null)
				AttachSnapEngine(newValue);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ScrollView.Artboard = this;
			Presenter.Artboard = this;

			UpdateZoom();
			UpdateOffset();
			UpdateDesignSize();
		}

		protected override void OnTemplateContractDetaching()
		{
			ScrollView.Artboard = null;
			Presenter.Artboard = null;

			base.OnTemplateContractDetaching();
		}

		private void OnZoomPropertyChangedPrivate(double oldZoom, double newZoom)
		{
			UpdateZoom();
		}

		private void UpdateDesignSize()
		{
			if (IsTemplateAttached == false)
				return;

			var designWidth = DesignWidth;
			var designHeight = DesignHeight;

			Presenter.DesignWidth = designWidth;
			Presenter.DesignHeight = designHeight;

			AdornerPresenter.DesignWidth = designWidth;
			AdornerPresenter.DesignHeight = designHeight;

			DesignTopContentControl.DesignWidth = designWidth;
			DesignTopContentControl.DesignHeight = designHeight;

			DesignBottomContentControl.DesignWidth = designWidth;
			DesignBottomContentControl.DesignHeight = designHeight;

			ScrollView.OnDesignSizeChangedInternal();
		}

		private void UpdateOffset()
		{
			if (IsTemplateAttached == false)
				return;

			var offsetX = ScrollPanelOffsetX;
			var offsetY = ScrollPanelOffsetY;

			HorizontalRuler.Offset = offsetX;
			VerticalRuler.Offset = offsetY;

			GridLineControl.OffsetX = offsetX;
			GridLineControl.OffsetY = offsetY;

			AdornerPresenter.OffsetX = offsetX;
			AdornerPresenter.OffsetY = offsetY;

			DesignTopContentControl.OffsetX = offsetX;
			DesignTopContentControl.OffsetY = offsetY;

			DesignBottomContentControl.OffsetX = offsetX;
			DesignBottomContentControl.OffsetY = offsetY;
		}

		internal void UpdateScrollPanelOffset(double offsetX, double offsetY)
		{
			ScrollPanelOffsetX = offsetX;
			ScrollPanelOffsetY = offsetY;

			UpdateOffset();
		}

		private void UpdateZoom()
		{
			if (IsTemplateAttached == false)
				return;

			var zoom = Zoom;

			Presenter.Zoom = zoom;
			GridLineControl.Zoom = zoom;
			VerticalRuler.Zoom = zoom;
			HorizontalRuler.Zoom = zoom;
			AdornerPresenter.Zoom = zoom;
			DesignTopContentControl.Zoom = zoom;
			DesignBottomContentControl.Zoom = zoom;
		}
	}
}