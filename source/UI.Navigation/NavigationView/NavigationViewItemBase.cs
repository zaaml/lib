// <copyright file="NavigationViewItemBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewItemBaseTemplateContract))]
	public abstract class NavigationViewItemBase : TemplateContractControl
	{
		private static readonly DependencyPropertyKey DisplayModePropertyKey = DPM.RegisterReadOnly<NavigationViewItemDisplayMode, NavigationViewItemBase>
			("DisplayMode", NavigationViewItemDisplayMode.Expanded);

		public static readonly DependencyProperty DisplayModeProperty = DisplayModePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey NavigationViewControlPropertyKey = DPM.RegisterReadOnly<NavigationViewControl, NavigationViewItemBase>
			("NavigationViewControl", default, d => d.OnNavigationViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty NavigationViewControlProperty = NavigationViewControlPropertyKey.DependencyProperty;
		private bool _forceCompactDisplayMode;

		public NavigationViewItemDisplayMode DisplayMode
		{
			get => (NavigationViewItemDisplayMode) GetValue(DisplayModeProperty);
			internal set => this.SetReadOnlyValue(DisplayModePropertyKey, value);
		}

		internal bool ForceCompactDisplayMode
		{
			get => _forceCompactDisplayMode;
			set
			{
				if (_forceCompactDisplayMode == value)
					return;

				_forceCompactDisplayMode = value;

				UpdateDisplayMode();
			}
		}

		private protected virtual bool IsExpandedVisualState => false;

		internal bool IsExpandedVisualStateInternal => IsExpandedVisualState;

		private protected virtual bool IsFocusedVisualState => false;

		internal bool IsFocusedVisualStateInternal => IsFocusedVisualState;

		private protected virtual bool IsPressedVisualState => false;

		private protected virtual bool IsSelectedVisualState => false;

		internal bool IsSelectedVisualStateInternal => IsSelectedVisualState;

		private protected virtual bool IsValidVisualState => true;

		internal bool IsValidVisualStateInternal => IsValidVisualState;

		public NavigationViewControl NavigationViewControl
		{
			get => (NavigationViewControl) GetValue(NavigationViewControlProperty);
			internal set => this.SetReadOnlyValue(NavigationViewControlPropertyKey, value);
		}

		private protected virtual void OnCommandChangedCore()
		{
		}

		private protected virtual void OnCommandParameterChangedCore()
		{
		}

		private protected virtual void OnCommandTargetChangedCore()
		{
		}

		private void OnDisplayModeChanged(object sender, NavigationViewDisplayModeChangedEventArgs e)
		{
			UpdateDisplayMode();
		}

		private void OnIsPaneExpandedChanged(object sender, EventArgs e)
		{
			UpdateDisplayMode();
		}

		protected virtual void OnNavigationViewControlChanged(NavigationViewControl oldNavigationView, NavigationViewControl newNavigationView)
		{
		}

		internal virtual void OnNavigationViewControlChangedInternal(NavigationViewControl oldNavigationView, NavigationViewControl newNavigationView)
		{
			OnNavigationViewControlChanged(oldNavigationView, newNavigationView);
		}

		private void OnNavigationViewControlPropertyChangedPrivate(NavigationViewControl oldNavigationView, NavigationViewControl newNavigationView)
		{
			OnNavigationViewControlChangedInternal(oldNavigationView, newNavigationView);

			if (oldNavigationView != null)
			{
				oldNavigationView.DisplayModeChanged -= OnDisplayModeChanged;
				oldNavigationView.IsPaneOpenChanged -= OnIsPaneExpandedChanged;
			}

			if (newNavigationView != null)
			{
				newNavigationView.DisplayModeChanged += OnDisplayModeChanged;
				newNavigationView.IsPaneOpenChanged += OnIsPaneExpandedChanged;
			}
		}

		private void UpdateDisplayMode()
		{
			var displayMode = NavigationViewItemDisplayMode.Expanded;

			if (ForceCompactDisplayMode)
				displayMode = NavigationViewItemDisplayMode.Compact;
			else if (NavigationViewControl != null)
				displayMode = NavigationViewControl.IsPaneOpen ? NavigationViewItemDisplayMode.Expanded : NavigationViewItemDisplayMode.Compact;

			DisplayMode = displayMode;
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
		}
	}
}