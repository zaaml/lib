// <copyright file="PopupContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	[TemplateContractType(typeof(PopupContentPresenterTemplateContract))]
	public sealed class PopupContentPresenter : TemplateContractContentControl, IPopupChild
	{
		private static readonly DependencyPropertyKey ActualIsOpenPropertyKey = DPM.RegisterReadOnly<bool, PopupContentPresenter>
			("ActualIsOpen", p => p.OnIsOpenChanged);

		public static readonly DependencyProperty ActualIsOpenProperty = ActualIsOpenPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey PopupPropertyKey = DPM.RegisterReadOnly<Popup, PopupContentPresenter>
			("Popup");

		public static readonly DependencyProperty PopupProperty = PopupPropertyKey.DependencyProperty;

		public static readonly DependencyProperty DropShadowProperty = DPM.Register<bool, PopupContentPresenter>
			("DropShadow", true);

		public static readonly DependencyProperty BorderStyleProperty = DPM.Register<PopupBorderStyle, PopupContentPresenter>
			("BorderStyle", PopupBorderStyle.Border);

		public static readonly DependencyProperty AllowMotionAnimationProperty = DPM.Register<bool, PopupContentPresenter>
			("AllowMotionAnimation");

		public static readonly DependencyProperty AllowOpacityAnimationProperty = DPM.Register<bool, PopupContentPresenter>
			("AllowOpacityAnimation");

		public static readonly DependencyProperty PopupMinWidthProperty = DPM.Register<PopupLength, PopupContentPresenter>
			("PopupMinWidth", new PopupLength(0.0), p => p.OnPopupSizePropertyChanged);

		public static readonly DependencyProperty PopupMaxWidthProperty = DPM.Register<PopupLength, PopupContentPresenter>
			("PopupMaxWidth", new PopupLength(double.PositiveInfinity), p => p.OnPopupSizePropertyChanged);

		public static readonly DependencyProperty PopupWidthProperty = DPM.Register<PopupLength, PopupContentPresenter>
			("PopupWidth", PopupLength.Auto, p => p.OnPopupSizePropertyChanged);

		public static readonly DependencyProperty PopupMinHeightProperty = DPM.Register<PopupLength, PopupContentPresenter>
			("PopupMinHeight", new PopupLength(0.0), p => p.OnPopupSizePropertyChanged);

		public static readonly DependencyProperty PopupMaxHeightProperty = DPM.Register<PopupLength, PopupContentPresenter>
			("PopupMaxHeight", new PopupLength(double.PositiveInfinity), p => p.OnPopupSizePropertyChanged);

		public static readonly DependencyProperty PopupHeightProperty = DPM.Register<PopupLength, PopupContentPresenter>
			("PopupHeight", PopupLength.Auto, p => p.OnPopupSizePropertyChanged);

		static PopupContentPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PopupContentPresenter>();
		}

		public PopupContentPresenter()
		{
			this.OverrideStyleKey<PopupContentPresenter>();

			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;

#if !SILVERLIGHT
			Focusable = true;
#endif
			IsTabStop = false;
		}

		public bool ActualIsOpen
		{
			get => (bool) GetValue(ActualIsOpenProperty);
			private set => this.SetReadOnlyValue(ActualIsOpenPropertyKey, value);
		}

		public bool AllowMotionAnimation
		{
			get => (bool) GetValue(AllowMotionAnimationProperty);
			set => SetValue(AllowMotionAnimationProperty, value);
		}

		public bool AllowOpacityAnimation
		{
			get => (bool) GetValue(AllowOpacityAnimationProperty);
			set => SetValue(AllowOpacityAnimationProperty, value);
		}

		public PopupBorderStyle BorderStyle
		{
			get => (PopupBorderStyle) GetValue(BorderStyleProperty);
			set => SetValue(BorderStyleProperty, value);
		}

		private FrameworkElement ContentHost => TemplateContract.ContentHost;

		private ContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		public bool DropShadow
		{
			get => (bool) GetValue(DropShadowProperty);
			set => SetValue(DropShadowProperty, value);
		}

		public Popup Popup
		{
			get => this.GetReadOnlyValue<Popup>(PopupPropertyKey);
			internal set => this.SetReadOnlyValue(PopupPropertyKey, value);
		}

		public PopupLength PopupHeight
		{
			get => (PopupLength) GetValue(PopupHeightProperty);
			set => SetValue(PopupHeightProperty, value);
		}

		public PopupLength PopupMaxHeight
		{
			get => (PopupLength) GetValue(PopupMaxHeightProperty);
			set => SetValue(PopupMaxHeightProperty, value);
		}

		public PopupLength PopupMaxWidth
		{
			get => (PopupLength) GetValue(PopupMaxWidthProperty);
			set => SetValue(PopupMaxWidthProperty, value);
		}

		public PopupLength PopupMinHeight
		{
			get => (PopupLength) GetValue(PopupMinHeightProperty);
			set => SetValue(PopupMinHeightProperty, value);
		}

		public PopupLength PopupMinWidth
		{
			get => (PopupLength) GetValue(PopupMinWidthProperty);
			set => SetValue(PopupMinWidthProperty, value);
		}

		public PopupLength PopupWidth
		{
			get => (PopupLength) GetValue(PopupWidthProperty);
			set => SetValue(PopupWidthProperty, value);
		}

		private PopupContentPresenterTemplateContract TemplateContract => (PopupContentPresenterTemplateContract) TemplateContractInternal;

		private double GetSize(PopupLength popupLength, SizePart sizePart, double autoSize)
		{
			if (popupLength.UnitType == PopupLengthUnitType.Auto)
				return autoSize;

			if (popupLength.UnitType == PopupLengthUnitType.Absolute)
				return popupLength.Value;

			if (popupLength.RelativeElement == null)
				return autoSize;

			if (popupLength.RelativeElement.Equals("Target", StringComparison.OrdinalIgnoreCase))
			{
				if (Popup.Placement?.ActualPlacement is RelativePlacementBase relativePlacement && relativePlacement.Target != null)
					return popupLength.Value * GetSizePart(relativePlacement.Target.RenderSize, sizePart);
			}

			if (popupLength.RelativeElement.Equals("Screen", StringComparison.OrdinalIgnoreCase))
			{
				var screen = Screen.FromElement(this);

				if (screen != null)
					return popupLength.Value * GetSizePart(screen.Bounds.Size, sizePart);
			}

			return autoSize;
		}

		private static double GetSizePart(Size size, SizePart sizePart)
		{
			return sizePart == SizePart.Width ? size.Width : size.Height;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (ContentHost != null)
			{
				ContentHost.MinWidth = GetSize(PopupMinWidth, SizePart.Width, 0);
				ContentHost.MinHeight = GetSize(PopupMinHeight, SizePart.Height, 0);
				ContentHost.MaxWidth = GetSize(PopupMaxWidth, SizePart.Width, double.PositiveInfinity);
				ContentHost.MaxHeight = GetSize(PopupMaxHeight, SizePart.Height, double.PositiveInfinity);
				ContentHost.Width = GetSize(PopupWidth, SizePart.Width, double.NaN);
				ContentHost.Height = GetSize(PopupHeight, SizePart.Height, double.NaN);
			}

			return base.MeasureOverride(availableSize);
		}

		private void OnIsOpenChanged()
		{
			Dispatcher.BeginInvoke(UpdateVisualStateInt);
		}

		private void OnPopupSizePropertyChanged()
		{
			InvalidateMeasure();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Dispatcher.BeginInvoke(UpdateVisualStateInt);
		}

		protected override void OnUnloaded()
		{
			base.OnUnloaded();

			GotoVisualState("PopupClosed", false);
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			base.UpdateVisualState(useTransitions);

			GotoVisualState(ActualIsOpen ? "PopupOpened" : "PopupClosed", true);
		}

		private void UpdateVisualStateInt()
		{
			UpdateVisualState(true);
		}

		bool IPopupChild.IsOpen
		{
			set => ActualIsOpen = value;
		}

		private enum SizePart
		{
			Width,
			Height
		}
	}

	public class PopupContentPresenterTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public FrameworkElement ContentHost { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }
	}
}