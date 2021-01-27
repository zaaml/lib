// <copyright file="TreeViewItemGridCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemGridCellsPresenter
		: TreeViewItemGridElementPresenter<TreeViewItemGridCellsPresenter,
			TreeViewItemGridCellsPanel,
			TreeViewItemGridCellCollection,
			TreeViewItemGridCell>
	{
		private static readonly DependencyPropertyKey TreeViewItemPropertyKey = DPM.RegisterReadOnly<TreeViewItem, TreeViewItemGridCellsPresenter>
			("TreeViewItem", d => d.OnTreeViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty TreeViewItemProperty = TreeViewItemPropertyKey.DependencyProperty;

		public TreeViewItemGridCellsPresenter()
		{
			AllowCellSplitter = false;
		}

		private Stack<TreeViewItemGridCell> CellsPool { get; } = new();

		public TreeViewItem TreeViewItem
		{
			get => (TreeViewItem) GetValue(TreeViewItemProperty);
			internal set => this.SetReadOnlyValue(TreeViewItemPropertyKey, value);
		}

		protected override TreeViewItemGridCellCollection CreateCellCollection()
		{
			return new(this);
		}

		protected override TreeViewItemGridCell CreateCell()
		{
			return new TreeViewItemGridCell { TreeViewItem = TreeViewItem };
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

			foreach (var cell in CellsPool) 
				cell.TreeViewItem = newValue;

			UpdateTreeViewControl();
		}

		private void UpdateTreeViewControl()
		{
			TreeViewControl = TreeViewItem?.TreeViewControl;
		}
	}
}