// <copyright file="ListViewItemGridElementPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.ListView
{
	public abstract class ListViewItemGridElementPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		: GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPresenter : GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListViewItemGridElementPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("ListViewControl", d => d.OnListViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ViewPropertyKey = DPM.RegisterReadOnly<ListGridView, ListViewItemGridElementPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("View", d => d.OnViewPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ListViewControlProperty = ListViewControlPropertyKey.DependencyProperty;

		private bool CellsDirty { get; set; }

		private Stack<TGridCell> CellsPool { get; } = new();

		protected override GridController Controller => View?.GridController;

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			internal set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
		}

		public ListGridView View
		{
			get => (ListGridView) GetValue(ViewProperty);
			private set => this.SetReadOnlyValue(ViewPropertyKey, value);
		}

		private void AttachView(ListGridView view)
		{
			view.Columns.Changed += OnViewColumnsChanged;
		}

		protected abstract TGridCell CreateCell();

		private void CreateCells(ListGridViewColumnCollection columns)
		{
			DestroyCells();

			for (var i = 0; i < columns.Count; i++)
				Cells.Add(GetCell());
		}

		private void DestroyCells()
		{
			for (var index = Cells.Count - 1; index >= 0; index--)
				CellsPool.Push(Cells[index]);

			Cells.Clear();
		}

		private void DetachView(ListGridView view)
		{
			view.Columns.Changed -= OnViewColumnsChanged;
		}

		private void EnsureColumns()
		{
			if (CellsDirty == false)
				return;

			var columns = View?.Columns;

			if (columns != null)
				CreateCells(columns);
			else
				DestroyCells();

			CellsDirty = false;
		}

		private TGridCell GetCell()
		{
			return CellsPool.Count > 0 ? CellsPool.Pop() : CreateCell();
		}

		private void InvalidateCells()
		{
			CellsDirty = true;

			InvalidateMeasure();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			EnsureColumns();

			return base.MeasureOverride(availableSize);
		}

		private void OnListViewControlDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == ListViewControl.ViewProperty)
				UpdateView();
		}

		private void OnListViewControlPropertyChangedPrivate(ListViewControl oldValue, ListViewControl newValue)
		{
			if (oldValue != null) 
				oldValue.DependencyPropertyChangedInternal -= OnListViewControlDependencyPropertyChanged;

			if (newValue != null) 
				newValue.DependencyPropertyChangedInternal += OnListViewControlDependencyPropertyChanged;

			UpdateView();
		}

		private void OnViewColumnsChanged(object sender, EventArgs e)
		{
			CellsDirty = true;

			InvalidateMeasure();
		}

		private void OnViewPropertyChangedPrivate(ListGridView oldValue, ListGridView newValue)
		{
			if (oldValue != null)
				DetachView(oldValue);

			if (newValue != null)
				AttachView(newValue);

			InvalidateCells();
		}

		internal void UpdateStructure()
		{
			foreach (var cell in Cells)
				cell.UpdateStructureInternal(true);
		}

		private void UpdateView()
		{
			View = ListViewControl?.View as ListGridView;
		}
	}
}