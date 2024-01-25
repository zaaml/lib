// <copyright file="PropertyGridViewCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyGridViewCellCollection
		: GridViewCellCollection<PropertyGridViewColumn, PropertyGridViewCellsPresenter,
			PropertyGridViewCellsPanel,
			PropertyGridViewCellCollection,
			PropertyGridViewCell>
	{
		public PropertyGridViewCellCollection(PropertyGridViewCellsPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}