// <copyright file="PropertyViewItemCellSplitter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemCellSplitter
		: GridCellSplitter<PropertyViewItemCellPresenter,
			PropertyViewItemCellPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell,
			PropertyViewItemCellSplitter,
			PropertyViewItemCellColumnController,
			PropertyViewItemColumn>
	{
	}
}