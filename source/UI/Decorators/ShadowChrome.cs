// <copyright file="ShadowChrome.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Zaaml.PresentationCore.Extensions;
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

		public static readonly DependencyProperty SideProperty = DPM.Register<ShadowSide, ShadowChrome>
			("Side", ShadowSide.All, d => d.OnSidePropertyChangedPrivate);

		static ShadowChrome()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ShadowChrome>();
		}

		public ShadowChrome()
		{
			UseLayoutRounding = true;

			OuterClipGeometry = new RectangleGeometry();
			
			ShadowEffect = new DropShadowEffect
			{
				Opacity = 1.0,
				BlurRadius = 0,
				Direction = 0,
				ShadowDepth = 0,
				Color = Colors.Black
			};

			ShadowBorder = new ClipBorder
			{
				Background = new SolidColorBrush(ShadowColor),
				Effect = ShadowEffect
			};

			ShadowClipGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, OuterClipGeometry, Geometry.Empty);

			Clip = ShadowClipGeometry;

			Popup.SetHitTestVisible(this, false);
			Popup.SetHitTestVisible(ShadowBorder, false);

			this.OverrideStyleKey<ShadowChrome>();

			Focusable = false;
			IsTabStop = false;
		}

		private protected override void OnDependencyPropertyChangedInternal(DependencyPropertyChangedEventArgs args)
		{
			if (args.Property == CornerRadiusProperty)
			{
				InvalidateArrange();

				ShadowBorder.CornerRadius = CornerRadius;
			}

			base.OnDependencyPropertyChangedInternal(args);
		}

		private CombinedGeometry ShadowClipGeometry { get; }

		private RectangleGeometry OuterClipGeometry { get; }

		private ClipBorder ShadowBorder { get; }

		private static Color ShadowColor => Colors.Black;

		private DropShadowEffect ShadowEffect { get; }

		public double ShadowOpacity
		{
			get => (double)GetValue(ShadowOpacityProperty);
			set => SetValue(ShadowOpacityProperty, value);
		}

		public double ShadowSize
		{
			get => (double)GetValue(ShadowSizeProperty);
			set => SetValue(ShadowSizeProperty, value);
		}

		public ShadowSide Side
		{
			get => (ShadowSide)GetValue(SideProperty);
			set => SetValue(SideProperty, value);
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

		private void OnSidePropertyChangedPrivate(ShadowSide oldValue, ShadowSide newValue)
		{
			UpdateClip(RenderSize);
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Children.Clear();

			base.UndoTemplateOverride();
		}

		private void UpdateClip(Size arrangeBounds)
		{
			var side = Side;
			var shadowSize = ShadowSize;
			var outerClipRect = new Rect(new Point(-shadowSize, -shadowSize), new Size(arrangeBounds.Width + shadowSize * 2, arrangeBounds.Height + shadowSize * 2));

			var left = (side & ShadowSide.Left) == 0;
			var top = (side & ShadowSide.Top) == 0;
			var right = (side & ShadowSide.Right) == 0;
			var bottom = (side & ShadowSide.Bottom) == 0;

			if (left)
				outerClipRect.X = 0;

			if (top)
				outerClipRect.Y = 0;

			if (right)
				outerClipRect.Width = left ? arrangeBounds.Width : arrangeBounds.Width + shadowSize;

			if (bottom)
				outerClipRect.Height = top ? arrangeBounds.Height : arrangeBounds.Height + shadowSize;

			OuterClipGeometry.Rect = outerClipRect;

			ShadowClipGeometry.Geometry2 = ClipBorder.BuildRoundRectangleGeometry(new Rect(arrangeBounds), CornerRadius);
		}

		private void UpdateMargin()
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.Margin = new Thickness(-ShadowSize);
			ShadowBorder.Margin = new Thickness(ShadowSize);
		}
	}

	[Flags]
	public enum ShadowSide
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		All = Left | Top | Right | Bottom
	}
}