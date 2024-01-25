// <copyright file="ListGridViewElementsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public abstract class ListGridViewElementsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		: GridViewCellsPresenter<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPresenter : GridViewCellsPresenter<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<ListGridViewColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListGridViewElementsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("ListViewControl", d => d.OnListViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ViewPropertyKey = DPM.RegisterReadOnly<ListGridView, ListGridViewElementsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("View", d => d.OnViewPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ListViewControlProperty = ListViewControlPropertyKey.DependencyProperty;

		public ListViewControl ListViewControl
		{
			get => (ListViewControl)GetValue(ListViewControlProperty);
			internal set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
		}

		public ListGridView View
		{
			get => (ListGridView)GetValue(ViewProperty);
			private set => this.SetReadOnlyValue(ViewPropertyKey, value);
		}

		protected override GridViewController ViewController => View?.ViewController;

		private void AttachView(ListGridView view)
		{
			Columns = view.Columns;
		}

		private void DetachView(ListGridView view)
		{
			Columns = null;
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

		private void OnViewPropertyChangedPrivate(ListGridView oldValue, ListGridView newValue)
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
			View = ListViewControl?.View as ListGridView;
		}
	}
}