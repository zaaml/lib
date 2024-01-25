// <copyright file="PropertyGridViewController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyGridViewController : GridViewController
	{
		public PropertyGridViewController(PropertyViewControl propertyView) : base(propertyView)
		{
			PropertyView = propertyView;
		}

		protected override IEnumerable<GridViewCellsPresenter> CellsPresenters
		{
			get { yield break; }
		}

		protected override GridViewCellsPresenter HeaderCellsPresenter => null;

		public PropertyViewControl PropertyView { get; }

		protected override GridViewColumnController CreateColumnController()
		{
			return new PropertyGridViewColumnController(this);
		}
	}
}