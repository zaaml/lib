// <copyright file="PopupContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using ZaamlContentControl = Zaaml.UI.Controls.Core.ContentControl;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class PopupContentPresenter : ZaamlContentControl, IPopupChild
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

		public static readonly DependencyProperty PopupMinWidthProperty = DPM.Register<double, PopupContentPresenter>
			("PopupMinWidth", 0.0);

		public static readonly DependencyProperty PopupMaxWidthProperty = DPM.Register<double, PopupContentPresenter>
			("PopupMaxWidth", double.PositiveInfinity);

		public static readonly DependencyProperty PopupWidthProperty = DPM.Register<double, PopupContentPresenter>
			("PopupWidth", double.NaN);

		public static readonly DependencyProperty PopupMinHeightProperty = DPM.Register<double, PopupContentPresenter>
			("PopupMinHeight", 0.0);

		public static readonly DependencyProperty PopupMaxHeightProperty = DPM.Register<double, PopupContentPresenter>
			("PopupMaxHeight", double.PositiveInfinity);

		public static readonly DependencyProperty PopupHeightProperty = DPM.Register<double, PopupContentPresenter>
			("PopupHeight", double.NaN);

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
		
		public double PopupHeight
		{
			get => (double) GetValue(PopupHeightProperty);
			set => SetValue(PopupHeightProperty, value);
		}

		public double PopupMaxHeight
		{
			get => (double) GetValue(PopupMaxHeightProperty);
			set => SetValue(PopupMaxHeightProperty, value);
		}

		public double PopupMaxWidth
		{
			get => (double) GetValue(PopupMaxWidthProperty);
			set => SetValue(PopupMaxWidthProperty, value);
		}

		public double PopupMinHeight
		{
			get => (double) GetValue(PopupMinHeightProperty);
			set => SetValue(PopupMinHeightProperty, value);
		}

		public double PopupMinWidth
		{
			get => (double) GetValue(PopupMinWidthProperty);
			set => SetValue(PopupMinWidthProperty, value);
		}

		public double PopupWidth
		{
			get => (double) GetValue(PopupWidthProperty);
			set => SetValue(PopupWidthProperty, value);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Dispatcher.BeginInvoke(UpdateVisualStateInt);
		}

		private void OnIsOpenChanged()
		{
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
	}
}