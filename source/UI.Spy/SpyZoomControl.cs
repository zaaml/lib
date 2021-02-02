// <copyright file="SpyZoomControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Artboard;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Spy
{
	[TemplateContractType(typeof(SpyZoomControlTemplateContract))]
	public class SpyZoomControl : TemplateContractControl
	{
		public static readonly DependencyProperty ElementProperty = DPM.Register<UIElement, SpyZoomControl>
			("Element", d => d.OnElementPropertyChangedPrivate);

		private Window _elementWindow;

		static SpyZoomControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<SpyZoomControl>();
		}

		public SpyZoomControl()
		{
			this.OverrideStyleKey<SpyZoomControl>();

			Renderer = new ElementRenderer(this);

			ArtboardItem = new ArtboardItem
			{
				Canvas = new ArtboardCanvas
				{
					Children =
					{
						Renderer
					}
				}
			};
		}

		private SpyArtboardControl ArtboardControl => TemplateContract.ArtboardControl;

		private ArtboardItem ArtboardItem { get; }

		public UIElement Element
		{
			get => (UIElement) GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

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

		private ElementRenderer Renderer { get; }

		private SpyZoomControlTemplateContract TemplateContract => (SpyZoomControlTemplateContract) TemplateContractInternal;

		private void ElementWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateArtboardSize();
		}

		private void OnElementLayoutUpdated(object sender, EventArgs e)
		{
			UpdateElementWindow();
		}

		private void OnElementPropertyChangedPrivate(UIElement oldValue, UIElement newValue)
		{
			if (oldValue is FrameworkElement oldFrameworkElement)
				oldFrameworkElement.LayoutUpdated -= OnElementLayoutUpdated;

			if (newValue is FrameworkElement newFrameworkElement)
				newFrameworkElement.LayoutUpdated += OnElementLayoutUpdated;

			UpdateElementWindow();
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

			ArtboardItem.DesignWidth = size.Width;
			ArtboardItem.DesignHeight = size.Height;

			Renderer.Width = size.Width;
			Renderer.Height = size.Height;
		}

		private void UpdateElementWindow()
		{
			ElementWindow = Element != null ? Window.GetWindow(Element) : null;
		}

		internal class ElementRenderer : Panel
		{
			public ElementRenderer(SpyZoomControl spyZoomControl)
			{
				SpyZoomControl = spyZoomControl;
			}

			public SpyZoomControl SpyZoomControl { get; }
		}
	}
}