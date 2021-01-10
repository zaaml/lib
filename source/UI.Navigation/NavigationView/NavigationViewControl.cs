// <copyright file="NavigationViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Windows;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewControlTemplateContract))]
	public class NavigationViewControl : ItemsControlBase<NavigationViewControl, NavigationViewItemBase, NavigationViewItemCollection, NavigationViewItemsPresenter, NavigationViewPanel>
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, NavigationViewControl>
			("Content", n => n.LogicalChildMentor.OnLogicalChildPropertyChanged);

		public static readonly DependencyProperty PaneTitleProperty = DPM.Register<string, NavigationViewControl>
			("PaneTitle");

		private static readonly DependencyPropertyKey DisplayModePropertyKey = DPM.RegisterReadOnly<NavigationViewDisplayMode, NavigationViewControl>
			("DisplayMode", NavigationViewDisplayMode.Expanded, d => d.OnDisplayModePropertyChangedPrivate);

		public static readonly DependencyProperty PaneDisplayModeProperty = DPM.Register<NavigationViewPaneDisplayMode, NavigationViewControl>
			("PaneDisplayMode", NavigationViewPaneDisplayMode.Auto, d => d.OnPaneDisplayModePropertyChangedPrivate);

		public static readonly DependencyProperty OpenPaneLengthProperty = DPM.Register<double, NavigationViewControl>
			("OpenPaneLength", 320);

		public static readonly DependencyProperty CompactPaneLengthProperty = DPM.Register<double, NavigationViewControl>
			("CompactPaneLength", 48);

		public static readonly DependencyProperty CompactModeThresholdWidthProperty = DPM.Register<double, NavigationViewControl>
			("CompactModeThresholdWidth", 640, d => d.OnCompactModeThresholdWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ExpandedModeThresholdWidthProperty = DPM.Register<double, NavigationViewControl>
			("ExpandedModeThresholdWidth", 1008, d => d.OnExpandedModeThresholdWidthPropertyChangedPrivate);

		public static readonly DependencyProperty IsPaneToggleButtonVisibleProperty = DPM.Register<bool, NavigationViewControl>
			("IsPaneToggleButtonVisible", true);

		public static readonly DependencyProperty IsPaneOpenProperty = DPM.Register<bool, NavigationViewControl>
			("IsPaneOpen", true, n => n.OnIsPaneOpenPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ParentWindowPropertyKey = DPM.RegisterReadOnly<WindowBase, NavigationViewControl>
			("ParentWindow");

		public static readonly DependencyProperty ParentWindowProperty = ParentWindowPropertyKey.DependencyProperty;

		public static readonly DependencyProperty DisplayModeProperty = DisplayModePropertyKey.DependencyProperty;

		public event EventHandler IsPaneOpenChanged;

		public event EventHandler<NavigationViewDisplayModeChangedEventArgs> DisplayModeChanged;

		static NavigationViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewControl>();
		}

		public NavigationViewControl()
		{
			this.OverrideStyleKey<NavigationViewControl>();
		}

		public double CompactModeThresholdWidth
		{
			get => (double) GetValue(CompactModeThresholdWidthProperty);
			set => SetValue(CompactModeThresholdWidthProperty, value);
		}

		public double CompactPaneLength
		{
			get => (double) GetValue(CompactPaneLengthProperty);
			set => SetValue(CompactPaneLengthProperty, value);
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public NavigationViewDisplayMode DisplayMode
		{
			get => (NavigationViewDisplayMode) GetValue(DisplayModeProperty);
			private set => this.SetReadOnlyValue(DisplayModePropertyKey, value);
		}

		public double ExpandedModeThresholdWidth
		{
			get => (double) GetValue(ExpandedModeThresholdWidthProperty);
			set => SetValue(ExpandedModeThresholdWidthProperty, value);
		}

		public bool IsPaneOpen
		{
			get => (bool) GetValue(IsPaneOpenProperty);
			set => SetValue(IsPaneOpenProperty, value);
		}

		private bool IsPaneOpenExpandedMode { get; set; } = true;

		public bool IsPaneToggleButtonVisible
		{
			get => (bool) GetValue(IsPaneToggleButtonVisibleProperty);
			set => SetValue(IsPaneToggleButtonVisibleProperty, value);
		}

		public double OpenPaneLength
		{
			get => (double) GetValue(OpenPaneLengthProperty);
			set => SetValue(OpenPaneLengthProperty, value);
		}

		public NavigationViewPaneDisplayMode PaneDisplayMode
		{
			get => (NavigationViewPaneDisplayMode) GetValue(PaneDisplayModeProperty);
			set => SetValue(PaneDisplayModeProperty, value);
		}

		public string PaneTitle
		{
			get => (string) GetValue(PaneTitleProperty);
			set => SetValue(PaneTitleProperty, value);
		}

		public WindowBase ParentWindow
		{
			get => (WindowBase) GetValue(ParentWindowProperty);
			private set => this.SetReadOnlyValue(ParentWindowPropertyKey, value);
		}

		private void ClosePane()
		{
			this.SetCurrentValueInternal(IsPaneOpenProperty, false);
		}

		protected override NavigationViewItemCollection CreateItemCollection()
		{
			return new NavigationViewItemCollection(this);
		}

		private void OnCompactModeThresholdWidthPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateDisplayMode();
		}

		private void OnDisplayModePropertyChangedPrivate(NavigationViewDisplayMode oldValue, NavigationViewDisplayMode newValue)
		{
			if (oldValue == NavigationViewDisplayMode.Expanded)
				IsPaneOpenExpandedMode = IsPaneOpen;

			if (newValue == NavigationViewDisplayMode.Expanded)
			{
				if (IsPaneOpenExpandedMode)
					OpenPane();
			}
			else
				ClosePane();

			DisplayModeChanged?.Invoke(this, new NavigationViewDisplayModeChangedEventArgs(oldValue, newValue));
		}

		private void OnExpandedModeThresholdWidthPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateDisplayMode();
		}

		private void OnIsPaneOpenPropertyChangedPrivate()
		{
			IsPaneOpenChanged?.Invoke(this, EventArgs.Empty);
		}

		internal override void OnItemAttachedInternal(NavigationViewItemBase item)
		{
			item.NavigationViewControl = this;

			base.OnItemAttachedInternal(item);
		}

		internal void OnItemClick(NavigationViewHeaderedIconItem item)
		{
			if (DisplayMode == NavigationViewDisplayMode.Minimal || DisplayMode == NavigationViewDisplayMode.Compact)
			{
				if (item is NavigationViewItem || item is NavigationViewCommandItem)
					ClosePane();
			}
		}

		internal override void OnItemDetachedInternal(NavigationViewItemBase item)
		{
			base.OnItemDetachedInternal(item);

			item.NavigationViewControl = null;
		}

		protected override void OnLoaded()
		{
			ParentWindow = Parent as WindowBase;

			base.OnLoaded();
		}

		private void OnPaneDisplayModePropertyChangedPrivate(NavigationViewPaneDisplayMode oldValue, NavigationViewPaneDisplayMode newValue)
		{
			UpdateDisplayMode();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			UpdateDisplayMode();
		}

		protected override void OnUnloaded()
		{
			base.OnUnloaded();

			ParentWindow = null;
		}

		private void OpenPane()
		{
			this.SetCurrentValueInternal(IsPaneOpenProperty, true);
		}

		private void UpdateDisplayMode()
		{
			if (PaneDisplayMode == NavigationViewPaneDisplayMode.Auto)
			{
				var displayMode = NavigationViewDisplayMode.Minimal;

				if (ActualWidth >= CompactModeThresholdWidth)
					displayMode = NavigationViewDisplayMode.Compact;

				if (ActualWidth >= ExpandedModeThresholdWidth)
					displayMode = NavigationViewDisplayMode.Expanded;

				if (DisplayMode != displayMode)
					DisplayMode = displayMode;
			}
			else if (PaneDisplayMode == NavigationViewPaneDisplayMode.Left)
				DisplayMode = NavigationViewDisplayMode.Expanded;
			else if (PaneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact || PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
				DisplayMode = NavigationViewDisplayMode.Compact;
			else if (PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal)
				DisplayMode = NavigationViewDisplayMode.Minimal;
		}
	}
}