// <copyright file="TreeGridViewCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeGridViewCellsPresenter
		: TreeGridViewElementsPresenter<TreeGridViewCellsPresenter,
			TreeGridViewCellsPanel,
			TreeGridViewCellCollection,
			TreeGridViewCell>
	{
		private static readonly DependencyPropertyKey TreeViewItemPropertyKey = DPM.RegisterReadOnly<TreeViewItem, TreeGridViewCellsPresenter>
			("TreeViewItem", d => d.OnTreeViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty TreeViewItemProperty = TreeViewItemPropertyKey.DependencyProperty;

		public TreeGridViewCellsPresenter()
		{
			AllowCellSplitter = false;
		}

		public TreeViewItem TreeViewItem
		{
			get => (TreeViewItem)GetValue(TreeViewItemProperty);
			internal set => this.SetReadOnlyValue(TreeViewItemPropertyKey, value);
		}

		protected override void AddCellGeneratorChanged(TreeGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualCellGeneratorChanged += cellGeneratorChanged;
		}

		protected override void AttachCell(TreeGridViewCell gridCell)
		{
			base.AttachCell(gridCell);

			gridCell.TreeViewItem = TreeViewItem;
		}

		protected override TreeGridViewCellCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override void DetachCell(TreeGridViewCell gridCell)
		{
			gridCell.TreeViewItem = null;

			base.DetachCell(gridCell);
		}

		protected override GridViewCellGenerator<TreeGridViewCell> GetCellGenerator(TreeGridViewColumn gridColumn)
		{
			return gridColumn.ActualCellGenerator;
		}

		private void OnTreeViewItemPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == TreeViewItem.TreeViewControlProperty)
				UpdateTreeViewControl();
		}

		private void OnTreeViewItemPropertyChangedPrivate(TreeViewItem oldValue, TreeViewItem newValue)
		{
			if (oldValue != null)
				oldValue.DependencyPropertyChangedInternal -= OnTreeViewItemPropertyChanged;

			if (newValue != null)
				newValue.DependencyPropertyChangedInternal += OnTreeViewItemPropertyChanged;

			foreach (var cell in Cells)
				cell.TreeViewItem = newValue;

			UpdateTreeViewControl();
		}

		protected override void RemoveCellGeneratorChanged(TreeGridViewColumn gridColumn, EventHandler cellGeneratorChanged)
		{
			gridColumn.ActualCellGeneratorChanged -= cellGeneratorChanged;
		}

		private void UpdateTreeViewControl()
		{
			TreeViewControl = TreeViewItem?.TreeViewControl;
		}
	}
}