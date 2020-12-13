// <copyright file="MenuItemBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(MenuItemBaseTemplateContract))]
	public abstract class MenuItemBase : TemplateContractControl, IMenuItemOwner
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey OwnerPropertyKey = DPM.RegisterReadOnly<IMenuItemOwner, MenuItemBase>
			("OwnerInt");

		#endregion

		#region Fields

		private IMenuItemOwner _owner;
		private MenuItemGeneratorBase _parentGenerator;

		#endregion

		#region Ctors

		internal MenuItemBase()
		{
		}

		#endregion

		#region Properties

		internal abstract IMenuItemCollection ItemsCore { get; }

		protected override IEnumerator LogicalChildren => ItemsCore != null ? ItemsCore.LogicalChildren : base.LogicalChildren;

		internal virtual IMenuItemOwner Owner
		{
			get => _owner;
			set
			{
				if (ReferenceEquals(_owner, value))
					return;

				var oldOwner = _owner;

				_owner = value;

				OwnerInt = value;

				OnOwnerChangedInternal(oldOwner, value);
			}
		}

		public Control Menu
		{
			get
			{
				var current = (IMenuItemOwner)this;

				while (current.Owner != null) 
					current = current.Owner;

				return current as Control;
			}
		}

		private IMenuItemOwner OwnerInt
		{
			set => this.SetReadOnlyValue(OwnerPropertyKey, value);
		}

		internal MenuItemGeneratorBase ParentGenerator
		{
			get => _parentGenerator;
			set
			{
				if (ReferenceEquals(_parentGenerator, value))
					return;

				_parentGenerator = value;

				OnParentGeneratorChanged();
			}
		}

		internal virtual bool StaysOpenOnClickInternal => false;

		#endregion

		#region  Methods

		internal virtual void OnAttached()
		{
		}

		internal virtual void OnDetached()
		{
		}

		protected virtual void OnMenuItemAdded(MenuItemBase menuItem)
		{
		}

		private void OnMenuItemAddedCore(MenuItemBase menuItem)
		{
			OnMenuItemAdded(menuItem);
			OnMenuItemsChanged();
		}

		protected virtual void OnMenuItemRemoved(MenuItemBase menuItem)
		{
		}

		private void OnMenuItemRemovedCore(MenuItemBase menuItem)
		{
			OnMenuItemRemoved(menuItem);
			OnMenuItemsChanged();
		}

		protected virtual void OnMenuItemsChanged()
		{
		}

		internal virtual void OnOwnerChangedInternal(IMenuItemOwner oldOwner, IMenuItemOwner newOwner)
		{
		}

		internal virtual void OnParentGeneratorChanged()
		{
		}

		#endregion

		#region Interface Implementations

		#region IMenuItemOwner

		IMenuItemCollection IMenuItemOwner.Items => ItemsCore;

		IMenuItemOwner IMenuItemOwner.Owner => Owner;

		void IMenuItemOwner.OnMenuItemAdded(MenuItemBase menuItem) => OnMenuItemAddedCore(menuItem);

		void IMenuItemOwner.OnMenuItemRemoved(MenuItemBase menuItem) => OnMenuItemRemovedCore(menuItem);

		Orientation IMenuItemOwner.Orientation => Orientation.Vertical;

		void IMenuItemOwner.AddLogicalChild(object menuItem)
		{
			LogicalChildMentor.AddLogicalChild(menuItem);
		}

		void IMenuItemOwner.RemoveLogicalChild(object menuItem)
		{
			LogicalChildMentor.RemoveLogicalChild(menuItem);
		}

		#endregion

		#endregion
	}

	public abstract class MenuItemBaseTemplateContract : TemplateContract
	{
	}
}