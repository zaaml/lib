// <copyright file="ListGridViewCellsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewCellsPanel
		: GridViewCellsPanel<ListGridViewColumn, ListGridViewCellsPresenter,
			ListGridViewCellsPanel,
			ListGridViewCellCollection,
			ListGridViewCell>
	{
		public ListGridViewCellsPanel()
		{
			FillElement = new ListGridViewCellElement(this);
		}

		protected override GridViewElement FillElement { get; }

		public ListGridView View => CellsPresenter?.View;
	}
}