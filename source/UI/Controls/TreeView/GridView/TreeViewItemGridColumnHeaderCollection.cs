// <copyright file="TreeViewItemGridColumnHeaderCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemGridColumnHeaderCollection
		: GridCellCollection<TreeViewItemGridColumnHeadersPresenter,
			TreeViewItemGridColumnHeadersPanel,
			TreeViewItemGridColumnHeaderCollection,
			TreeViewItemGridColumnHeader>
	{
		public TreeViewItemGridColumnHeaderCollection(TreeViewItemGridColumnHeadersPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}