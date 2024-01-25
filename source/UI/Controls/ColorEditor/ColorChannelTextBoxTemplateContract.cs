// <copyright file="ColorChannelTextBoxTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Editors.Text;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.ColorEditor
{
	public class ColorChannelTextBoxTemplateContract : TextEditorTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public Label UnitLabel { get; [UsedImplicitly] private set; }
	}
}