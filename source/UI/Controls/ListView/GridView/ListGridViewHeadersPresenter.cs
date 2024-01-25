// <copyright file="ListGridViewHeadersPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewHeadersPresenter
		: ListGridViewElementsPresenter<ListGridViewHeadersPresenter,
			ListGridViewHeadersPanel,
			ListGridViewHeaderCollection,
			ListGridViewHeader>
	{
		public ListGridViewHeadersPresenter()
		{
			AllowCellSplitter = true;
		}

		protected override void AddCellGeneratorChanged(ListGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualHeaderGeneratorChanged += cellGeneratorChanged;
		}

		protected override ListGridViewHeaderCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override GridViewCellGenerator<ListGridViewHeader> GetCellGenerator(ListGridViewColumn gridColumn)
		{
			return gridColumn.ActualHeaderGenerator;
		}

		protected override void RemoveCellGeneratorChanged(ListGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualHeaderGeneratorChanged -= cellGeneratorChanged;
		}
	}
}