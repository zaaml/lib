// <copyright file="ArtboardControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardControlTemplateContract : ItemsControlBaseTemplateContract<ArtboardItemsPresenter>
	{
		[TemplateContractPart(Required = true)]
		public ArtboardAdornerPresenter AdornerPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardGridLineControl GridLineControl { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardRuler HorizontalRuler { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardSnapGuidePresenter HorizontalSnapGuidePresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardScrollViewControl ScrollView { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardRuler VerticalRuler { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardSnapGuidePresenter VerticalSnapGuidePresenter { get; [UsedImplicitly] private set; }
	}
}