// <copyright file="PropertyViewItemCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemCellCollection
		: GridCellCollection<PropertyViewItemCellsPresenter,
			PropertyViewItemCellsPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell>
	{
		public PropertyViewItemCellCollection(PropertyViewItemCellsPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}