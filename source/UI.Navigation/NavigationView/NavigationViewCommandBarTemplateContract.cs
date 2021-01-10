// <copyright file="NavigationViewCommandBarTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewCommandBarTemplateContract : NavigationViewItemBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public NavigationViewCommandItemsPresenter ItemsPresenter { get; [UsedImplicitly] private set; }
	}
}