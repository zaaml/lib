// <copyright file="TreeGridViewCellCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewCellCollection
		: GridViewCellCollection<TreeGridViewColumn, TreeGridViewCellsPresenter,
			TreeGridViewCellsPanel,
			TreeGridViewCellCollection,
			TreeGridViewCell>
	{
		public TreeGridViewCellCollection(TreeGridViewCellsPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}