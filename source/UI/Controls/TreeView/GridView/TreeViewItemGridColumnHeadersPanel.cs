// <copyright file="TreeViewItemGridColumnHeadersPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemGridColumnHeadersPanel
		: GridCellsPanel<TreeViewItemGridColumnHeadersPresenter,
			TreeViewItemGridColumnHeadersPanel,
			TreeViewItemGridColumnHeaderCollection,
			TreeViewItemGridColumnHeader>
	{
		protected override GridElement FillElement { get; } = new TreeViewItemGridHeaderElement();
	}
}