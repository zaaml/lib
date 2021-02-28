// <copyright file="PropertyViewItemGridController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyViewItemGridController
		: GridController
	{
		public PropertyViewItemGridController(PropertyViewControl propertyView) : base(propertyView)
		{
			PropertyView = propertyView;
		}

		protected override IEnumerable<GridCellsPresenter> CellsPresenters
		{
			get { yield break; }
		}

		public PropertyViewControl PropertyView { get; }
	}
}