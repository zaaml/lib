// <copyright file="NavigationViewFrameTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	public class NavigationViewFrameTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }
	}
}