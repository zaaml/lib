// <copyright file="HeaderedMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(HeaderedMenuItemTemplateContract))]
	public abstract class HeaderedMenuItem : HierarchicalMenuItem, IIconOwner
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, HeaderedMenuItem>
			(nameof(Header), m => m.OnHeaderChanged);

		public static readonly DependencyProperty HeaderTemplateProperty = DPM.Register<DataTemplate, HeaderedMenuItem>
			(nameof(HeaderTemplate), m => m.OnHeaderTemplateChanged);

		public static readonly DependencyProperty HeaderTemplateSelectorProperty = DPM.Register<DataTemplateSelector, HeaderedMenuItem>
			(nameof(HeaderTemplateSelector), m => m.OnHeaderTemplateSelectorChanged);

		public static readonly DependencyProperty HeaderStringFormatProperty = DPM.Register<string, HeaderedMenuItem>
			(nameof(HeaderStringFormat), m => m.OnHeaderStringFormatChanged);

		public static readonly DependencyProperty InputGestureTextProperty = DPM.Register<string, HeaderedMenuItem>
			(nameof(InputGestureText), m => m.OnInputGestureTextChanged);

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, HeaderedMenuItem>
			(nameof(Icon), null, m => m.OnIconChanged);

		private static readonly DependencyPropertyKey ActualRolePropertyKey = DPM.RegisterReadOnly<MenuItemRole, HeaderedMenuItem>
			(nameof(ActualRole), m => m.OnActualRoleChanged);

		private static readonly DependencyPropertyKey ActualShowHeaderPropertyKey = DPM.RegisterReadOnly<bool, HeaderedMenuItem>
			(nameof(ActualShowHeader));

		public static readonly DependencyProperty ActualShowHeaderProperty = ActualShowHeaderPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ActualRoleProperty = ActualRolePropertyKey.DependencyProperty;

		#endregion

		#region Fields

		private byte _packedValue;

		#endregion

		#region Properties

		public MenuItemRole ActualRole
		{
			get => (MenuItemRole) GetValue(ActualRoleProperty);
			private set => this.SetReadOnlyValue(ActualRolePropertyKey, value);
		}

		public bool ActualShowHeader
		{
			get => (bool) GetValue(ActualShowHeaderProperty);
			private set => this.SetReadOnlyValue(ActualShowHeaderPropertyKey, value);
		}

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		protected MenuItemHeaderPresenter HeaderPresenter => TemplateContract.HeaderPresenter;

		public string HeaderStringFormat
		{
			get => (string) GetValue(HeaderStringFormatProperty);
			set => SetValue(HeaderStringFormatProperty, value);
		}

		public DataTemplate HeaderTemplate
		{
			get => (DataTemplate) GetValue(HeaderTemplateProperty);
			set => SetValue(HeaderTemplateProperty, value);
		}

		public DataTemplateSelector HeaderTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(HeaderTemplateSelectorProperty);
			set => SetValue(HeaderTemplateSelectorProperty, value);
		}

		public string InputGestureText
		{
			get => (string) GetValue(InputGestureTextProperty);
			set => SetValue(InputGestureTextProperty, value);
		}

		private bool IsHeaderMouseOver
		{
			get => PackedDefinition.IsHeaderMouseOver.GetValue(_packedValue);
			set => PackedDefinition.IsHeaderMouseOver.SetValue(ref _packedValue, value);
		}

		internal override TimeSpan OpenCloseDelay => ActualRole == MenuItemRole.TopLevelHeaderPopup ? TimeSpan.FromMilliseconds(10) : MenuController.HoverDelay;

		private HeaderedMenuItemTemplateContract TemplateContract => (HeaderedMenuItemTemplateContract) TemplateContractInternal;

		#endregion

		#region  Methods

		private void OnActualRoleChanged(MenuItemRole oldRole, MenuItemRole newRole)
		{
			if (HeaderPresenter != null)
				HeaderPresenter.ActualRole = newRole;

			UpdateActualShowHeader();
		}

		internal override void OnHasSubmenuChangedInternal(bool oldHasSubmenu, bool newHasSubmenu)
		{
			if (HeaderPresenter != null)
				HeaderPresenter.ActualShowSubmenuGlyph = newHasSubmenu;

			UpdateRole();
		}

		private void OnHeaderChanged(object oldHeader, object newHeader)
		{
			if (HeaderPresenter != null)
				HeaderPresenter.Header = newHeader;

			UpdateActualShowHeader();
		}

		internal void OnHeaderGotKeyboardFocus()
		{
			MenuController.OnHeaderGotKeyboardFocus(this);
		}

		internal void OnHeaderMouseEnter()
		{
			IsHeaderMouseOver = true;
			MenuController.OnHeaderMouseEnter(this);
		}

		internal void OnHeaderMouseLeave()
		{
			IsHeaderMouseOver = false;
			MenuController.OnHeaderMouseLeave(this);
		}

    protected override void OnUnloaded()
    {
      base.OnUnloaded();

      UpdateVisualState(false);
    }

    internal virtual void OnHeaderMouseLeftButtonDown(MouseButtonEventArgs eventArgs)
		{
			MenuController.OnHeaderMouseLeftButtonDown(this);
		}

		internal virtual void OnHeaderMouseLeftButtonUp(MouseButtonEventArgs eventArgs)
		{
			MenuController.OnHeaderMouseLeftButtonUp(this);
		}

		private void OnHeaderStringFormatChanged(string oldHeaderStringFormat, string newHeaderStringFormat)
		{
			if (HeaderPresenter != null)
				HeaderPresenter.HeaderStringFormat = newHeaderStringFormat;

			UpdateActualShowHeader();
		}

		private void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
		{
			if (HeaderPresenter != null)
				HeaderPresenter.HeaderTemplate = newHeaderTemplate;

			UpdateActualShowHeader();
		}

		private void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
		{
			if (HeaderPresenter != null)
				HeaderPresenter.HeaderTemplateSelector = newHeaderTemplateSelector;

			UpdateActualShowHeader();
		}

		private void OnIconChanged(IconBase oldIcon, IconBase newIcon)
		{
			// TODO : Implement Icon preload (Silverlight image deferred loading issue)
			//(newIcon as BitmapIcon)?.Source?.Preload();

			LogicalChildMentor.OnLogicalChildPropertyChanged(oldIcon, newIcon);

			if (HeaderPresenter != null)
				HeaderPresenter.Icon = newIcon;
		}

		private void OnInputGestureTextChanged(string oldGestureText, string newGestureText)
		{
			if (HeaderPresenter != null)
				HeaderPresenter.InputGestureText = newGestureText;
		}

		internal override void OnIsLevelFocusedChangedInternal()
		{
			base.OnIsLevelFocusedChangedInternal();

			UpdateHeaderState();
		}

		internal override void OnOwnerChangedInternal(IMenuItemOwner oldOwner, IMenuItemOwner newOwner)
		{
			base.OnOwnerChangedInternal(oldOwner, newOwner);

			UpdateRole();
		}

		internal override void OnSubMenuModeChangedInternal()
		{
			base.OnSubMenuModeChangedInternal();

			UpdateRole();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			HeaderPresenter.MenuItem = this;

			HeaderPresenter.Header = Header;
			HeaderPresenter.HeaderTemplate = HeaderTemplate;
			HeaderPresenter.InputGestureText = InputGestureText;
			HeaderPresenter.Icon = Icon;

			UpdateHeaderState();
		}

		protected override void OnTemplateContractDetaching()
		{
			HeaderPresenter.MenuItem = null;

			base.OnTemplateContractDetaching();
		}

		private void UpdateActualShowHeader()
		{
			if (ActualRole == MenuItemRole.SubmenuHeaderInline)
				ActualShowHeader = Header != null || HeaderTemplate != null;
			else
				ActualShowHeader = true;
		}

		protected void UpdateHeaderState()
		{
			var menuItemHeader = HeaderPresenter;

			if (menuItemHeader == null)
				return;

			UpdateHeaderStateCore();
		}

		protected virtual void UpdateHeaderStateCore()
		{
			var menuItemHeader = HeaderPresenter;

			menuItemHeader.ActualIsHighlighted = IsLevelFocused;
			menuItemHeader.ActualShowSubmenuGlyph = HasSubmenu;
			menuItemHeader.ActualRole = ActualRole;
		}

		private void UpdateRole()
		{
			if (Owner is MenuBar)
				ActualRole = HasSubmenu == false ? MenuItemRole.TopLevelItem : (SubmenuMode == SubmenuMode.Inline ? MenuItemRole.TopLevelHeaderInline : MenuItemRole.TopLevelHeaderPopup);
			else
				ActualRole = HasSubmenu == false ? MenuItemRole.SubmenuItem : (SubmenuMode == SubmenuMode.Inline ? MenuItemRole.SubmenuHeaderInline : MenuItemRole.SubmenuHeaderPopup);
		}

		#endregion

		#region Interface Implementations

		#region IIconOwner

		public IconBase Icon
		{
			get => (IconBase) GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		#endregion

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsHeaderMouseOver;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsHeaderMouseOver = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}

	public class HeaderedMenuItemTemplateContract : HierarchicalMenuItemTemplateContract
	{
		#region Properties

		[TemplateContractPart(Required = true)]
		public MenuItemHeaderPresenter HeaderPresenter { get; [UsedImplicitly] private set; }

		#endregion
	}
}