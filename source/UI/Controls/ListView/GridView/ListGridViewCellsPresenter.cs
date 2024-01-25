// <copyright file="ListGridViewCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewCellsPresenter
		: ListGridViewElementsPresenter<ListGridViewCellsPresenter,
			ListGridViewCellsPanel,
			ListGridViewCellCollection,
			ListGridViewCell>
	{
		private static readonly DependencyPropertyKey ListViewItemPropertyKey = DPM.RegisterReadOnly<ListViewItem, ListGridViewCellsPresenter>
			("ListViewItem", d => d.OnListViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewItemProperty = ListViewItemPropertyKey.DependencyProperty;

		public ListGridViewCellsPresenter()
		{
			AllowCellSplitter = false;
		}

		public ListViewItem ListViewItem
		{
			get => (ListViewItem)GetValue(ListViewItemProperty);
			internal set => this.SetReadOnlyValue(ListViewItemPropertyKey, value);
		}

		protected override void AddCellGeneratorChanged(ListGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualCellGeneratorChanged += cellGeneratorChanged;
		}

		protected override void AttachCell(ListGridViewCell gridCell)
		{
			base.AttachCell(gridCell);

			gridCell.ListViewItem = ListViewItem;
		}

		protected override ListGridViewCellCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override void DetachCell(ListGridViewCell gridCell)
		{
			gridCell.ListViewItem = null;

			base.DetachCell(gridCell);
		}

		protected override GridViewCellGenerator<ListGridViewCell> GetCellGenerator(ListGridViewColumn gridColumn)
		{
			return gridColumn.ActualCellGenerator;
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

		protected override void RemoveCellGeneratorChanged(ListGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualCellGeneratorChanged -= cellGeneratorChanged;
		}

		private void UpdateListViewControl()
		{
			ListViewControl = ListViewItem?.ListViewControl;
		}
	}
}