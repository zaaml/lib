// <copyright file="ColorEditorControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.ColorEditor
{
	public sealed class ColorEditorControlTemplateContract : TemplateContract
	{
		[TemplateContractPart] public ColorRectangleEditorControl RectangleEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart] public ColorSliderEditorControl SliderEditor { get; [UsedImplicitly] private set; }
	}
}