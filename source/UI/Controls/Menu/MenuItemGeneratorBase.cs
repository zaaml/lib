// <copyright file="MenuItemGeneratorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Menu
{
	public abstract class MenuItemGeneratorBase : ItemGenerator<MenuItemBase>
	{
		internal override MenuItemBase CreateItemCore(object source)
		{
			var menuItemBase = base.CreateItemCore(source);

			menuItemBase.ParentGenerator = this;

			return menuItemBase;
		}

		internal override void DisposeItemCore(MenuItemBase item, object source)
		{
			base.DisposeItemCore(item, source);

			item.ParentGenerator = null;
		}
	}
}