// <copyright file="MenuStripItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(MenuStripItemsPresenterTemplateContract))]
	public class MenuStripItemsPresenter : MenuItemsPresenterBase<MenuItemBase, MenuItemsPanel>
	{
	}

	public sealed class MenuStripItemsPresenterTemplateContract : MenuItemsPresenterBaseTemplateContract<MenuItemBase, MenuItemsPanel>
	{
	}
}