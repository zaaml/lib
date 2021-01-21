// <copyright file="PropertyViewItemCellPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemCellsPanel
		: GridCellsPanel<PropertyViewItemCellsPresenter,
			PropertyViewItemCellsPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell,
			PropertyViewItemCellSplitter,
			PropertyViewItemCellColumnController,
			PropertyViewItemColumn>
	{
		protected override PropertyViewItemCellSplitter CreateCellSplitter()
		{
			return new PropertyViewItemCellSplitter();
		}
	}
}