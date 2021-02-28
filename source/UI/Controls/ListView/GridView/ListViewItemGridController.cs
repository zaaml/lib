// <copyright file="ListViewItemGridController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemGridController : GridController
	{
		public ListViewItemGridController(ListViewControl listViewControl) : base(listViewControl)
		{
			ListViewControl = listViewControl;
		}

		protected override IEnumerable<GridCellsPresenter> CellsPresenters
		{
			get
			{
				var columnHeadersPresenter = ListViewControl.ColumnHeadersPresenterInternal;

				if (columnHeadersPresenter != null)
					yield return columnHeadersPresenter;

				var listViewPanel = ListViewControl.ItemsPresenterInternal?.ItemsHostInternal;

				if (listViewPanel == null)
					yield break;

				foreach (UIElement child in listViewPanel.Children)
				{
					if (child is ListViewItem listViewItem && listViewItem.CellsPresenterInternal != null)
						yield return listViewItem.CellsPresenterInternal;
				}
			}
		}

		protected override int ColumnCount => ListGridView?.Columns.Count ?? 0;

		protected override GridColumnWidthConstraints DefaultColumnWidthConstraints => ListGridView?.DefaultColumnWidthConstraints ?? base.DefaultColumnWidthConstraints;

		private ListGridView ListGridView => ListViewControl?.View as ListGridView;

		public ListViewControl ListViewControl { get; }

		public override GridColumn GetColumn(int index)
		{
			var columns = ListGridView?.Columns;

			if (columns == null)
				return null;

			return index >= 0 && index < columns.Count && columns.Count > 0 ? columns[index] : null;
		}
	}
}