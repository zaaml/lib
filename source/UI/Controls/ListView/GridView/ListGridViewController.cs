// <copyright file="ListGridViewController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewController : GridViewController
	{
		public ListGridViewController(ListViewControl listViewControl) : base(listViewControl)
		{
			ListViewControl = listViewControl;
		}

		protected override IEnumerable<GridViewCellsPresenter> CellsPresenters
		{
			get
			{
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

		protected override GridViewCellsPresenter HeaderCellsPresenter => ListViewControl.GridViewHeadersPresenterInternal;

		public ListViewControl ListViewControl { get; }

		protected override GridViewColumnController CreateColumnController()
		{
			return new ListGridViewColumnController(this);
		}
	}
}