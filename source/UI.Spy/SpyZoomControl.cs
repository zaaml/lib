// <copyright file="SpyZoomControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Artboard;
using Zaaml.UI.Controls.Core;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Spy
{
	[TemplateContractType(typeof(SpyZoomControlTemplateContract))]
	public class SpyZoomControl : TemplateContractControl
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<UIElement, SpyZoomControl>
			("Element", d => d.OnElementPropertyChangedPrivate);

		private Window _elementWindow;
		private bool _isElementMouseOver;

		static SpyZoomControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<SpyZoomControl>();
		}

		public SpyZoomControl()
		{
			this.OverrideStyleKey<SpyZoomControl>();

			Renderer = new ElementRenderer(this);

			Canvas = new ArtboardCanvas
			{
				Children =
				{
					Renderer, SizeRenderer
				}
			};

			ArtboardItem = new ArtboardItem
			{
				Canvas = Canvas
			};
		}

		private SpyArtboardControl ArtboardControl => TemplateContract.ArtboardControl;

		private ArtboardItem ArtboardItem { get; }

		private ArtboardCanvas Canvas { get; }

		public UIElement Element
		{
			get => (UIElement)GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		private Rect ElementBox { get; set; }

		private Window ElementWindow
		{
			get => _elementWindow;
			set
			{
				if (ReferenceEquals(_elementWindow, value))
					return;

				if (_elementWindow != null)
				{
					_elementWindow.SizeChanged -= ElementWindowOnSizeChanged;

					Renderer.Background = null;
				}

				_elementWindow = value;

				if (_elementWindow != null)
				{
					_elementWindow.SizeChanged += ElementWindowOnSizeChanged;

					Renderer.Background = new VisualBrush(_elementWindow);
				}

				UpdateArtboardSize();
			}
		}

		private bool IsElementMouseOver
		{
			get => _isElementMouseOver;
			set
			{
				if (_isElementMouseOver == value)
					return;

				if (_isElementMouseOver)
					OnMouseLeaveElement();

				_isElementMouseOver = value;

				if (_isElementMouseOver)
					OnMouseEnterElement();
			}
		}

		private ElementRenderer Renderer { get; }

		private SpyZoomElementSizeRenderer SizeRenderer { get; } = new()
		{
			Visibility = Visibility.Collapsed
		};

		private SpyZoomControlTemplateContract TemplateContract => (SpyZoomControlTemplateContract)TemplateContractCore;

		private void ElementWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateArtboardSize();
		}

		private void OnElementLayoutUpdated(object sender, EventArgs e)
		{
			UpdateElementBox();
			UpdateElementWindow();
			UpdateSizeRenderer();
		}

		private void OnElementPropertyChangedPrivate(UIElement oldValue, UIElement newValue)
		{
			if (oldValue is FrameworkElement oldFrameworkElement)
				oldFrameworkElement.LayoutUpdated -= OnElementLayoutUpdated;

			if (newValue is FrameworkElement newFrameworkElement)
				newFrameworkElement.LayoutUpdated += OnElementLayoutUpdated;

			UpdateElementBox();

			IsElementMouseOver = ElementBox.IsEmpty == false && ElementBox.Contains(Mouse.GetPosition(Renderer));

			UpdateElementWindow();
			UpdateSizeRenderer();
		}

		private void OnElementRendererMouseMove(MouseEventArgs e)
		{
			if (ElementWindow == null || Element == null)
			{
				IsElementMouseOver = false;

				return;
			}

			IsElementMouseOver = ElementBox.IsEmpty == false && ElementBox.Contains(e.GetPosition(Renderer));
		}

		private void OnMouseEnterElement()
		{
			SizeRenderer.Visibility = Visibility.Visible;
		}

		private void OnMouseLeaveElement()
		{
			SizeRenderer.Visibility = Visibility.Collapsed;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ArtboardControl.ItemCollection.Add(ArtboardItem);

			ArtboardControl.ShowGrid = false;

			UpdateArtboardSize();
		}

		protected override void OnTemplateContractDetaching()
		{
			ArtboardControl.ItemCollection.Remove(ArtboardItem);

			base.OnTemplateContractDetaching();
		}

		private void UpdateArtboardSize()
		{
			if (ArtboardControl == null)
				return;

			var size = ElementWindow?.RenderSize ?? new Size(600, 400);

			ArtboardItem.Width = size.Width;
			ArtboardItem.Height = size.Height;

			Renderer.Width = size.Width;
			Renderer.Height = size.Height;
		}

		private void UpdateElementBox()
		{
			if (ElementWindow != null && Element is FrameworkElement element && element.IsVisualDescendantOf(ElementWindow))
				ElementBox = element.GetBoundingBox(ElementWindow);
			else
				ElementBox = Rect.Empty;
		}

		private void UpdateElementWindow()
		{
			if (Element == null)
			{
				ElementWindow = null;

				return;
			}

			var elementWindow = Window.GetWindow(Element);

			if (elementWindow != null)
				ElementWindow = elementWindow;
		}

		private void UpdateSizeRenderer()
		{
			if (Element is not FrameworkElement element)
				return;

			var box = element.GetVisualRootBox();

			SizeRenderer.ElementSize = box.Size;

			ArtboardCanvas.SetPosition(SizeRenderer, box.Location.WithOffset(0, -SizeRenderer.ActualHeight));
		}

		internal class ElementRenderer : Panel
		{
			public ElementRenderer(SpyZoomControl spyZoomControl)
			{
				SpyZoomControl = spyZoomControl;
			}

			public SpyZoomControl SpyZoomControl { get; }

			protected override void OnMouseMove(MouseEventArgs e)
			{
				SpyZoomControl.OnElementRendererMouseMove(e);
			}
		}
	}
}