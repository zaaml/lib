// <copyright file="TreeViewItemGridCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.TreeView
{
	[TemplateContractType(typeof(TreeViewItemGridCellTemplateContract))]
	public sealed class TreeViewItemGridCell
		: TreeViewItemGridCellBase<TreeViewItemGridCellsPresenter,
			TreeViewItemGridCellsPanel,
			TreeViewItemGridCellCollection,
			TreeViewItemGridCell>
	{
		private static readonly DependencyPropertyKey IsExpanderCellPropertyKey = DPM.RegisterReadOnly<bool, TreeViewItemGridCell>
			("IsExpanderCell", c => c.OnIsExpanderCellPropertyChangedPrivate);

		public static readonly DependencyProperty IsExpanderCellProperty = IsExpanderCellPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey TreeViewItemPropertyKey = DPM.RegisterReadOnly<TreeViewItem, TreeViewItemGridCell>
			("TreeViewItem", c => c.OnTreeViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty TreeViewItemProperty = TreeViewItemPropertyKey.DependencyProperty;

		static TreeViewItemGridCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItemGridCell>();
		}

		internal TreeViewItemGridCell()
		{
			this.OverrideStyleKey<TreeViewItemGridCell>();
		}

		protected override DataTemplate CellContentTemplateCore => Column?.CellDisplayContentTemplate;

		protected override Binding ContentBindingCore => Column?.ActualMemberBinding;

		private TreeViewItemExpander Expander => TemplateContract.Expander;

		public bool IsExpanderCell
		{
			get => (bool) GetValue(IsExpanderCellProperty);
			private set => this.SetReadOnlyValue(IsExpanderCellPropertyKey, value);
		}

		protected override Thickness PaddingCore
		{
			get
			{
				var column = Column;

				if (column == null)
					return base.PaddingCore;

				return column.GetValueSource(TreeGridViewColumn.CellPaddingProperty) == PropertyValueSource.Default ? base.PaddingCore : column.CellPadding;
			}
		}

		private TreeViewItemGridCellTemplateContract TemplateContract => (TreeViewItemGridCellTemplateContract) TemplateContractInternal;

		public TreeViewItem TreeViewItem
		{
			get => (TreeViewItem) GetValue(TreeViewItemProperty);
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

			IsExpanderCell = Column?.IsExpanderColumn ?? false;
		}
	}
}