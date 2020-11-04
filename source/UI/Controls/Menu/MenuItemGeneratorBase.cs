// <copyright file="MenuItemGeneratorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Menu
{
	public abstract class MenuItemGeneratorBase : ItemGenerator<MenuItemBase>
	{
		#region  Methods

		internal override MenuItemBase CreateItemCore(object itemSource)
		{
			var menuItemBase = base.CreateItemCore(itemSource);

			menuItemBase.ParentGenerator = this;

			return menuItemBase;
		}

		internal override void DisposeItemCore(MenuItemBase item, object itemSource)
		{
			base.DisposeItemCore(item, itemSource);

			item.ParentGenerator = null;
		}

		#endregion
	}
}