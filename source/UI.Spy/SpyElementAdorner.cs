// <copyright file="SpyElementAdorner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyElementAdorner : FixedTemplateControl<Border>
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<UIElement, SpyElementAdorner>
			("Element", d => d.OnElementPropertyChangedPrivate);

		private readonly CompositionRenderingObserver _renderingObserver;
		private readonly TranslateTransform _renderTransform = new();

		private OverlayContentControl _overlayContentControl;

		public SpyElementAdorner()
		{
			_renderingObserver = new CompositionRenderingObserver(UpdatePlacement);

			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;
			IsHitTestVisible = false;

			RenderTransform = _renderTransform;

			ItemPresenter = new OverlayItemPresenter
			{
				IsHitTestVisible = false,
				Content = this
			};
		}

		public UIElement Element
		{
			get => (UIElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		private OverlayItemPresenter ItemPresenter { get; }

		private OverlayContentControl OverlayContentControl
		{
			get => _overlayContentControl;
			set
			{
				if (ReferenceEquals(_overlayContentControl, value))
					return;

				_overlayContentControl?.RemoveItem(ItemPresenter);

				_overlayContentControl = value;

				_overlayContentControl?.AddItem(ItemPresenter);

				UpdatePlacement();
			}
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Background = new SolidColorBrush(Colors.Red) { Opacity = 0.1 };
			TemplateRoot.BorderBrush = Brushes.Red;
			TemplateRoot.BorderThickness = new Thickness(1);
			TemplateRoot.IsHitTestVisible = false;
		}

		private void OnElementPropertyChangedPrivate(UIElement oldValue, UIElement newValue)
		{
			if (oldValue is FrameworkElement oldFrameworkElement)
				OverlayContentControl = null;

			if (newValue is FrameworkElement newFrameworkElement)
				OverlayContentControl = OverlayContentControl.GetOverlay(newFrameworkElement);
		}

		private void UpdatePlacement()
		{
			var element = Element;

			if (element == null || OverlayContentControl == null)
				return;

			if (ReferenceEquals(element, OverlayContentControl) == false && element.IsVisualDescendantOf(OverlayContentControl) == false)
				return;

			var transform = element.TransformToAncestor(OverlayContentControl);
			var position = transform.Transform(new Point(0, 0));

			_renderTransform.X = position.X;
			_renderTransform.Y = position.Y;

			Width = element.RenderSize.Width;
			Height = element.RenderSize.Height;
		}
	}
}