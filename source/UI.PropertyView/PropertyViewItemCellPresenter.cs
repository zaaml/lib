// <copyright file="PropertyViewItemCellPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemCellPresenter
		: GridCellPresenter<PropertyViewItemCellPresenter,
			PropertyViewItemCellPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell,
			PropertyViewItemCellSplitter,
			PropertyViewItemCellColumnController,
			PropertyViewItemColumn>
	{
		public PropertyViewItemCellPresenter()
		{
			AllowCellSplitter = true;
		}

		protected override PropertyViewItemCellColumnController ColumnController => (TemplatedParent as PropertyTreeViewItem)?.PropertyView?.ItemCellColumnController;

		protected override PropertyViewItemCellCollection CreateCellCollection()
		{
			return new PropertyViewItemCellCollection(this);
		}
	}
}