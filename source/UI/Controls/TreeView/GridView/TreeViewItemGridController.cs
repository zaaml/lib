// <copyright file="TreeViewItemGridController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemGridController
		: GridController
	{
		public TreeViewItemGridController(TreeViewControl treeViewControl) : base(treeViewControl)
		{
			TreeViewControl = treeViewControl;
		}

		protected override IEnumerable<GridCellsPresenter> CellsPresenters
		{
			get
			{
				var columnHeadersPresenter = TreeViewControl.ColumnHeadersPresenterInternal;

				if (columnHeadersPresenter != null)
					yield return columnHeadersPresenter;

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

		protected override int ColumnCount => TreeGridView?.Columns.Count ?? 0;

		private TreeGridView TreeGridView => TreeViewControl?.View as TreeGridView;

		public TreeViewControl TreeViewControl { get; }

		public override GridColumn GetColumn(int index)
		{
			var columns = TreeGridView?.Columns;

			if (columns == null)
				return null;

			return index >= 0 && index < columns.Count && columns.Count > 0 ? columns[index] : null;
		}
	}
}