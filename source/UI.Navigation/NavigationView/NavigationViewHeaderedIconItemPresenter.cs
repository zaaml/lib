// <copyright file="NavigationViewHeaderedIconItemPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.NavigationView
{
	public partial class NavigationViewHeaderedIconItemPresenter : Control
	{
		private static readonly DependencyPropertyKey NavigationViewItemPropertyKey = DPM.RegisterReadOnly<NavigationViewHeaderedIconItem, NavigationViewHeaderedIconItemPresenter>
			("NavigationViewItem", default, d => d.OnNavigationViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty NavigationViewItemProperty = NavigationViewItemPropertyKey.DependencyProperty;

		static NavigationViewHeaderedIconItemPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewHeaderedIconItemPresenter>();
		}

		public NavigationViewHeaderedIconItemPresenter()
		{
			_buttonController = new ButtonController<NavigationViewHeaderedIconItemPresenter>(this);

			this.OverrideStyleKey<NavigationViewHeaderedIconItemPresenter>();
		}

		public NavigationViewHeaderedIconItem NavigationViewItem
		{
			get => (NavigationViewHeaderedIconItem) GetValue(NavigationViewItemProperty);
			internal set => this.SetReadOnlyValue(NavigationViewItemPropertyKey, value);
		}

		private void OnNavigationViewItemPropertyChangedPrivate(NavigationViewHeaderedIconItem oldValue, NavigationViewHeaderedIconItem newValue)
		{
			UpdateVisualState(true);
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			if (NavigationViewItem == null)
				return;

			var isMouseOver = IsMouseOver;
			var isFocused = NavigationViewItem.IsFocusedVisualStateInternal;
			var isSelected = NavigationViewItem.IsSelectedVisualStateInternal;
			var isValid = NavigationViewItem.IsValidVisualStateInternal;
			var isPressed = IsPressed;
			var isExpanded = NavigationViewItem.IsExpandedVisualStateInternal;

			// Common states
			if (!IsEnabled)
				GotoVisualState(CommonVisualStates.Disabled, useTransitions);
			else if (isPressed)
				GotoVisualState(CommonVisualStates.Pressed, useTransitions);
			else if (isMouseOver)
				GotoVisualState(CommonVisualStates.MouseOver, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Normal, useTransitions);

			// Selection states
			if (isSelected)
			{
				if (isFocused)
					GotoVisualState(CommonVisualStates.Selected, useTransitions);
				else
				{
					if (GotoVisualState(CommonVisualStates.SelectedUnfocused, useTransitions) == false)
						GotoVisualState(CommonVisualStates.Selected, useTransitions);
				}
			}
			else
				GotoVisualState(CommonVisualStates.Unselected, useTransitions);

			// Expansion states
			if (isExpanded)
				GotoVisualState(CommonVisualStates.Expanded, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Collapsed, useTransitions);

			// Focus states
			if (isFocused)
				GotoVisualState(CommonVisualStates.Focused, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Unfocused, useTransitions);

			// Validation states
			if (isValid)
				GotoVisualState(CommonVisualStates.Valid, useTransitions);
			else
			{
				if (isFocused)
					GotoVisualState(CommonVisualStates.InvalidFocused, useTransitions);
				else
					GotoVisualState(CommonVisualStates.InvalidUnfocused, useTransitions);
			}
		}

		internal void UpdateVisualStateInternal(bool useTransitions)
		{
			UpdateVisualState(useTransitions);
		}
	}
}