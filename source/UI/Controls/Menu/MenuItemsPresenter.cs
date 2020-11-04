// <copyright file="MenuItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(MenuItemsPresenterTemplateContract))]
	public sealed class MenuItemsPresenter : MenuItemsPresenterBase<MenuItemBase, MenuItemsPanel>
	{
		#region Ctors

		static MenuItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuItemsPresenter>();
		}

		public MenuItemsPresenter()
		{
			this.OverrideStyleKey<MenuItemsPresenter>();
		}

		#endregion
	}

	public sealed class MenuItemsPresenterTemplateContract : MenuItemsPresenterBaseTemplateContract<MenuItemBase, MenuItemsPanel>
	{
	}
}