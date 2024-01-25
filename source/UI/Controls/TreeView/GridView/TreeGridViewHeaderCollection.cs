// <copyright file="TreeGridViewHeaderCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewHeaderCollection
		: GridViewCellCollection<TreeGridViewColumn, TreeGridViewHeadersPresenter,
			TreeGridViewHeadersPanel,
			TreeGridViewHeaderCollection,
			TreeGridViewHeader>
	{
		public TreeGridViewHeaderCollection(TreeGridViewHeadersPresenter cellsPresenter) : base(cellsPresenter)
		{
		}
	}
}