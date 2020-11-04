// <copyright file="PropertyViewItemCellColumnController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemCellColumnController
		: GridCellColumnController<PropertyViewItemCellPresenter,
			PropertyViewItemCellPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell,
			PropertyViewItemCellSplitter,
			PropertyViewItemCellColumnController,
			PropertyViewItemColumn>
	{
		public PropertyViewItemCellColumnController(PropertyViewControl propertyView)
		{
			PropertyView = propertyView;
		}

		public PropertyViewControl PropertyView { get; }

		protected override PropertyViewItemColumn CreateColumn(int index)
		{
			return new PropertyViewItemColumn(this, index);
		}
	}
}