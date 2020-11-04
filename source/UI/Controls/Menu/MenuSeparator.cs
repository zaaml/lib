// <copyright file="MenuSeparator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(MenuSeparatorTemplateContract))]
	public sealed class MenuSeparator : MenuItemBase
	{
		#region Ctors

		static MenuSeparator()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuSeparator>();
		}

		public MenuSeparator()
		{
			this.OverrideStyleKey<MenuSeparator>();
		}

		#endregion

		#region Properties

		internal override IMenuItemCollection ItemsCore => MenuItemCollection.Empty;

		#endregion
	}

	public sealed class MenuSeparatorTemplateContract : MenuItemBaseTemplateContract
	{
	}
}