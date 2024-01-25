// <copyright file="ListGridViewCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewCellCollection
		: GridViewCellCollection<ListGridViewColumn, ListGridViewCellsPresenter,
			ListGridViewCellsPanel,
			ListGridViewCellCollection,
			ListGridViewCell>
	{
		public ListGridViewCellCollection(ListGridViewCellsPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}