// <copyright file="TreeGridViewHeadersPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewHeadersPanel
		: GridViewCellsPanel<TreeGridViewColumn, TreeGridViewHeadersPresenter,
			TreeGridViewHeadersPanel,
			TreeGridViewHeaderCollection,
			TreeGridViewHeader>
	{
		public TreeGridViewHeadersPanel()
		{
			FillElement = new TreeGridViewHeaderElement(this);
		}

		protected override GridViewElement FillElement { get; }

		public TreeGridView View => CellsPresenter?.View;
	}
}