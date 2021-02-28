// <copyright file="TreeViewItemGridElementPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeViewItemGridElementPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		: GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPresenter : GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		private static readonly DependencyPropertyKey TreeViewControlPropertyKey = DPM.RegisterReadOnly<TreeViewControl, TreeViewItemGridElementPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("TreeViewControl", d => d.OnTreeViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ViewPropertyKey = DPM.RegisterReadOnly<TreeGridView, TreeViewItemGridElementPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("View", d => d.OnViewPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TreeViewControlProperty = TreeViewControlPropertyKey.DependencyProperty;

		private bool CellsDirty { get; set; }

		private Stack<TGridCell> CellsPool { get; } = new();

		protected abstract TGridCell CreateCell();

		private TGridCell GetCell()
		{
			return CellsPool.Count > 0 ? CellsPool.Pop() : CreateCell();
		}

		private void CreateCells(TreeGridViewColumnCollection columns)
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

		protected override GridController Controller => View?.GridController;

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl) GetValue(TreeViewControlProperty);
			internal set => this.SetReadOnlyValue(TreeViewControlPropertyKey, value);
		}

		public TreeGridView View
		{
			get => (TreeGridView) GetValue(ViewProperty);
			private set => this.SetReadOnlyValue(ViewPropertyKey, value);
		}

		private void AttachView(TreeGridView view)
		{
			view.Columns.Changed += OnViewColumnsChanged;
		}

		private void DetachView(TreeGridView view)
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

		private void OnTreeViewControlDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == TreeViewControl.ViewProperty)
				UpdateView();
		}

		private void OnTreeViewControlPropertyChangedPrivate(TreeViewControl oldValue, TreeViewControl newValue)
		{
			if (oldValue != null)
				oldValue.DependencyPropertyChangedInternal -= OnTreeViewControlDependencyPropertyChanged;

			if (newValue != null)
				newValue.DependencyPropertyChangedInternal += OnTreeViewControlDependencyPropertyChanged;

			UpdateView();
		}

		private void OnViewColumnsChanged(object sender, EventArgs e)
		{
			CellsDirty = true;

			InvalidateMeasure();
		}

		private void OnViewPropertyChangedPrivate(TreeGridView oldValue, TreeGridView newValue)
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
			View = TreeViewControl?.View as TreeGridView;
		}
	}
}