// <copyright file="PopupMenu.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(PopupMenuTemplateContract))]
	[ContentProperty(nameof(Items))]
	public partial class PopupMenu : PopupControlBase, IMenuBase
	{
		#region Ctors

		static PopupMenu()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PopupMenu>();
		}

		public PopupMenu()
		{
			this.OverrideStyleKey<PopupMenu>();

			PopupController.IsModalMenu = true;
			PopupController.CloseOnLostKeyboardFocus = true;
			PopupController.Placement = new MousePlacement();

			MenuController = new MenuController<PopupMenu>(this);
		}

		#endregion

		#region Properties

		internal override bool HandleFocus => true;

		internal MenuController<PopupMenu> MenuController { get; }

		private PopupMenuTemplateContract TemplateContract => (PopupMenuTemplateContract) TemplateContractInternal;

		#endregion

		#region  Methods

		private void EnsureOwner()
		{
			if (Owner == null && Parent is FrameworkElement freParent && GlobalPopup.IsAncestorOf(this) == false)
				Owner = freParent;
		}

		internal override void OnClosedInternal()
		{
			MenuController.OnClosed();

			base.OnClosedInternal();
		}

		internal override void OnClosingInternal(PopupCancelEventArgs e)
		{
			MenuController.OnClosing();

			base.OnClosingInternal(e);
		}

		internal override void OnIsOpenChangedInternal()
		{
			ItemsPresenter?.ResetScrollViewer();

			IsTabStop = IsOpen;

			base.OnIsOpenChangedInternal();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			MenuController.OnKeyDown(e);
		}

		protected override void OnMeasuring()
		{
			EnsureOwner();

			base.OnMeasuring();
		}

		internal override void OnOpenedInternal()
		{
			MenuController.OnOpened();

			base.OnOpenedInternal();
		}

		internal override void OnOpeningInternal(PopupCancelEventArgs e)
		{
			MenuController.OnOpening();

			base.OnOpeningInternal(e);
		}

		#endregion

		#region Interface Implementations

		#region IMenuBase

		bool IMenuBase.IsOpen
		{
			get => IsOpen;
			set => IsOpen = value;
		}

		MenuController IMenuBase.MenuController => MenuController;

		PopupControlController IMenuBase.PopupController => PopupController;

		#endregion

		#region IMenuItemOwner

		void IMenuItemOwner.AddLogicalChild(object menuItem)
		{
			LogicalChildMentor.AddLogicalChild(menuItem);
		}

		void IMenuItemOwner.RemoveLogicalChild(object menuItem)
		{
			LogicalChildMentor.RemoveLogicalChild(menuItem);
		}

		IMenuItemCollection IMenuItemOwner.Items => Items;

		IMenuItemOwner IMenuItemOwner.Owner => null;

		Orientation IMenuItemOwner.Orientation => Orientation.Vertical;

		void IMenuItemOwner.OnMenuItemAdded(MenuItemBase menuItem)
		{
		}

		void IMenuItemOwner.OnMenuItemRemoved(MenuItemBase menuItem)
		{
		}

		#endregion

		#endregion
	}

	public class PopupMenuTemplateContract : PopupControlBaseTemplateContract
	{
		#region Properties

		[TemplateContractPart(Required = false)]
		public MenuItemsPresenter ItemsPresenter { get; [UsedImplicitly] private set; }

		#endregion
	}
}