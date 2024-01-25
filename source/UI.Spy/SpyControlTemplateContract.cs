// <copyright file="SpyControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public SpyPropertyViewControl PropertyView { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public SpyVisualTreeViewControl VisualTree { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public SpyZoomControl ZoomControl { get; [UsedImplicitly] private set; }
	}
}