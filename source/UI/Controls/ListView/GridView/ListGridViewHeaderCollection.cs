// <copyright file="ListGridViewHeaderCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewHeaderCollection
		: GridViewCellCollection<ListGridViewColumn, ListGridViewHeadersPresenter,
			ListGridViewHeadersPanel,
			ListGridViewHeaderCollection,
			ListGridViewHeader>
	{
		public ListGridViewHeaderCollection(ListGridViewHeadersPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}