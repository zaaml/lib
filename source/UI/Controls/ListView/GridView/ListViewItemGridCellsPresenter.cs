// <copyright file="ListViewItemGridCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemGridCellsPresenter
		: ListViewItemGridElementPresenter<ListViewItemGridCellsPresenter,
			ListViewItemGridCellsPanel,
			ListViewItemGridCellCollection,
			ListViewItemGridCell>
	{
		private static readonly DependencyPropertyKey ListViewItemPropertyKey = DPM.RegisterReadOnly<ListViewItem, ListViewItemGridCellsPresenter>
			("ListViewItem", d => d.OnListViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewItemProperty = ListViewItemPropertyKey.DependencyProperty;

		public ListViewItemGridCellsPresenter()
		{
			AllowCellSplitter = false;
		}

		private Stack<ListViewItemGridCell> CellsPool { get; } = new();

		public ListViewItem ListViewItem
		{
			get => (ListViewItem) GetValue(ListViewItemProperty);
			internal set => this.SetReadOnlyValue(ListViewItemPropertyKey, value);
		}

		protected override ListViewItemGridCellCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override void CreateCells(ListGridViewColumnCollection columns)
		{
			DestroyCells();

			for (var index = 0; index < columns.Count; index++)
				Cells.Add(GetCell());
		}

		protected override void DestroyCells()
		{
			for (var index = Cells.Count - 1; index >= 0; index--)
				CellsPool.Push(Cells[index]);

			Cells.Clear();
		}

		private ListViewItemGridCell GetCell()
		{
			if (CellsPool.Count > 0)
				return CellsPool.Pop();

			return new ListViewItemGridCell();
		}

		private void OnListViewItemPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == ListViewItem.ListViewControlProperty)
				UpdateListViewControl();
		}

		private void OnListViewItemPropertyChangedPrivate(ListViewItem oldValue, ListViewItem newValue)
		{
			if (oldValue != null)
				oldValue.DependencyPropertyChangedInternal -= OnListViewItemPropertyChanged;

			if (newValue != null)
				newValue.DependencyPropertyChangedInternal += OnListViewItemPropertyChanged;

			UpdateListViewControl();
		}

		private void UpdateListViewControl()
		{
			ListViewControl = ListViewItem?.ListViewControl;
		}
	}
}