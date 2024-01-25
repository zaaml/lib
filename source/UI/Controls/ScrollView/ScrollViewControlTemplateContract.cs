// <copyright file="ScrollViewControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.ScrollView
{
	public sealed class ScrollViewControlTemplateContract : ScrollViewControlBaseTemplateContract<ScrollViewPresenter, ScrollViewPanel>
	{
		[TemplateContractPart(Required = true)]
		public ScrollViewPresenter ScrollViewPresenter { get; [UsedImplicitly] private set; }

		protected override ScrollViewPresenter ScrollViewPresenterCore => ScrollViewPresenter;
	}
}