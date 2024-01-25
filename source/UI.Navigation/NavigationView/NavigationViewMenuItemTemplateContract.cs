// <copyright file="NavigationViewMenuItemTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewMenuItemTemplateContract : NavigationViewHeaderedIconItemTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public NavigationViewSubItemsPresenter ItemsPresenter { get; [UsedImplicitly] private set; }
	}
}