// <copyright file="PropertyViewItemColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemColumn
		: GridCellColumn<PropertyViewItemCellPresenter,
			PropertyViewItemCellPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell,
			PropertyViewItemCellSplitter,
			PropertyViewItemCellColumnController,
			PropertyViewItemColumn>
	{
		public PropertyViewItemColumn(PropertyViewItemCellColumnController controller, int index) : base(controller, index)
		{
		}
	}
}