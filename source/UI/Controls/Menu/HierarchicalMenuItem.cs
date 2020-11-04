// <copyright file="HierarchicalMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(HierarchicalMenuItemTemplateContract))]
	public abstract class HierarchicalMenuItem : MenuItemBase
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey HasSubmenuPropertyKey = DPM.RegisterReadOnly<bool, HierarchicalMenuItem>
			(nameof(HasSubmenu), m => m.OnHasSubmenuChangedPrivate);

		public static readonly DependencyProperty HasSubmenuProperty = HasSubmenuPropertyKey.DependencyProperty;

		public static readonly DependencyProperty OpenSubmenuModeProperty = DPM.Register<OpenSubmenuMode, HierarchicalMenuItem>
			(nameof(OpenSubmenuMode), OpenSubmenuMode.Hover);

		public static readonly DependencyProperty IsSubmenuOpenProperty = DPM.Register<bool, HierarchicalMenuItem>
			(nameof(IsSubmenuOpen), false, m => m.OnIsSubMenuOpenChanged);

		public static readonly DependencyProperty IsSubmenuEnabledProperty = DPM.Register<bool, HierarchicalMenuItem>
			(nameof(IsSubmenuEnabled), true, m => m.OnIsSubmenuEnabledChanged);

		private static readonly DependencyPropertyKey HierarchyParentPropertyKey = DPM.RegisterReadOnly<HierarchicalMenuItem, HierarchicalMenuItem>
			(nameof(ParentMenuItem));

		public static readonly DependencyProperty ParentMenuItemProperty = HierarchyParentPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SubmenuModeProperty = DPM.Register<SubmenuMode, HierarchicalMenuItem>
			(nameof(SubmenuMode), SubmenuMode.Popup, m => m.OnSubmenuModeChangedPrivate);

		#endregion

		#region Fields

		private byte _packedValue;

		public event EventHandler SubMenuClosed;
		public event EventHandler SubMenuOpened;

		#endregion

		#region Ctors

		internal HierarchicalMenuItem()
		{
		}

		#endregion

		#region Properties

		public bool HasSubmenu
		{
			get => (bool) GetValue(HasSubmenuProperty);
			protected set => this.SetReadOnlyValue(HasSubmenuPropertyKey, value);
		}

		internal bool IsLevelFocused
		{
			get => PackedDefinition.IsLevelFocused.GetValue(_packedValue);
			set
			{
				if (IsLevelFocused == value)
					return;

				PackedDefinition.IsLevelFocused.SetValue(ref _packedValue, value);

				OnIsLevelFocusedChangedInternal();
			}
		}

		internal bool IsPopupVisited
		{
			get => PackedDefinition.IsPopupVisited.GetValue(_packedValue);
			set => PackedDefinition.IsPopupVisited.SetValue(ref _packedValue, value);
		}

		public bool IsSubmenuEnabled
		{
			get => (bool) GetValue(IsSubmenuEnabledProperty);
			set => SetValue(IsSubmenuEnabledProperty, value);
		}

		public bool IsSubmenuOpen
		{
			get => (bool) GetValue(IsSubmenuOpenProperty);
			set => SetValue(IsSubmenuOpenProperty, value);
		}

		internal override IMenuItemCollection ItemsCore => MenuItemCollection.Empty;

		internal IMenuBase MenuBase
		{
			get
			{
				for (var currentOwner = Owner; currentOwner != null; currentOwner = currentOwner.Owner)
				{
					if (currentOwner is IMenuBase menu)
						return menu;
				}

				return null;
			}
		}

		internal MenuController MenuController => MenuController.GetController(this);

		internal virtual TimeSpan OpenCloseDelay => MenuController.HoverDelay;

		public OpenSubmenuMode OpenSubmenuMode
		{
			get => (OpenSubmenuMode) GetValue(OpenSubmenuModeProperty);
			set => SetValue(OpenSubmenuModeProperty, value);
		}

		public HierarchicalMenuItem ParentMenuItem
		{
			get => (HierarchicalMenuItem) GetValue(ParentMenuItemProperty);
			private set => this.SetReadOnlyValue(HierarchyParentPropertyKey, value);
		}

		internal abstract FrameworkElement Submenu { get; }

		protected abstract FrameworkElement SubmenuElement { get; }

		protected Panel SubmenuHost => TemplateContract.SubmenuHost;

		public SubmenuMode SubmenuMode
		{
			get => (SubmenuMode) GetValue(SubmenuModeProperty);
			set => SetValue(SubmenuModeProperty, value);
		}

		internal Popup SubmenuPopup => TemplateContract.SubmenuPopup;

		private HierarchicalMenuItemTemplateContract TemplateContract => (HierarchicalMenuItemTemplateContract) TemplateContractInternal;

		#endregion

		#region  Methods

		protected void AttachLogicalSubmenu()
		{
			AddLogicalChild(SubmenuElement);
		}

		protected void DetachLogicalSubmenu()
		{
			RemoveLogicalChild(SubmenuElement);
		}

		internal IEnumerable<MenuItem> EnumerateMenuItemAncestors()
		{
			var menuItem = Owner as MenuItem;

			while (menuItem != null)
			{
				yield return menuItem;

				menuItem = menuItem.Owner as MenuItem;
			}
		}

		protected internal virtual bool ExecuteCommand()
		{
			return false;
		}

		internal virtual void OnHasSubmenuChangedInternal(bool oldHasSubmenu, bool newHasSubmenu)
		{
		}

		private void OnHasSubmenuChangedPrivate(bool oldHasSubmenu, bool newHasSubmenu)
		{
			OnHasSubmenuChangedInternal(oldHasSubmenu, newHasSubmenu);
		}

		internal virtual void OnIsLevelFocusedChangedInternal()
		{
		}

		private void OnIsSubmenuEnabledChanged()
		{
			if (IsSubmenuOpen && IsSubmenuEnabled == false)
				IsSubmenuOpen = false;
		}

		private void OnIsSubMenuOpenChanged()
		{
			MenuController.OnSubmenuOpenChanged(this);

			UpdateVisualState(true);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			MenuController.OnKeyDown(e);
		}

		internal override void OnOwnerChangedInternal(IMenuItemOwner oldOwner, IMenuItemOwner newOwner)
		{
			base.OnOwnerChangedInternal(oldOwner, newOwner);

			ParentMenuItem = newOwner as HierarchicalMenuItem;
		}

		private void OnPopupChildMouseEnter(object sender, MouseEventArgs e)
		{
			MenuController.OnPopupMouseEnter(this);
		}

		private void OnPopupChildMouseLeave(object sender, MouseEventArgs e)
		{
			MenuController.OnPopupMouseLeave(this);
		}

		protected virtual void OnSubMenuClosed()
		{
			SubMenuClosed?.Invoke(this, EventArgs.Empty);
		}

		internal void OnSubMenuClosedInt()
		{
			OnSubMenuClosed();
		}

		protected virtual void OnSubmenuElementChanged()
		{
			var submenuHost = SubmenuHost;

			if (submenuHost == null)
				return;

			submenuHost.Children.Clear();
			submenuHost.Children.Add(SubmenuElement);
		}

		internal virtual void OnSubMenuModeChangedInternal()
		{
		}

		private void OnSubmenuModeChangedPrivate()
		{
			OnSubMenuModeChangedInternal();
		}

		protected virtual void OnSubMenuOpened()
		{
			SubMenuOpened?.Invoke(this, EventArgs.Empty);
		}

		internal void OnSubMenuOpenedInt()
		{
			OnSubMenuOpened();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			if (SubmenuPopup != null)
			{
				SubmenuPopup.SetBinding(Popup.IsOpenProperty, new Binding { Path = new PropertyPath(IsSubmenuOpenProperty), Source = this, Mode = BindingMode.TwoWay });

				SubmenuPopup.Panel.MouseEnter += OnPopupChildMouseEnter;
				SubmenuPopup.Panel.MouseLeave += OnPopupChildMouseLeave;
			}

			if (SubmenuHost != null)
			{
				RemoveLogicalChild(SubmenuElement);

				SubmenuHost?.Children.Add(SubmenuElement);
			}
		}

		protected override void OnTemplateContractDetaching()
		{
			if (SubmenuPopup != null)
			{
				SubmenuPopup.ClearValue(Popup.IsOpenProperty);

				SubmenuPopup.Panel.MouseEnter -= OnPopupChildMouseEnter;
				SubmenuPopup.Panel.MouseLeave -= OnPopupChildMouseLeave;
			}

			if (SubmenuHost != null)
			{
				SubmenuHost?.Children.Clear();

				AddLogicalChild(SubmenuElement);
			}

			base.OnTemplateContractDetaching();
		}

		internal virtual void ResetPopupContent()
		{
		}

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsLevelFocused;
			public static readonly PackedBoolItemDefinition IsPopupVisited;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsLevelFocused = allocator.AllocateBoolItem();
				IsPopupVisited = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}

	public class HierarchicalMenuItemTemplateContract : MenuItemBaseTemplateContract
	{
		#region Properties

		[TemplateContractPart(Required = false)]
		public Panel SubmenuHost { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = false)]
		public Popup SubmenuPopup { get; [UsedImplicitly] private set; }

		#endregion
	}
}