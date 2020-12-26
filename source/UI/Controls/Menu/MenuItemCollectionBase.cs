// <copyright file="MenuItemCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Menu
{
	public abstract class MenuItemCollectionBase<TMenuItem> : Core.ItemCollectionBase<Control, TMenuItem>, IMenuItemCollection where TMenuItem : MenuItemBase
	{
		#region Ctors

		internal MenuItemCollectionBase(Control menuItemOwner) : base(menuItemOwner)
		{
		}

		#endregion

		#region Properties

		internal MenuItemsPresenter Container { get; set; }

		internal IEnumerator LogicalChildren => this.Where(m => ReferenceEquals(MenuItemOwner, m.GetLogicalParent())).GetEnumerator();

		internal IMenuItemOwner MenuItemOwner => (IMenuItemOwner) Control;

		#endregion

		#region  Methods

		protected override void AttachLogicalCore(TMenuItem menuItem)
		{
			MenuItemOwner.AddLogicalChild(menuItem);
		}

		protected override void DetachLogicalCore(TMenuItem menuItem)
		{
			MenuItemOwner.RemoveLogicalChild(menuItem);
		}

		protected override void OnItemAttached(TMenuItem menuItem)
		{
			base.OnItemAttached(menuItem);

			MenuItemOwner.OnMenuItemAdded(menuItem);

			menuItem.Owner = MenuItemOwner;

			menuItem.OnAttached();
		}

		protected override void OnItemDetached(TMenuItem menuItem)
		{
			menuItem.OnDetached();

			menuItem.Owner = null;

			MenuItemOwner.OnMenuItemRemoved(menuItem);

			base.OnItemDetached(menuItem);
		}

		#endregion

		#region Interface Implementations

		#region IMenuItemCollection

		IEnumerable IMenuItemCollection.Source
		{
			get => SourceCollectionInternal;
			set => SourceCollectionInternal = value;
		}

		IEnumerator IMenuItemCollection.LogicalChildren => LogicalChildren;

		IEnumerator<MenuItemBase> IMenuItemCollection.GetEnumerator()
		{
			return ActualItemsInternal.GetEnumerator();
		}

		#endregion

		#endregion
	}
}