// <copyright file="PropertyViewItemCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemCellCollection
		: GridCellCollection<PropertyViewItemCellPresenter,
			PropertyViewItemCellPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell,
			PropertyViewItemCellSplitter,
			PropertyViewItemCellColumnController,
			PropertyViewItemColumn>
	{
		public PropertyViewItemCellCollection(PropertyViewItemCellPresenter cellPresenter) : base(cellPresenter)
		{
		}
	}
}