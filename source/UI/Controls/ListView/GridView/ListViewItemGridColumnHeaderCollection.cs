// <copyright file="ListViewItemGridColumnHeaderCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemGridColumnHeaderCollection
		: GridCellCollection<ListViewItemGridColumnHeadersPresenter,
			ListViewItemGridColumnHeadersPanel,
			ListViewItemGridColumnHeaderCollection,
			ListViewItemGridColumnHeader>
	{
		public ListViewItemGridColumnHeaderCollection(ListViewItemGridColumnHeadersPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}