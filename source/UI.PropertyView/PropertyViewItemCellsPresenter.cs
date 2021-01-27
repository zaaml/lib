// <copyright file="PropertyViewItemCellPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemCellsPresenter
		: GridCellsPresenter<PropertyViewItemCellsPresenter,
			PropertyViewItemCellsPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell>
	{
		public PropertyViewItemCellsPresenter()
		{
			AllowCellSplitter = true;
		}

		protected override PropertyViewItemCellCollection CreateCellCollection()
		{
			return new PropertyViewItemCellCollection(this);
		}

		protected override GridController Controller => (TemplatedParent as PropertyTreeViewItem)?.PropertyView?.ItemGridController;
	}
}