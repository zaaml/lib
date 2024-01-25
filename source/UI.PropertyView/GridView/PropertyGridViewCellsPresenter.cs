// <copyright file="PropertyViewItemCellPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyGridViewCellsPresenter
		: GridViewCellsPresenter<PropertyGridViewColumn, PropertyGridViewCellsPresenter,
			PropertyGridViewCellsPanel,
			PropertyGridViewCellCollection,
			PropertyGridViewCell>
	{
		public PropertyGridViewCellsPresenter()
		{
			AllowCellSplitter = true;
		}

		protected override PropertyGridViewCellCollection CreateCellCollection()
		{
			return new PropertyGridViewCellCollection(this);
		}

		protected override GridViewCellGenerator<PropertyGridViewCell> GetCellGenerator(PropertyGridViewColumn gridColumn)
		{
			throw new NotImplementedException();
		}

		protected override void AddCellGeneratorChanged(PropertyGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			throw new NotImplementedException();
		}

		protected override void RemoveCellGeneratorChanged(PropertyGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			throw new NotImplementedException();
		}

		protected override GridViewController ViewController => (TemplatedParent as PropertyTreeViewItem)?.PropertyViewControl?.ViewController;
	}
}