// <copyright file="TextEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Editors.Text
{
	[TemplateContractType(typeof(TextEditorTemplateContract))]
	public class TextEditor : TextEditorBase
	{
	}

	public class TextEditorTemplateContract : TextEditorBaseTemplateContract
	{
	}
}