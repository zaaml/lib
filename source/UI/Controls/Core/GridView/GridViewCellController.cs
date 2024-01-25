// <copyright file="GridViewCellController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core.GridView
{
	public abstract class GridViewCellController
	{
		protected GridViewCellController(GridViewCell gridCell)
		{
			GridCellCore = gridCell;
		}


		protected GridViewCell GridCellCore { get; }
	}
}