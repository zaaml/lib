// <copyright file="TreeGridViewCellTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewCellTemplateContract : GridCellTemplateContract
	{
		[TemplateContractPart(Required = false)]
		public TreeViewItemExpander Expander { get; [UsedImplicitly] private set; }
	}
}