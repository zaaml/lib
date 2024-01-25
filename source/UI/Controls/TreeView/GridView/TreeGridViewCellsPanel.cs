// <copyright file="TreeGridViewCellsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewCellsPanel
		: GridViewCellsPanel<TreeGridViewColumn, TreeGridViewCellsPresenter,
			TreeGridViewCellsPanel,
			TreeGridViewCellCollection,
			TreeGridViewCell>
	{
		public TreeGridViewCellsPanel()
		{
			FillElement = new TreeGridViewCellElement(this);
		}

		protected override GridViewElement FillElement { get; }

		public TreeGridView View => CellsPresenter?.View;
	}
}