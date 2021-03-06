// <copyright file="ColorChannelSliderEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives.TrackBar;

namespace Zaaml.UI.Controls.ColorEditor
{
	public class ColorChannelSliderTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ColorRectangleRenderer ColorRectangleRenderer { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public TrackBarControl TrackBarControl { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public TrackBarValueItem TrackBarValueItem { get; [UsedImplicitly] private set; }
	}
}