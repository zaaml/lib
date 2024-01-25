// <copyright file="ListGridViewHeadersPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewHeadersPanel
		: GridViewCellsPanel<ListGridViewColumn, ListGridViewHeadersPresenter,
			ListGridViewHeadersPanel,
			ListGridViewHeaderCollection,
			ListGridViewHeader>
	{
		public ListGridViewHeadersPanel()
		{
			FillElement = new ListGridViewHeaderElement(this);
		}

		protected override GridViewElement FillElement { get; }

		public ListGridView View => CellsPresenter?.View;
	}
}