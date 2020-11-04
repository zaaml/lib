// <copyright file="MenuItemsPanelBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Panels.Primitives;

namespace Zaaml.UI.Controls.Menu
{
	public class MenuItemsPanelBase<TMenuItem> : StackItemsPanelBase<TMenuItem>
		where TMenuItem : MenuItemBase
	{
	}
}