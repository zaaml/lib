// <copyright file="ListViewItemTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewItemTemplateContract : IconContentControlTemplateContract
	{
		[TemplateContractPart(Required = false)]
		public ListGridViewCellsPresenter GridViewCellsPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = false)]
		public ListViewItemGlyphPresenter GlyphPresenter { get; [UsedImplicitly] private set; }
	}
}