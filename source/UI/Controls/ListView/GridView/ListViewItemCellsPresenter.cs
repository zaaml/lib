// <copyright file="ListViewItemCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemCellsPresenter
		: GridCellsPresenter<ListViewItemCellsPresenter,
			ListViewItemCellsPanel,
			ListViewItemCellCollection,
			ListViewItemCell,
			ListViewItemCellSplitter,
			ListViewItemCellColumnController,
			ListViewItemCellColumn>
	{
		private static readonly DependencyPropertyKey ListViewItemPropertyKey = DPM.RegisterReadOnly<ListViewItem, ListViewItemCellsPresenter>
			("ListViewItem", d => d.OnListViewItemPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListViewItemCellsPresenter>
			("ListViewControl", d => d.OnListViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ViewPropertyKey = DPM.RegisterReadOnly<ListGridView, ListViewItemCellsPresenter>
			("View", d => d.OnViewPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ListViewControlProperty = ListViewControlPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ListViewItemProperty = ListViewItemPropertyKey.DependencyProperty;

		public ListViewItemCellsPresenter()
		{
			AllowCellSplitter = true;
		}

		private bool CellsDirty { get; set; }

		protected override ListViewItemCellColumnController ColumnController => ListGridView?.ItemCellColumnController;

		private ListGridView ListGridView => (TemplatedParent as ListViewItem)?.ListViewControl?.View as ListGridView;

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			private set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
		}

		public ListViewItem ListViewItem
		{
			get => (ListViewItem) GetValue(ListViewItemProperty);
			internal set => this.SetReadOnlyValue(ListViewItemPropertyKey, value);
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

		protected override ListViewItemCellCollection CreateCellCollection()
		{
			return new ListViewItemCellCollection(this);
		}

		private void CreateColumns(ListGridViewColumnCollection columns)
		{
			foreach (var column in columns)
			{
				var listViewItemCell = new ListViewItemCell();

				listViewItemCell.SetBinding(ListViewItemCell.ChildProperty, new Binding {Path = new PropertyPath(column.Member), Converter = CellContentConverter.Instance});

				Cells.Add(listViewItemCell);
			}
		}

		private void DetachView(ListGridView view)
		{
			view.Columns.Changed -= OnViewColumnsChanged;
		}

		private void EnsureColumns()
		{
			if (CellsDirty == false)
				return;

			Cells.Clear();

			var columns = View?.Columns;

			if (columns != null)
				CreateColumns(columns);

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

		private void UpdateListViewControl()
		{
			ListViewControl = ListViewItem?.ListViewControl;
		}

		private void UpdateView()
		{
			View = ListViewControl?.View as ListGridView;
		}

		private sealed class CellContentConverter : BaseValueConverter
		{
			private CellContentConverter()
			{
			}

			public static CellContentConverter Instance { get; } = new CellContentConverter();

			protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}

			protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (value is FrameworkElement fre)
					return fre;

				return new ContentPresenter { Content = value };
			}
		}
	}
}