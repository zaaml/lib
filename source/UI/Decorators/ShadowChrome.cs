// <copyright file="ShadowChrome.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Decorators
{
	public sealed class ShadowChrome : FixedTemplateControl<Panel>
	{
		public static readonly DependencyProperty ShadowSizeProperty = DPM.Register<double, ShadowChrome>
			("ShadowSize", s => s.OnShadowSizeChanged);

		public static readonly DependencyProperty ShadowOpacityProperty = DPM.Register<double, ShadowChrome>
			("ShadowOpacity", 1.0, s => s.OnShadowOpacityChanged);

		static ShadowChrome()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ShadowChrome>();
		}

		public ShadowChrome()
		{
			InnerClipGeometry = new RectangleGeometry();
			OuterClipGeometry = new RectangleGeometry();

			Clip = new CombinedGeometry(GeometryCombineMode.Exclude, OuterClipGeometry, InnerClipGeometry);
			
			ShadowEffect = new DropShadowEffect
			{
				Opacity = 1.0,
				BlurRadius = 0,
				Direction = 0,
				ShadowDepth = 0
			};

			ShadowBorder = new Border
			{
				Background = new SolidColorBrush(ShadowColor),
				Effect = ShadowEffect
			};

			Popup.SetHitTestVisible(this, false);
			Popup.SetHitTestVisible(ShadowBorder, false);
			
			this.OverrideStyleKey<ShadowChrome>();

#if !SILVERLIGHT
			Focusable = false;
#endif
			IsTabStop = false;
		}

		private RectangleGeometry InnerClipGeometry { get; }
		
		private RectangleGeometry OuterClipGeometry { get; }
		
		private Border ShadowBorder { get; }

		private static Color ShadowColor => Color.FromArgb(255, 0, 0, 0);

		private DropShadowEffect ShadowEffect { get; }

		public double ShadowOpacity
		{
			get => (double) GetValue(ShadowOpacityProperty);
			set => SetValue(ShadowOpacityProperty, value);
		}

		public double ShadowSize
		{
			get => (double) GetValue(ShadowSizeProperty);
			set => SetValue(ShadowSizeProperty, value);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Children.Add(ShadowBorder);
			UpdateMargin();
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			UpdateClip(arrangeBounds);
			
			return base.ArrangeOverride(arrangeBounds);
		}

		private void UpdateClip(Size arrangeBounds)
		{
			var shadowSize = ShadowSize;
			
			OuterClipGeometry.Rect = new Rect(new Point(-shadowSize, -shadowSize), new Size(arrangeBounds.Width + shadowSize * 2, arrangeBounds.Height + shadowSize * 2));
			InnerClipGeometry.Rect = new Rect(arrangeBounds);
		}

		private void OnShadowOpacityChanged()
		{
			ShadowEffect.Opacity = ShadowOpacity;
		}

		private void OnShadowSizeChanged()
		{
			UpdateMargin();

			UpdateClip(RenderSize);
			ShadowEffect.BlurRadius = ShadowSize;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Children.Clear();

			base.UndoTemplateOverride();
		}

		private void UpdateMargin()
		{
			if (TemplateRoot == null)
				return;
			
			TemplateRoot.Margin = new Thickness(-ShadowSize);
			ShadowBorder.Margin = new Thickness(ShadowSize);
		}
	}
}