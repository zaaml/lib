// <copyright file="TreeViewItemTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewItemTemplateContract : IconContentControlTemplateContract
	{
		[TemplateContractPart(Required = false)]
		public TreeViewItemExpander Expander { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = false)]
		public TreeGridViewCellsPresenter CellsPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = false)]
		public TreeViewItemGlyphPresenter GlyphPresenter { get; [UsedImplicitly] private set; }
	}
}