// <copyright file="NavigationViewHeaderedIconItemTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewHeaderedIconItemTemplateContract : NavigationViewItemBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public NavigationViewHeaderedIconItemPresenter HeaderedIconItemPresenter { get; [UsedImplicitly] private set; }
	}
}