// <copyright file="TreeGridViewElementsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeGridViewElementsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		: GridViewCellsPresenter<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPresenter : GridViewCellsPresenter<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<TreeGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		private static readonly DependencyPropertyKey TreeViewControlPropertyKey = DPM.RegisterReadOnly<TreeViewControl, TreeGridViewElementsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("TreeViewControl", d => d.OnTreeViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ViewPropertyKey = DPM.RegisterReadOnly<TreeGridView, TreeGridViewElementsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("View", d => d.OnViewPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TreeViewControlProperty = TreeViewControlPropertyKey.DependencyProperty;

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl)GetValue(TreeViewControlProperty);
			internal set => this.SetReadOnlyValue(TreeViewControlPropertyKey, value);
		}

		public TreeGridView View
		{
			get => (TreeGridView)GetValue(ViewProperty);
			private set => this.SetReadOnlyValue(ViewPropertyKey, value);
		}

		protected override GridViewController ViewController => View?.ViewController;

		private void AttachView(TreeGridView view)
		{
			Columns = view.Columns;
		}

		private void DetachView(TreeGridView view)
		{
			Columns = null;
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

		private void OnViewPropertyChangedPrivate(TreeGridView oldValue, TreeGridView newValue)
		{
			if (oldValue != null)
				DetachView(oldValue);

			if (newValue != null)
				AttachView(newValue);
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