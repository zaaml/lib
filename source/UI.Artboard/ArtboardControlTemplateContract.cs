// <copyright file="ArtboardTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ArtboardAdornerPresenter AdornerPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardSnapGuidePresenter VerticalSnapGuidePresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardSnapGuidePresenter HorizontalSnapGuidePresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardDesignContentControl DesignBottomContentControl { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardDesignContentControl DesignTopContentControl { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardGridLineControl GridLineControl { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardRuler HorizontalRuler { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardPresenter Presenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardScrollViewControl ScrollView { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ArtboardRuler VerticalRuler { get; [UsedImplicitly] private set; }
	}
}