// <copyright file="ColorRectangleEditorControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.ColorEditor
{
	public class ColorRectangleEditorControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ColorRectangleRenderer ColorRectangleRenderer { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public XYController XYController { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public XYControllerItem XYControllerItem { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelSlider ZChannelSlider { get; [UsedImplicitly] private set; }
	}
}