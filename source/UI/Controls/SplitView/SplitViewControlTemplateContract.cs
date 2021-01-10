// <copyright file="SplitViewControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.SplitView
{
	public class SplitViewControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public SplitViewContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public Panel PanePanel { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public SplitViewPanePresenter PanePresenter { get; [UsedImplicitly] private set; }
	}
}