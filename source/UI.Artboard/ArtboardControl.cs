// <copyright file="ArtboardControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[ContentProperty("ItemCollection")]
	[TemplateContractType(typeof(ArtboardControlTemplateContract))]
	public class ArtboardControl : ItemsControlBase<ArtboardControl, ArtboardItem, ArtboardItemCollection, ArtboardItemsPresenter, ArtboardItemsPanel>
	{
		public static readonly DependencyProperty ShowGridProperty = DPM.Register<bool, ArtboardControl>
			("ShowGrid", true);

		public static readonly DependencyProperty ZoomProperty = DPM.Register<double, ArtboardControl>
			("Zoom", 1.0, a => a.OnZoomPropertyChangedPrivate);

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

		private protected readonly CompositeTransform ScrollViewTransform = new CompositeTransform
		{
			ScaleX = 1.0,
			ScaleY = 1.0
		};

		private bool _isRulerMouseOver;
		private bool _isSnapGuideMouseOver;
		private ArtboardSnapGuide _mouseSnapGuide;
		private double _mouseSnapGuideOriginLocation;
		private bool _movingSnapGuide;
		private Point _rulerOriginLocation;

		static ArtboardControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardControl>();
		}

		public ArtboardControl()
		{
			this.OverrideStyleKey<ArtboardControl>();

			MouseSnapGuide = PreviewSnapGuide;
		}

		public ArtboardAdornerFactoryCollection AdornerFactories => this.GetValueOrCreate(AdornerFactoriesPropertyKey, CreateArtboardAdornerFactoryCollection);

		private ArtboardAdornerPresenter AdornerPresenter => TemplateContract.AdornerPresenter;

		internal ArtboardAdornerPresenter AdornerPresenterInternal => AdornerPresenter;

		private IEnumerable<IArtboardComponentControl> Components
		{
			get
			{
				yield return ItemsPresenter;
				yield return GridLineControl;
				yield return VerticalRuler;
				yield return HorizontalRuler;
				yield return AdornerPresenter;
				yield return VerticalSnapGuidePresenter;
				yield return HorizontalSnapGuidePresenter;

				foreach (var artboardItem in ItemCollection)
					yield return artboardItem;
			}
		}

		protected Matrix FromDesignMatrix => ScrollViewTransform.Transform.Value;

		private ArtboardGridLineControl GridLineControl => TemplateContract.GridLineControl;

		internal ArtboardGridLineControl GridLineControlInternal => GridLineControl;

		private ArtboardRuler HorizontalRuler => TemplateContract.HorizontalRuler;

		private ArtboardSnapGuidePresenter HorizontalSnapGuidePresenter => TemplateContract.HorizontalSnapGuidePresenter;

		private bool IsRulerMouseOver
		{
			get => _isRulerMouseOver;
			set
			{
				if (_isRulerMouseOver == value)
					return;

				_isRulerMouseOver = value;

				UpdateSnapGuidePresenterHitTestVisibility();
			}
		}

		private bool IsSnapGuideMouseOver
		{
			get => _isSnapGuideMouseOver;
			set
			{
				if (_isSnapGuideMouseOver == value)
					return;

				_isSnapGuideMouseOver = value;

				UpdateSnapGuidePresenterHitTestVisibility();
			}
		}

		private ArtboardSnapGuide MouseSnapGuide
		{
			get => _mouseSnapGuide;
			set
			{
				if (ReferenceEquals(_mouseSnapGuide, value))
					return;

				_mouseSnapGuide = value;

				PreviewSnapGuide.Visibility = ReferenceEquals(_mouseSnapGuide, PreviewSnapGuide) == false ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		private ArtboardSnapGuide PreviewSnapGuide { get; } = new ArtboardSnapGuide {IsHitTestVisible = false};

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

		protected override ArtboardItemCollection CreateItemCollection()
		{
			return new ArtboardItemCollection(this);
		}

		protected Matrix ToDesignMatrix
		{
			get
			{
				var matrix = ScrollViewTransform.Transform.Value;

				matrix.Invert();

				return matrix;
			}
		}

		private ArtboardRuler VerticalRuler => TemplateContract.VerticalRuler;

		private ArtboardSnapGuidePresenter VerticalSnapGuidePresenter => TemplateContract.VerticalSnapGuidePresenter;

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

		private ArtboardSnapGuidePresenter GetTargetSnapGuidePresenter(ArtboardSnapGuide snapGuide)
		{
			return snapGuide.Orientation switch
			{
				Orientation.Horizontal => HorizontalSnapGuidePresenter,
				Orientation.Vertical => VerticalSnapGuidePresenter,
				_ => throw new InvalidOperationException()
			};
		}

		private void MoveSnapGuide(ArtboardSnapGuide snapGuide, MouseEventArgs e)
		{
			MouseSnapGuide = snapGuide;

			var ruler = MouseSnapGuide.Orientation == Orientation.Horizontal ? VerticalRuler : HorizontalRuler;

			_mouseSnapGuideOriginLocation = MouseSnapGuide.Location;
			_rulerOriginLocation = e.GetPosition(ruler);

			var captured = ruler.CaptureMouse();

			if (captured == false)
				return;

			_movingSnapGuide = true;
		}

		internal void OnAdornerFactoryAdded(ArtboardAdornerFactory adornerFactory)
		{
			foreach (var artboardItem in ItemCollection)
				artboardItem.OnAdornerFactoryAdded(adornerFactory);
		}

		internal void OnAdornerFactoryRemoved(ArtboardAdornerFactory adornerFactory)
		{
			foreach (var artboardItem in ItemCollection)
				artboardItem.OnAdornerFactoryRemoved(adornerFactory);
		}

		private void OnRulerGotMouseCapture(object sender, MouseEventArgs e)
		{
			var ruler = (ArtboardRuler) sender;

			ruler.Cursor = MouseSnapGuide.Cursor;
		}

		private void OnRulerLostMouseCapture(object sender, MouseEventArgs e)
		{
			var ruler = (ArtboardRuler) sender;

			ruler.Cursor = Cursors.Arrow;

			_movingSnapGuide = false;
		}

		private void OnRulerMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
				MoveSnapGuide(PreviewSnapGuide, e);
		}

		private void OnRulerMouseEnter(object sender, MouseEventArgs e)
		{
			var ruler = (ArtboardRuler) sender;

			PreviewSnapGuide.Orientation = ruler.Orientation.Rotate();
			GetTargetSnapGuidePresenter(PreviewSnapGuide).SnapGuides.Add(PreviewSnapGuide);

			IsRulerMouseOver = true;
		}

		private void OnRulerMouseLeave(object sender, MouseEventArgs e)
		{
			GetTargetSnapGuidePresenter(PreviewSnapGuide).SnapGuides.Remove(PreviewSnapGuide);

			IsRulerMouseOver = false;
		}

		private void OnRulerMouseMove(object sender, MouseEventArgs e)
		{
			var ruler = (ArtboardRuler) sender;
			var designMatrix = ToDesignMatrix;
			var rulerPosition = e.GetPosition(ruler);
			var designPosition = designMatrix.Transform(rulerPosition);

			if (ReferenceEquals(MouseSnapGuide, PreviewSnapGuide))
				MouseSnapGuide.Location = ruler.Orientation == Orientation.Horizontal ? designPosition.X : designPosition.Y;
			else
			{
				var designOriginPosition = designMatrix.Transform(_rulerOriginLocation);

				if (ruler.Orientation == Orientation.Horizontal)
					MouseSnapGuide.Location = _mouseSnapGuideOriginLocation + designPosition.X - designOriginPosition.X;
				else
					MouseSnapGuide.Location = _mouseSnapGuideOriginLocation + designPosition.Y - designOriginPosition.Y;
			}
		}

		private void OnRulerMouseUp(object sender, MouseButtonEventArgs e)
		{
			var ruler = (ArtboardRuler) sender;

			if (_movingSnapGuide == false)
				return;

			_movingSnapGuide = false;

			if (ReferenceEquals(MouseSnapGuide, PreviewSnapGuide))
			{
				var snapGuide = new ArtboardSnapGuide
				{
					Orientation = PreviewSnapGuide.Orientation,
					Location = PreviewSnapGuide.Location
				};

				SnapGuides.Add(snapGuide);
			}

			MouseSnapGuide = PreviewSnapGuide;

			ruler.ReleaseMouseCapture();
		}

		private void OnSnapEnginePropertyChangedPrivate(ArtboardSnapEngine oldValue, ArtboardSnapEngine newValue)
		{
			if (oldValue != null)
				DetachSnapEngine(oldValue);

			if (newValue != null)
				AttachSnapEngine(newValue);
		}

		internal void OnSnapGuideAdded(ArtboardSnapGuide snapGuide)
		{
			snapGuide.Artboard = this;

			if (IsTemplateAttached == false)
				return;

			GetTargetSnapGuidePresenter(snapGuide).SnapGuides.Add(snapGuide);
		}

		internal void OnSnapGuideMouseDown(ArtboardSnapGuide artboardSnapGuide, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			if (e.ChangedButton != MouseButton.Left)
				return;

			MoveSnapGuide(artboardSnapGuide, e);

			e.Handled = true;
		}

		internal void OnSnapGuideMouseEnter(ArtboardSnapGuide artboardSnapGuide, MouseEventArgs e)
		{
			IsSnapGuideMouseOver = true;
		}

		internal void OnSnapGuideMouseLeave(ArtboardSnapGuide artboardSnapGuide, MouseEventArgs e)
		{
			IsSnapGuideMouseOver = false;
		}

		internal void OnSnapGuideMouseUp(ArtboardSnapGuide artboardSnapGuide, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			if (e.ChangedButton != MouseButton.Left)
				return;

			e.Handled = true;
		}

		internal void OnSnapGuideOrientationChanged(ArtboardSnapGuide snapGuide)
		{
			if (IsTemplateAttached == false)
				return;

			if (snapGuide.Orientation == Orientation.Horizontal)
			{
				VerticalSnapGuidePresenter.SnapGuides.Remove(snapGuide);
				HorizontalSnapGuidePresenter.SnapGuides.Add(snapGuide);
			}
			else
			{
				HorizontalSnapGuidePresenter.SnapGuides.Remove(snapGuide);
				VerticalSnapGuidePresenter.SnapGuides.Add(snapGuide);
			}
		}

		internal void OnSnapGuideRemoved(ArtboardSnapGuide snapGuide)
		{
			if (IsTemplateAttached == false)
				return;

			GetTargetSnapGuidePresenter(snapGuide).SnapGuides.Remove(snapGuide);

			snapGuide.Artboard = null;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ScrollView.Artboard = this;

			foreach (var component in Components)
				component.Artboard = this;

			foreach (var snapGuide in SnapGuides)
			{
				if (snapGuide.Orientation == Orientation.Horizontal)
					HorizontalSnapGuidePresenter.SnapGuides.Add(snapGuide);
				else
					VerticalSnapGuidePresenter.SnapGuides.Add(snapGuide);
			}

			VerticalRuler.MouseEnter += OnRulerMouseEnter;
			VerticalRuler.MouseLeave += OnRulerMouseLeave;
			VerticalRuler.MouseMove += OnRulerMouseMove;
			VerticalRuler.MouseLeftButtonDown += OnRulerMouseDown;
			VerticalRuler.MouseLeftButtonUp += OnRulerMouseUp;
			VerticalRuler.GotMouseCapture += OnRulerGotMouseCapture;
			VerticalRuler.LostMouseCapture += OnRulerLostMouseCapture;

			HorizontalRuler.MouseEnter += OnRulerMouseEnter;
			HorizontalRuler.MouseLeave += OnRulerMouseLeave;
			HorizontalRuler.MouseMove += OnRulerMouseMove;
			HorizontalRuler.MouseLeftButtonDown += OnRulerMouseDown;
			HorizontalRuler.MouseLeftButtonUp += OnRulerMouseUp;
			HorizontalRuler.GotMouseCapture += OnRulerGotMouseCapture;
			HorizontalRuler.LostMouseCapture += OnRulerLostMouseCapture;

			UpdateZoom();
			UpdateOffset();
			UpdateSnapGuidePresenterHitTestVisibility();
		}

		protected override void OnTemplateContractDetaching()
		{
			VerticalRuler.MouseEnter -= OnRulerMouseEnter;
			VerticalRuler.MouseLeave -= OnRulerMouseLeave;
			VerticalRuler.MouseMove -= OnRulerMouseMove;
			VerticalRuler.MouseLeftButtonDown -= OnRulerMouseDown;
			VerticalRuler.MouseLeftButtonUp -= OnRulerMouseUp;
			VerticalRuler.GotMouseCapture -= OnRulerGotMouseCapture;
			VerticalRuler.LostMouseCapture -= OnRulerLostMouseCapture;

			HorizontalRuler.MouseEnter -= OnRulerMouseEnter;
			HorizontalRuler.MouseLeave -= OnRulerMouseLeave;
			HorizontalRuler.MouseMove -= OnRulerMouseMove;
			HorizontalRuler.MouseLeftButtonDown -= OnRulerMouseDown;
			HorizontalRuler.MouseLeftButtonUp -= OnRulerMouseUp;
			HorizontalRuler.GotMouseCapture -= OnRulerGotMouseCapture;
			HorizontalRuler.LostMouseCapture -= OnRulerLostMouseCapture;

			HorizontalSnapGuidePresenter.SnapGuides.Clear();
			VerticalSnapGuidePresenter.SnapGuides.Clear();

			ScrollView.Artboard = null;

			foreach (var component in Components)
				component.Artboard = this;

			base.OnTemplateContractDetaching();
		}

		private void OnZoomPropertyChangedPrivate(double oldZoom, double newZoom)
		{
			UpdateZoom();
		}

		private void UpdateOffset()
		{
			var offsetX = ScrollPanelOffsetX;
			var offsetY = ScrollPanelOffsetY;

			ScrollViewTransform.TranslateX = -offsetX;
			ScrollViewTransform.TranslateY = -offsetY;

			if (IsTemplateAttached == false)
				return;

			foreach (var component in Components)
			{
				component.ScrollOffsetX = offsetX;
				component.ScrollOffsetY = offsetY;
			}
		}

		internal void UpdateScrollPanelOffset(double offsetX, double offsetY)
		{
			ScrollPanelOffsetX = offsetX;
			ScrollPanelOffsetY = offsetY;

			UpdateOffset();
		}

		private void UpdateSnapGuidePresenterHitTestVisibility()
		{
			HorizontalSnapGuidePresenter.IsHitTestVisible = VerticalSnapGuidePresenter.IsHitTestVisible = IsRulerMouseOver || IsSnapGuideMouseOver;
		}

		private void UpdateZoom()
		{
			var zoom = Zoom;

			ScrollViewTransform.ScaleX = ScrollViewTransform.ScaleY = zoom;

			if (IsTemplateAttached == false)
				return;

			foreach (var component in Components)
				component.Zoom = zoom;
		}
	}
}