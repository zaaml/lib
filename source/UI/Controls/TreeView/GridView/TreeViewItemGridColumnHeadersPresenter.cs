// <copyright file="TreeViewItemGridColumnHeadersPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemGridColumnHeadersPresenter
		: TreeViewItemGridElementPresenter<TreeViewItemGridColumnHeadersPresenter,
			TreeViewItemGridColumnHeadersPanel,
			TreeViewItemGridColumnHeaderCollection,
			TreeViewItemGridColumnHeader>
	{
		public TreeViewItemGridColumnHeadersPresenter()
		{
			AllowCellSplitter = true;
		}

		protected override TreeViewItemGridColumnHeader CreateCell()
		{
			return new TreeViewItemGridColumnHeader();
		}

		protected override TreeViewItemGridColumnHeaderCollection CreateCellCollection()
		{
			return new(this);
		}
	}
}