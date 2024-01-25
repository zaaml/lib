// <copyright file="TreeGridViewHeadersPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewHeadersPresenter
		: TreeGridViewElementsPresenter<TreeGridViewHeadersPresenter,
			TreeGridViewHeadersPanel,
			TreeGridViewHeaderCollection,
			TreeGridViewHeader>
	{
		public TreeGridViewHeadersPresenter()
		{
			AllowCellSplitter = true;
		}

		protected override void AddCellGeneratorChanged(TreeGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualHeaderGeneratorChanged += cellGeneratorChanged;
		}

		protected override TreeGridViewHeaderCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override GridViewCellGenerator<TreeGridViewHeader> GetCellGenerator(TreeGridViewColumn gridColumn)
		{
			return gridColumn.ActualHeaderGenerator;
		}

		protected override void RemoveCellGeneratorChanged(TreeGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualHeaderGeneratorChanged -= cellGeneratorChanged;
		}
	}
}