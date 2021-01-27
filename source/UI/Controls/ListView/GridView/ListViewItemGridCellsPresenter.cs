// <copyright file="ListViewItemGridCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

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
		
		public ListViewItem ListViewItem
		{
			get => (ListViewItem) GetValue(ListViewItemProperty);
			internal set => this.SetReadOnlyValue(ListViewItemPropertyKey, value);
		}

		protected override ListViewItemGridCellCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override ListViewItemGridCell CreateCell()
		{
			return new ListViewItemGridCell { ListViewItem = ListViewItem };
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