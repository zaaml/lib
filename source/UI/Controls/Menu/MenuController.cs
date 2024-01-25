// <copyright file="MenuController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	internal abstract class MenuController
	{
		#region Static Fields and Constants

		public static readonly TimeSpan HoverDelay = TimeSpan.FromMilliseconds(200);
		public static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(10);
		private static readonly DependencyProperty MenuItemLevelProperty = DPM.RegisterAttached<MenuLevel, MenuController>("MenuItemLevel");

		#endregion

		#region Fields

		private HierarchicalMenuItem _focusedItem;
		private byte _packedValue;

		#endregion

		#region Properties

		internal HierarchicalMenuItem FocusedItem
		{
			get => _focusedItem;
			set
			{
				if (ReferenceEquals(_focusedItem, value))
				{
					UpdateFocusedItem();
					return;
				}

				var prevLevelSubmenuOpen = false;
				var prevLevel = _focusedItem != null ? GetMenuItemLevel(_focusedItem) : null;

				if (prevLevel != null)
				{
					prevLevelSubmenuOpen = prevLevel.OpenItem != null;
					prevLevel.OnItemLostFocus(_focusedItem);
				}

				_focusedItem = value;

				UpdateFocusedItem();

				if (_focusedItem != null && ReferenceEquals(FocusLevel, prevLevel) && prevLevelSubmenuOpen)
					FocusLevel.TriggerOpenPopup(_focusedItem, Delay);
			}
		}

		private MenuLevel FocusLevel { get; set; }

		private bool IsEnteredMenuMode
		{
			get => PackedDefinition.IsEnteredMenuMode.GetValue(_packedValue);
			set => PackedDefinition.IsEnteredMenuMode.SetValue(ref _packedValue, value);
		}

		private bool IsOpen
		{
			get => Menu.IsOpen;
			set => Menu.IsOpen = value;
		}

		protected abstract IMenuBase Menu { get; }

		protected abstract FrameworkElement MenuElement { get; }

		private HashSet<MenuLevel> OpenLevels { get; } = new HashSet<MenuLevel>();

		private MenuLevel RootLevel => EnsureLevel(Menu);

		private bool SuspendFocusHandler
		{
			get => PackedDefinition.SuspendFocusHandler.GetValue(_packedValue);
			set => PackedDefinition.SuspendFocusHandler.SetValue(ref _packedValue, value);
		}

		private static TreeEnumerationStrategy TreeStrategy => MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance;

		private int Version => Menu.PopupController?.Version ?? 0;

		#endregion

		#region  Methods

		private void Close()
		{
			foreach (var level in OpenLevels.ToList())
				level.Close();

			FocusedItem = null;
		}

		private void EnsureClosed(int version, DependencyObject menuItem)
		{
			if (Version != version)
				return;

			Close();

			PopupService.ClosePopupAncestors(menuItem, Menu.PopupController.Popup);
		}

		private static HierarchicalMenuItem EnsureHierarchical(MenuItemBase menuItem)
		{
			return menuItem as HierarchicalMenuItem ?? GetHierarchicalOwner(menuItem as MenuItemGroupBase);
		}

		private MenuLevel EnsureLevel(IMenuItemOwner menuOwner)
		{
			var level = (MenuLevel) ((DependencyObject) menuOwner).GetValue(MenuItemLevelProperty);

			if (ReferenceEquals(this, level?.Controller))
				return level;

			level = new MenuLevel(this, menuOwner);
			((DependencyObject) menuOwner).SetValue(MenuItemLevelProperty, level);

			return level;
		}

		private static HierarchicalMenuItem FindAncestorMenuItem(HierarchicalMenuItem menuItem, ref bool popupPassed)
		{
			var itemGroupParent = menuItem.Owner as MenuItemGroupBase;

			if (itemGroupParent != null)
			{
				var hierarchicalParent = GetHierarchicalOwner(itemGroupParent);
				if (hierarchicalParent != null)
				{
					popupPassed |= hierarchicalParent.SubmenuMode == SubmenuMode.Popup;

					return hierarchicalParent;
				}
			}

			var parentMenuItem = menuItem.ParentMenuItem;

			if (parentMenuItem != null)
			{
				popupPassed |= parentMenuItem.SubmenuMode == SubmenuMode.Popup;
				return parentMenuItem;
			}

			for (var ancestor = (FrameworkElement) menuItem.MenuBase ?? menuItem.GetParent<FrameworkElement>(TreeStrategy); ancestor != null; ancestor = ancestor.GetParent<FrameworkElement>(TreeStrategy))
			{
				popupPassed |= ancestor is Popup;

				var ancestorItem = ancestor as HierarchicalMenuItem;
				if (ancestorItem != null)
					return ancestorItem;
			}

			return null;
		}

		public static IMenuBase FindRootMenu(HierarchicalMenuItem menuItem)
		{
			var rootMenu = (FrameworkElement) menuItem.MenuBase;

			for (var ancestor = rootMenu.GetParent<FrameworkElement>(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance); ancestor != null; ancestor = ancestor.GetParent<FrameworkElement>(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance))
			{
				var menu = ancestor as IMenuBase ?? (ancestor as HierarchicalMenuItem)?.MenuBase;

				if (menu == null) continue;

				rootMenu = (FrameworkElement) menu;
				ancestor = rootMenu;
			}

			return (IMenuBase) rootMenu ?? GlobalMenu.Instance;
		}

		public static MenuController GetController(HierarchicalMenuItem menuItem)
		{
			return FindRootMenu(menuItem)?.MenuController;
		}

		private static HierarchicalMenuItem GetHierarchicalOwner(MenuItemGroupBase itemGroup)
		{
			while (itemGroup != null)
			{
				var hierarchicalParent = itemGroup.Owner as HierarchicalMenuItem;
				if (hierarchicalParent != null)
					return hierarchicalParent;

				itemGroup = itemGroup.Owner as MenuItemGroupBase;
			}

			return null;
		}

		private MenuLevel GetMenuItemLevel(MenuItemBase menuItem)
		{
			var popupPassed = false;

			var currentMenuItem = EnsureHierarchical(menuItem);
			HierarchicalMenuItem prevMenuItem = null;

			while (currentMenuItem != null && popupPassed == false)
			{
				prevMenuItem = currentMenuItem;
				currentMenuItem = FindAncestorMenuItem(currentMenuItem, ref popupPassed);
			}

			if (currentMenuItem != null)
				return EnsureLevel(currentMenuItem);

			var menu = FindRootMenu(prevMenuItem);
			return menu != null ? EnsureLevel(menu) : null;
		}

		private MenuLevel GetParentLevel(MenuLevel level)
		{
			if (ReferenceEquals(level, RootLevel))
				return null;

			var menuItemOwner = level.MenuOwner as HierarchicalMenuItem;
			var parentLevel = menuItemOwner != null ? GetMenuItemLevel(menuItemOwner) : RootLevel;

			return ReferenceEquals(parentLevel, level) ? null : parentLevel;
		}

		private MenuLevel GetSubmenuLevel(HierarchicalMenuItem menuItem)
		{
			return EnsureLevel(menuItem);
		}

		private bool IsTopLevelItem(HierarchicalMenuItem menuItem)
		{
			var headeredMenuItem = menuItem as HeaderedMenuItem;

			if (headeredMenuItem == null)
				return false;

			switch (headeredMenuItem.ActualRole)
			{
				case MenuItemRole.TopLevelItem:
				case MenuItemRole.TopLevelHeaderPopup:
				case MenuItemRole.TopLevelHeaderInline:
					return true;
				case MenuItemRole.SubmenuItem:
				case MenuItemRole.SubmenuHeaderPopup:
				case MenuItemRole.SubmenuHeaderInline:
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void OnClosed()
		{
			Close();
		}

		public void OnClosing()
		{
#if !SILVERLIGHT
			if (IsEnteredMenuMode == false)
				return;

			var presentationSource = PresentationSource.FromVisual(MenuElement);
			if (presentationSource != null)
				InputManager.Current.PopMenuMode(presentationSource);

			IsEnteredMenuMode = false;
#endif
		}

		public void OnHeaderGotKeyboardFocus(HierarchicalMenuItem menuItem)
		{
			if (SuspendFocusHandler == false)
				FocusedItem = menuItem;
		}

		public void OnHeaderMouseEnter(HierarchicalMenuItem menuItem)
		{
			if (IsOpen == false)
				return;

			OpenOnHover(menuItem);

			menuItem.IsPopupVisited = false;

			FocusedItem = menuItem;
		}

    public void OnHeaderMouseLeave(HeaderedMenuItem menuItem)
    {
	    if (IsOpen == false)
		    return;

			if (IsTopLevelItem(menuItem))
        return;

			var menuLevel = GetMenuItemLevel(menuItem);
			menuLevel.RevokeAction(ActionKind.Open, menuItem);

			if (menuItem.OpenSubmenuMode == OpenSubmenuMode.Hover)
			{
				if (menuItem.HasSubmenu && menuItem.SubmenuMode == SubmenuMode.Popup && menuItem.IsPopupVisited == false)
					menuLevel.TriggerClosePopup(menuItem, menuItem.OpenCloseDelay);
			}

			FocusedItem = null;
		}

		public void OnHeaderMouseLeftButtonDown(HierarchicalMenuItem menuItem)
		{
			if (menuItem.IsSubmenuOpen == false)
			{
				OpenOnClick(menuItem);

				menuItem.IsPopupVisited = false;

				FocusedItem = menuItem;
			}
			else
			{
				if (IsTopLevelItem(menuItem))
					IsOpen = false;
			}
		}

		public void OnHeaderMouseLeftButtonUp(HierarchicalMenuItem menuItem)
		{
		}

		public void OnKeyDown(KeyEventArgs e)
		{
			if (e.Handled || IsOpen == false)
				return;

			var vertical = (FocusedItem?.Owner ?? Menu).Orientation == Orientation.Vertical;

			switch (e.Key)
			{
				case Key.Up:
					SelectNext(vertical ? SelectDirection.Prev : SelectDirection.PrevLevel);
					e.Handled = true;
					break;
				case Key.Down:
					SelectNext(vertical ? SelectDirection.Next : SelectDirection.NextLevel);
					e.Handled = true;
					break;
				case Key.Right:
					SelectNext(vertical ? SelectDirection.NextLevel : SelectDirection.Next);
					e.Handled = true;
					break;
				case Key.Left:
					SelectNext(vertical ? SelectDirection.PrevLevel : SelectDirection.Prev);
					e.Handled = true;
					break;
				case Key.Home:
					SelectNext(SelectDirection.First);
					e.Handled = true;
					break;
				case Key.End:
					SelectNext(SelectDirection.Last);
					e.Handled = true;
					break;
				case Key.Escape:
					if (FocusLevel != null && GetParentLevel(FocusLevel) != null)
						SelectNext(SelectDirection.PrevLevel);
					else
						IsOpen = false;

					e.Handled = true;
					break;
				case Key.Enter:
					var submenuLevel = FocusedItem != null && FocusedItem.HasSubmenu ? GetSubmenuLevel(FocusedItem) : null;
					if (submenuLevel != null)
					{
						SelectNext(SelectDirection.NextLevel);
						e.Handled = true;
					}
					else if (FocusedItem?.ExecuteCommand() == true)
					{
						IsOpen = false;
						e.Handled = true;
					}

					break;
			}
		}

		public void OnMenuItemClick(MenuItem menuItem)
		{
			if (menuItem.StaysOpenOnClickInternal)
				return;

			var version = Version;

			MenuElement.Dispatcher.BeginInvoke(() => EnsureClosed(version, menuItem));
		}

		public void OnOpened()
		{
#if !SILVERLIGHT
			var presentationSource = PresentationSource.FromVisual(MenuElement);

			if (presentationSource == null)
				return;

			InputManager.Current.PushMenuMode(presentationSource);
			IsEnteredMenuMode = true;
#endif
		}

		public void OnOpening()
		{
			FocusedItem = null;
		}

		public void OnPopupMouseEnter(HierarchicalMenuItem menuItem)
		{
			menuItem.IsPopupVisited = true;
			GetMenuItemLevel(menuItem).RevokeAction(ActionKind.Close, menuItem);
		}

		public void OnPopupMouseLeave(HierarchicalMenuItem menuItem)
		{
		}

		public void OnSubmenuClosed(HierarchicalMenuItem menuItem)
		{
			menuItem.IsPopupVisited = false;
			menuItem.ResetPopupContent();
			menuItem.OnSubMenuClosedInt();

			GetMenuItemLevel(menuItem).OnSubmenuClosed(menuItem);

			menuItem.OnSubMenuClosedInt();
		}

		public void OnSubmenuOpenChanged(HierarchicalMenuItem menuItem)
		{
			if (menuItem.IsSubmenuOpen)
				OnSubmenuOpened(menuItem);
			else
				OnSubmenuClosed(menuItem);
		}

		public void OnSubmenuOpened(HierarchicalMenuItem menuItem)
		{
			IsOpen = true;

			GetMenuItemLevel(menuItem).OnSubmenuOpened(menuItem);

			menuItem.OnSubMenuOpenedInt();
		}

		private void OpenOnClick(HierarchicalMenuItem menuItem)
		{
			GetMenuItemLevel(menuItem).TriggerOpenPopup(menuItem, TimeSpan.FromMilliseconds(10));
		}

		private void OpenOnHover(HierarchicalMenuItem menuItem)
		{
			if (menuItem.OpenSubmenuMode != OpenSubmenuMode.Hover)
				return;

			if (IsTopLevelItem(menuItem) && IsOpen == false)
				return;

			GetMenuItemLevel(menuItem).TriggerOpenPopup(menuItem, menuItem.OpenCloseDelay);
		}

		private bool OpenTopLevelSubmenu(HierarchicalMenuItem menuItem, bool moveNext)
		{
			if (menuItem?.ParentMenuItem == null || IsTopLevelItem(menuItem.ParentMenuItem) == false)
				return false;

			var level = GetMenuItemLevel(menuItem);

			var parentLevel = GetParentLevel(level);

			if (parentLevel == null)
				return false;

			var focusState = parentLevel.GetFocusState();

			var next = moveNext ? focusState.Next : focusState.Prev;
			if (next == null)
				return false;

			if (parentLevel.OpenItem != null)
				parentLevel.TriggerClosePopup(parentLevel.OpenItem, TimeSpan.Zero);

			FocusedItem = next;

			if (next.HasSubmenu)
				parentLevel.TriggerOpenPopup(next, TimeSpan.Zero);

			return true;
		}

		private void SelectNext(SelectDirection mode)
		{
			var focusState = (FocusLevel ?? RootLevel).GetFocusState();

			switch (mode)
			{
				case SelectDirection.First:
					FocusedItem = focusState.First;
					break;
				case SelectDirection.Prev:
					FocusedItem = focusState.Prev ?? focusState.Last;
					break;
				case SelectDirection.Next:
					FocusedItem = focusState.Next ?? focusState.First;
					break;
				case SelectDirection.Last:
					FocusedItem = focusState.Last;
					break;

				case SelectDirection.PrevLevel:
					var prevLevel = FocusLevel != null ? GetParentLevel(FocusLevel) : null;

					if (prevLevel == null || ReferenceEquals(prevLevel, RootLevel))
					{
						if (OpenTopLevelSubmenu(FocusedItem, false))
							break;
					}

					var openItem = prevLevel?.OpenItem;

					if (openItem != null)
					{
						prevLevel.TriggerClosePopup(openItem, TimeSpan.Zero);
						FocusedItem = openItem;
					}
					else if (FocusLevel?.OpenItem != null)
					{
						openItem = FocusLevel.OpenItem;
						FocusLevel.TriggerClosePopup(openItem, TimeSpan.Zero);
						FocusedItem = openItem;
					}

					break;
				case SelectDirection.NextLevel:
					var focusedItem = FocusedItem;
					if (focusedItem == null)
					{
						var currentOpenItem = FocusLevel?.OpenItem;
						if (currentOpenItem != null && IsTopLevelItem(currentOpenItem))
							focusedItem = currentOpenItem;
					}

					if (focusedItem == null)
						break;

					var nextLevel = GetSubmenuLevel(focusedItem);
					var currentLevel = GetMenuItemLevel(focusedItem);

					if (focusedItem.HasSubmenu)
					{
						currentLevel.TriggerOpenPopup(focusedItem, TimeSpan.Zero);

						focusedItem.Submenu?.UpdateLayout();

						focusState = nextLevel.GetFocusState();

						if (focusState.First != null)
							FocusedItem = focusState.First;
						else
						{
							FocusLevel = nextLevel;
							var submenuElement = focusedItem.Submenu;
							if (submenuElement != null)
								FocusHelper.SetKeyboardFocusedElement(submenuElement);
						}
					}
					else
						OpenTopLevelSubmenu(focusedItem, true);

					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode));
			}
		}

		private void UpdateFocusedItem()
		{
			if (_focusedItem == null)
				return;

			try
			{
				SuspendFocusHandler = true;

				if (ReferenceEquals(FocusHelper.GetKeyboardFocusedElement(), _focusedItem) == false)
					FocusHelper.SetKeyboardFocusedElement(_focusedItem);

				var focusLevel = GetMenuItemLevel(_focusedItem);
				if (ReferenceEquals(focusLevel, FocusLevel) && ReferenceEquals(focusLevel.FocusedItem, _focusedItem))
					return;

				FocusLevel = focusLevel;
				FocusLevel.OnItemGotFocus(_focusedItem);
			}
			finally
			{
				SuspendFocusHandler = false;
			}
		}

		#endregion

		#region  Nested Types

		private struct FocusState
		{
			public FocusState(HierarchicalMenuItem current) : this()
			{
				_current = current;
			}

			public void ProceedItem(HierarchicalMenuItem menuItem)
			{
				var headeredMenuItem = menuItem as HeaderedMenuItem;

				if (headeredMenuItem?.ActualShowHeader == false)
					return;

				if (First == null)
					First = menuItem;

				if (ReferenceEquals(_current, menuItem))
					Prev = _prevProceed;

				if (ReferenceEquals(_current, _prevProceed))
					Next = menuItem;

				Last = menuItem;

				_prevProceed = menuItem;
			}

			public HierarchicalMenuItem First;
			public HierarchicalMenuItem Last;

			private HierarchicalMenuItem _prevProceed;
			private readonly HierarchicalMenuItem _current;
			public HierarchicalMenuItem Prev;
			public HierarchicalMenuItem Next;
		}

		internal class GlobalMenu : MenuBase
		{
			#region Static Fields and Constants

			public static readonly GlobalMenu Instance = new GlobalMenu();

			#endregion

			#region Ctors

			private GlobalMenu()
			{
			}

			#endregion

			#region Properties

			internal override bool IsOpenCore { get; set; }

			protected override Orientation Orientation => Orientation.Vertical;

			#endregion
		}

		private class MenuLevel
		{
			#region Fields

			private readonly Dictionary<HierarchicalMenuItem, DelayAction> _closeActions = new Dictionary<HierarchicalMenuItem, DelayAction>();
			private readonly Dictionary<HierarchicalMenuItem, DelayAction> _openActions = new Dictionary<HierarchicalMenuItem, DelayAction>();
			private HierarchicalMenuItem _focusedItem;
			private HierarchicalMenuItem _openItem;

			#endregion

			#region Ctors

			public MenuLevel(MenuController controller, IMenuItemOwner menuOwner)
			{
				Controller = controller;
				MenuOwner = menuOwner;
			}

			#endregion

			#region Properties

			public MenuController Controller { get; }

			public HierarchicalMenuItem FocusedItem
			{
				get => _focusedItem;
				private set
				{
					if (ReferenceEquals(_focusedItem, value))
						return;

					if (_focusedItem != null)
						_focusedItem.IsLevelFocused = false;

					_focusedItem = value;

					if (_focusedItem != null)
						_focusedItem.IsLevelFocused = true;
				}
			}

			public IMenuItemOwner MenuOwner { get; }

			public HierarchicalMenuItem OpenItem
			{
				get => _openItem;
				private set
				{
					if (ReferenceEquals(_openItem, value))
						return;

					if (_openItem != null)
						_openItem.IsSubmenuOpen = false;

					_openItem = value;

					if (_openItem == null)
						Controller.OpenLevels.Remove(this);
					else
					{
						Controller.OpenLevels.Add(this);
						FocusedItem = value;
					}
				}
			}

			#endregion

			#region  Methods

			public void Close()
			{
				FocusedItem = null;
				OpenItem = null;
			}

			private static void CloseSubmenu(HierarchicalMenuItem menuItem)
			{
				if (menuItem.IsSubmenuOpen == false)
					return;

				menuItem.IsSubmenuOpen = false;
			}

			private Dictionary<HierarchicalMenuItem, DelayAction> GetActions(ActionKind actionKind)
			{
				return actionKind == ActionKind.Open ? _openActions : _closeActions;
			}

			public FocusState GetFocusState()
			{
				var focusState = new FocusState(FocusedItem ?? OpenItem);

				var headeredMenuItem = MenuOwner as HierarchicalMenuItem;
				var submenu = headeredMenuItem?.Submenu;
				if (submenu != null)
					foreach (var menuItem in submenu.GetVisualDescendants().OfType<HierarchicalMenuItem>())
						ProceedFocus(menuItem, ref focusState);

				// TODO Inspect this (Review History before MenuItemGroupBase)
				foreach (var menuItem in MenuOwner.ItemCollection)
					ProceedFocus(menuItem, ref focusState);

				return focusState;
			}

			private void InvokeAction(ActionKind kind, TimeSpan delay, HierarchicalMenuItem menuItem)
			{
				GetActions(kind).GetValueOrCreate(menuItem, () => new DelayAction(() => InvokeActionImpl(kind, menuItem))).Invoke(delay);
			}

			private void InvokeActionImpl(ActionKind kind, HierarchicalMenuItem menuItem)
			{
				if (kind == ActionKind.Open)
					OpenSubmenu(menuItem);
				else
					CloseSubmenu(menuItem);

				GetActions(kind).Remove(menuItem);
			}

			public void OnItemGotFocus(HierarchicalMenuItem menuItem)
			{
				FocusedItem = menuItem;

				var current = this;
				while (current != null)
				{
					current = Controller.GetParentLevel(current);
					if (current != null)
						current.FocusedItem = current.OpenItem;
				}
			}

			public void OnItemLostFocus(HierarchicalMenuItem menuItem)
			{
				if (ReferenceEquals(FocusedItem, menuItem) && ReferenceEquals(OpenItem, menuItem) == false)
					FocusedItem = null;
			}

			public void OnSubmenuClosed(HierarchicalMenuItem menuItem)
			{
				RevokeAction(ActionKind.Close, menuItem);
				RevokeAction(ActionKind.Open, menuItem);

				if (ReferenceEquals(OpenItem, menuItem))
					OpenItem = null;

				if (ReferenceEquals(FocusedItem, menuItem))
					FocusedItem = null;
			}

			public void OnSubmenuOpened(HierarchicalMenuItem menuItem)
			{
				RevokeAction(ActionKind.Close, menuItem);
				RevokeAction(ActionKind.Open, menuItem);

				OpenItem = menuItem;
			}

			private void OpenSubmenu(HierarchicalMenuItem menuItem)
			{
				if (menuItem.IsSubmenuOpen)
					return;

				Controller.IsOpen = true;

				menuItem.IsSubmenuOpen = true;
			}

			private void ProceedFocus(MenuItemBase menuItem, ref FocusState focusState)
			{
				var hierarchical = EnsureHierarchical(menuItem);
				if (hierarchical != null && ReferenceEquals(Controller.GetMenuItemLevel(hierarchical), this))
					focusState.ProceedItem(hierarchical);

				// TODO Inspect this (Review History before MenuItemGroupBase)
				var itemGroup = menuItem as MenuItemGroupBase;
				if (itemGroup != null)
					foreach (var item in itemGroup.ItemsCore)
						ProceedFocus(item, ref focusState);

				if (hierarchical == null)
					return;

				if (hierarchical.SubmenuMode == SubmenuMode.Popup)
					return;

				foreach (var descendant in menuItem.GetVisualDescendants().OfType<HierarchicalMenuItem>())
					if (ReferenceEquals(Controller.GetMenuItemLevel(descendant), this))
						focusState.ProceedItem(descendant);
			}

			public void RevokeAction(ActionKind kind, HierarchicalMenuItem menuItem)
			{
				var actions = GetActions(kind);
				var action = actions.GetValueOrDefault(menuItem);

				if (action == null) return;

				action.Revoke();
				actions.Remove(menuItem);
			}

			public void TriggerClosePopup(HierarchicalMenuItem menuItem, TimeSpan timeSpan)
			{
				RevokeAction(ActionKind.Open, menuItem);
				InvokeAction(ActionKind.Close, timeSpan, menuItem);
			}

			public void TriggerOpenPopup(HierarchicalMenuItem menuItem, TimeSpan timeSpan)
			{
				RevokeAction(ActionKind.Close, menuItem);

				foreach (var action in GetActions(ActionKind.Open).Values)
					action.Revoke();

				if (OpenItem != null && ReferenceEquals(OpenItem, menuItem) == false)
				{
					OpenItem.IsPopupVisited = false;
					TriggerClosePopup(OpenItem, OpenItem.OpenCloseDelay);
				}

				if (menuItem.IsSubmenuOpen == false && menuItem.HasSubmenu && menuItem.SubmenuMode == SubmenuMode.Popup)
					InvokeAction(ActionKind.Open, timeSpan, menuItem);
			}

			#endregion
		}

		private enum ActionKind
		{
			Open,
			Close
		}

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsEnteredMenuMode;
			public static readonly PackedBoolItemDefinition SuspendFocusHandler;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsEnteredMenuMode = allocator.AllocateBoolItem();
				SuspendFocusHandler = allocator.AllocateBoolItem();
			}

			#endregion
		}

		private enum SelectDirection
		{
			First,
			Prev,
			Next,
			Last,
			NextLevel,
			PrevLevel
		}

		#endregion
	}

	internal sealed class MenuController<TMenu> : MenuController where TMenu : FrameworkElement, IMenuBase
	{
		#region Fields

		private readonly TMenu _menu;

		#endregion

		#region Ctors

		public MenuController(TMenu menu)
		{
			_menu = menu;

#if !SILVERLIGHT
			KeyboardNavigation.SetTabNavigation(_menu, KeyboardNavigationMode.None);
			KeyboardNavigation.SetControlTabNavigation(_menu, KeyboardNavigationMode.None);
#endif
		}

		#endregion

		#region Properties

		protected override IMenuBase Menu => _menu;

		protected override FrameworkElement MenuElement => _menu;

		#endregion
	}
}