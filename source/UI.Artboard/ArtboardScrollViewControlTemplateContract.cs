// <copyright file="ArtboardScrollViewControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardScrollViewControlTemplateContract : ScrollViewControlBaseTemplateContract<ArtboardScrollViewPresenter, ArtboardScrollViewPanel>
	{
		[TemplateContractPart(Required = true)]
		public ArtboardScrollViewPresenter ScrollViewPresenter { get; [UsedImplicitly] private set; }

		protected override ArtboardScrollViewPresenter ScrollViewPresenterCore => ScrollViewPresenter;
	}
}