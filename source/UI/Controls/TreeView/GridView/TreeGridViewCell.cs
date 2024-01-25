// <copyright file="TreeGridViewCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	[TemplateContractType(typeof(TreeGridViewCellTemplateContract))]
	public sealed class TreeGridViewCell
		: TreeGridViewCellBase<TreeGridViewCellsPresenter,
			TreeGridViewCellsPanel,
			TreeGridViewCellCollection,
			TreeGridViewCell>
	{
		private static readonly DependencyPropertyKey IsExpanderCellPropertyKey = DPM.RegisterReadOnly<bool, TreeGridViewCell>
			("IsExpanderCell", c => c.OnIsExpanderCellPropertyChangedPrivate);

		public static readonly DependencyProperty IsExpanderCellProperty = IsExpanderCellPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey TreeViewItemPropertyKey = DPM.RegisterReadOnly<TreeViewItem, TreeGridViewCell>
			("TreeViewItem", c => c.OnTreeViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty TreeViewItemProperty = TreeViewItemPropertyKey.DependencyProperty;

		static TreeGridViewCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeGridViewCell>();
		}

		public TreeGridViewCell()
		{
			this.OverrideStyleKey<TreeGridViewCell>();
		}

		private TreeViewItemExpander Expander => TemplateContract.Expander;

		protected override TreeGridView GridView => (CellsPresenterInternal as TreeGridViewCellsPresenter)?.TreeViewItem?.TreeViewControl.View as TreeGridView;

		protected override GridViewLines GridViewLines => GridView?.CellAppearance?.ActualGridLines ?? GridViewLines.Both;

		public bool IsExpanderCell
		{
			get => (bool)GetValue(IsExpanderCellProperty);
			private set => this.SetReadOnlyValue(IsExpanderCellPropertyKey, value);
		}

		private TreeGridViewCellTemplateContract TemplateContract => (TreeGridViewCellTemplateContract)TemplateContractCore;

		public TreeViewItem TreeViewItem
		{
			get => (TreeViewItem)GetValue(TreeViewItemProperty);
			internal set => this.SetReadOnlyValue(TreeViewItemPropertyKey, value);
		}

		private void OnIsExpanderCellPropertyChangedPrivate()
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			if (Expander != null)
				Expander.TreeViewItem = TreeViewItem;
		}

		protected override void OnTemplateContractDetaching()
		{
			if (Expander != null)
				Expander.TreeViewItem = null;

			base.OnTemplateContractDetaching();
		}

		private void OnTreeViewItemPropertyChangedPrivate()
		{
			if (Expander != null)
				Expander.TreeViewItem = TreeViewItem;
		}

		protected override void UpdateStructure()
		{
			base.UpdateStructure();

			IsExpanderCell = GridColumn?.IsExpanderColumn ?? false;
		}
	}
}