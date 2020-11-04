// <copyright file="ContextMenuSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	public abstract class ContextMenuSelector : InheritanceContextObject, IPopupControllerSelector, ISharedItem
	{
		#region Ctors

		protected ContextMenuSelector()
		{
			Owners = new SharedContextControlSelector(this);
		}

		#endregion

		#region Properties

		private SharedContextControlSelector Owners { get; }

		#endregion

		#region  Methods

		protected void RegisterContextMenu(ContextMenu menu)
		{
			Owners.RegisterSharedItem(menu);
		}

		public abstract ContextMenu Select(FrameworkElement menuOwner, DependencyObject eventSource);

		protected void UnregisterContextMenu(ContextMenu menu)
		{
			Owners.UnregisterSharedItem(menu);
		}

		#endregion

		#region Interface Implementations

		#region IPopupControllerSelector

		PopupControlController IPopupControllerSelector.SelectController(FrameworkElement frameworkElement, DependencyObject eventSource)
		{
			return Select(frameworkElement, eventSource)?.PopupController;
		}

		#endregion

		#region ISharedItem

		bool ISharedItem.IsShared { get; set; }

		SharedItemOwnerCollection ISharedItem.Owners => Owners;

		#endregion

		#endregion
	}
}