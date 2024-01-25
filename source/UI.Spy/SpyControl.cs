// <copyright file="SpyControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Spy
{
	[TemplateContractType(typeof(SpyControlTemplateContract))]
	public class SpyControl : TemplateContractControl
	{
		public static readonly DependencyProperty TrackerProperty = DPM.Register<SpyElementTracker, SpyControl>
			("Tracker", d => d.OnTrackerPropertyChangedPrivate);

		public static readonly DependencyProperty ElementProperty = DPM.Register<UIElement, SpyControl>
			("Element", d => d.OnElementPropertyChanged);

		public static readonly DependencyProperty DisplayModeProperty = DPM.Register<SpyControlDisplayMode, SpyControl>
			("DisplayMode", SpyControlDisplayMode.PropertyView);

		static SpyControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<SpyControl>();
		}

		public SpyControl()
		{
			this.OverrideStyleKey<SpyControl>();
		}

		public SpyControlDisplayMode DisplayMode
		{
			get => (SpyControlDisplayMode)GetValue(DisplayModeProperty);
			set => SetValue(DisplayModeProperty, value);
		}

		public UIElement Element
		{
			get => (UIElement)GetValue(ElementProperty);
			set => SetValue(ElementProperty, value);
		}

		private SpyPropertyViewControl PropertyView => TemplateContract.PropertyView;

		private SpyControlTemplateContract TemplateContract => (SpyControlTemplateContract)TemplateContractCore;

		public SpyElementTracker Tracker
		{
			get => (SpyElementTracker)GetValue(TrackerProperty);
			set => SetValue(TrackerProperty, value);
		}

		private SpyVisualTreeViewControl VisualTree => TemplateContract.VisualTree;

		private SpyZoomControl ZoomControl => TemplateContract.ZoomControl;

		private void OnElementPropertyChanged(UIElement oldElement, UIElement newElement)
		{
		}

		private void OnTrackerPropertyChangedPrivate(SpyElementTracker oldValue, SpyElementTracker newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null && this.ReadLocalBinding(ElementProperty) is { } binding && ReferenceEquals(binding.Source, oldValue))
				ClearValue(ElementProperty);

			if (newValue != null)
				SetBinding(ElementProperty, new Binding { Path = new PropertyPath(SpyElementTracker.ElementProperty), Source = newValue, Mode = BindingMode.TwoWay });
		}
	}

	public enum SpyControlDisplayMode
	{
		PropertyView,
		ZoomView
	}
}