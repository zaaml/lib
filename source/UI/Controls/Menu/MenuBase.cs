// <copyright file="MenuBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(MenuBaseTemplateContract))]
	[ContentProperty(nameof(Items))]
	public abstract partial class MenuBase : TemplateContractControl, IMenuBase
	{
		#region Ctors

		protected MenuBase()
		{
			MenuController = new MenuController<MenuBase>(this);
		}

		#endregion

		#region Properties

		internal abstract bool IsOpenCore { get; set; }

		internal MenuController<MenuBase> MenuController { get; }

		protected abstract Orientation Orientation { get; }

		internal virtual PopupControlController PopupController => null;

		#endregion

		#region  Methods

		protected override void OnKeyDown(KeyEventArgs e)
		{
			MenuController.OnKeyDown(e);
		}

		#endregion

		#region Interface Implementations

		#region IMenuBase

		bool IMenuBase.IsOpen
		{
			get => IsOpenCore;
			set => IsOpenCore = value;
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

		Orientation IMenuItemOwner.Orientation => Orientation;

		void IMenuItemOwner.OnMenuItemAdded(MenuItemBase menuItem)
		{
		}

		void IMenuItemOwner.OnMenuItemRemoved(MenuItemBase menuItem)
		{
		}

		#endregion

		#endregion
	}

	public abstract class MenuBaseTemplateContract : TemplateContract
	{
		#region Properties

		[TemplateContractPart(Required = false)]
		public MenuItemsPresenter ItemsPresenter { get; [UsedImplicitly] private set; }

		#endregion
	}
}