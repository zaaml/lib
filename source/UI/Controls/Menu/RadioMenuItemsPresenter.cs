// <copyright file="RadioMenuItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(RadioMenuItemsPresenterTemplateContract))]
	public sealed class RadioMenuItemsPresenter : MenuItemsPresenterBase<RadioMenuItem, RadioMenuItemsPanel>
	{
		#region Ctors

		static RadioMenuItemsPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<RadioMenuItemsPresenter>();
		}

		public RadioMenuItemsPresenter()
		{
			this.OverrideStyleKey<RadioMenuItemsPresenter>();
		}

		#endregion
	}

	public sealed class RadioMenuItemsPresenterTemplateContract : MenuItemsPresenterBaseTemplateContract<RadioMenuItem, RadioMenuItemsPanel>
	{
	}
}