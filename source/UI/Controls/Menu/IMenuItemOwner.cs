// <copyright file="IMenuItemOwner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Zaaml.UI.Controls.Menu
{
	internal interface IMenuItemCollection
	{
		#region Properties

		IEnumerator LogicalChildren { get; }

		IEnumerable Source { get; set; }

		#endregion

		#region  Methods

		IEnumerator<MenuItemBase> GetEnumerator();

		#endregion
	}

	internal interface IMenuItemOwner
	{
		#region Properties

		IMenuItemCollection ItemCollection { get; }

		Orientation Orientation { get; }

		IMenuItemOwner Owner { get; }

		#endregion

		#region  Methods

		void AddLogicalChild(object menuItem);

		void OnMenuItemAdded(MenuItemBase menuItem);

		void OnMenuItemRemoved(MenuItemBase menuItem);

		void RemoveLogicalChild(object menuItem);

		#endregion
	}
}