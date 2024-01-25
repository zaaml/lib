// <copyright file="PropertyViewItemTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyViewItemTemplateContract : TreeViewItemTemplateContract
	{
		[TemplateContractPart] public ValidationErrorControl ValidationErrorControl { get; [UsedImplicitly] private set; }
	}
}