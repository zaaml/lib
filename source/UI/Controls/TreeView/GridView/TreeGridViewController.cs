// <copyright file="TreeGridViewController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewController : GridViewController
	{
		public TreeGridViewController(TreeViewControl treeViewControl) : base(treeViewControl)
		{
			TreeViewControl = treeViewControl;
		}

		protected override IEnumerable<GridViewCellsPresenter> CellsPresenters
		{
			get
			{
				var treeViewPanel = TreeViewControl.ItemsPresenterInternal?.ItemsHostInternal;

				if (treeViewPanel == null)
					yield break;

				foreach (UIElement child in treeViewPanel.Children)
				{
					if (child is TreeViewItem treeViewItem && treeViewItem.CellsPresenterInternal != null)
						yield return treeViewItem.CellsPresenterInternal;
				}
			}
		}

		protected override GridViewCellsPresenter HeaderCellsPresenter => TreeViewControl.ColumnHeadersPresenterInternal;

		public TreeViewControl TreeViewControl { get; }

		protected override GridViewColumnController CreateColumnController()
		{
			return new TreeGridViewColumnController(this);
		}
	}
}