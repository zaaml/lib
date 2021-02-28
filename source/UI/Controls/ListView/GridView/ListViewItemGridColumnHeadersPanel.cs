// <copyright file="ListViewItemGridColumnHeadersPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemGridColumnHeadersPanel
		: GridCellsPanel<ListViewItemGridColumnHeadersPresenter,
			ListViewItemGridColumnHeadersPanel,
			ListViewItemGridColumnHeaderCollection,
			ListViewItemGridColumnHeader>
	{
		protected override GridElement FillElement { get; } = new ListViewItemGridHeaderElement();
	}
}