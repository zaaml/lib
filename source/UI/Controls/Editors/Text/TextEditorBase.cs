// <copyright file="TextEditorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Editors.Core;

namespace Zaaml.UI.Controls.Editors.Text
{
	[TemplateContractType(typeof(TextEditorBaseTemplateContract))]
	public abstract class TextEditorBase : EditorBase
	{
	}

	public abstract class TextEditorBaseTemplateContract : EditorBaseTemplateContract
	{
	}
}