// <copyright file="MenuItemHeaderPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	public sealed class MenuItemHeaderPresenter : HeaderedContentControl, IIconOwner
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty InputGestureTextProperty = DPM.Register<string, MenuItemHeaderPresenter>
			(nameof(InputGestureText));

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, MenuItemHeaderPresenter>
			(nameof(Icon));

		private static readonly DependencyPropertyKey ActualIsCheckedPropertyKey = DPM.RegisterReadOnly<bool?, MenuItemHeaderPresenter>
			(nameof(ActualIsChecked), false, m => m.UpdateVisualStateInt);

		public static readonly DependencyProperty ActualIsCheckedProperty = ActualIsCheckedPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualIsHighlightedPropertyKey = DPM.RegisterReadOnly<bool, MenuItemHeaderPresenter>
			(nameof(ActualIsHighlighted), m => m.UpdateVisualStateInt);

		public static readonly DependencyProperty ActualIsHighlightedProperty = ActualIsHighlightedPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualShowSubmenuGlyphPropertyKey = DPM.RegisterReadOnly<bool, MenuItemHeaderPresenter>
			(nameof(ActualShowSubmenuGlyph), m => m.UpdateVisualStateInt);

		public static readonly DependencyProperty ActualShowSubmenuGlyphProperty = ActualShowSubmenuGlyphPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualRolePropertyKey = DPM.RegisterReadOnly<MenuItemRole, MenuItemHeaderPresenter>
			(nameof(ActualRole));

		public static readonly DependencyProperty ActualRoleProperty = ActualRolePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey MenuItemPropertyKey = DPM.RegisterReadOnly<HeaderedMenuItem, MenuItemHeaderPresenter>
			(nameof(MenuItem), h => h.OnMenuItemChanged);

		public static readonly DependencyProperty MenuItemProperty = MenuItemPropertyKey.DependencyProperty;
		private bool _isPressed;

		#endregion

		#region Ctors

		static MenuItemHeaderPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuItemHeaderPresenter>();
		}

		public MenuItemHeaderPresenter()
		{
			this.OverrideStyleKey<MenuItemHeaderPresenter>();
		}

		#endregion

		#region Properties

		public HeaderedMenuItem MenuItem
		{
			get => (HeaderedMenuItem) GetValue(MenuItemProperty);
			internal set => this.SetReadOnlyValue(MenuItemPropertyKey, value);
		}

		public MenuItemRole ActualRole
		{
			get => (MenuItemRole) GetValue(ActualRoleProperty);
			internal set => this.SetReadOnlyValue(ActualRolePropertyKey, value);
		}

		public bool ActualShowSubmenuGlyph
		{
			get => (bool) GetValue(ActualShowSubmenuGlyphProperty);
			internal set => this.SetReadOnlyValue(ActualShowSubmenuGlyphPropertyKey, value);
		}

		public bool ActualIsHighlighted
		{
			get => (bool) GetValue(ActualIsHighlightedProperty);
			internal set => this.SetReadOnlyValue(ActualIsHighlightedPropertyKey, value);
		}

		public bool? ActualIsChecked
		{
			get => (bool?) GetValue(ActualIsCheckedProperty);
			internal set => this.SetReadOnlyValue(ActualIsCheckedPropertyKey, value);
		}

		public IconBase Icon
		{
			get => (IconBase) GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public string InputGestureText
		{
			get => (string) GetValue(InputGestureTextProperty);
			set => SetValue(InputGestureTextProperty, value);
		}

		#endregion

		#region  Methods

#if SILVERLIGHT
    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      MenuItem?.OnHeaderGotKeyboardFocus();
    }
#else
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);

			MenuItem?.OnHeaderGotKeyboardFocus();
		}
# endif

		private void OnMenuItemChanged(HierarchicalMenuItem oldMenuItem, HierarchicalMenuItem newMenuItem)
		{
		}

		private void UpdateVisualStateInt()
		{
			UpdateVisualState(true);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			MenuItem?.OnHeaderMouseEnter();

			if (MouseInternal.LeftButtonState == MouseButtonState.Pressed)
				IsPressed = true;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			MenuItem?.OnHeaderMouseLeave();

			IsPressed = false;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			MenuItem?.OnHeaderMouseLeftButtonDown(e);

			IsPressed = true;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			MenuItem?.OnHeaderMouseLeftButtonUp(e);

			IsPressed = false;
		}

		private bool IsPressed
		{
			get => _isPressed;
			set
			{
				if (_isPressed == value)
					return;

				_isPressed = value;

				UpdateVisualState(true);
			}
		}

		protected override void OnUnloaded()
		{
			base.OnUnloaded();

#if SILVERLIGHT
      IsMouseOver = false;
#endif
			IsPressed = false;
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			if (IsEnabled == false)
				GotoVisualState("Disabled", useTransitions);
			else if (IsPressed)
				GotoVisualState("Pressed", useTransitions);
			else if (IsMouseOver)
				GotoVisualState("MouseOver", useTransitions);
			else if (ActualIsHighlighted)
				GotoVisualState("Highlighted", useTransitions);
			else
				GotoVisualState("Normal", useTransitions);

			var isChecked = ActualIsChecked;

			if (isChecked == false)
				GotoVisualState(CommonVisualStates.Unchecked, useTransitions);
			else if (isChecked == true)
				GotoVisualState(CommonVisualStates.Checked, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Indeterminate, useTransitions);
		}

		#endregion
	}
}