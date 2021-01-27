// <copyright file="ListViewItemGridCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ListView
{
	[TemplateContractType(typeof(ListViewItemGridCellTemplateContract))]
	public sealed class ListViewItemGridCell
		: ListViewItemGridCellBase<ListViewItemGridCellsPresenter,
			ListViewItemGridCellsPanel,
			ListViewItemGridCellCollection,
			ListViewItemGridCell>
	{
		private static readonly DependencyPropertyKey ListViewItemPropertyKey = DPM.RegisterReadOnly<ListViewItem, ListViewItemGridCell>
			("ListViewItem", default, d => d.OnListViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewItemProperty = ListViewItemPropertyKey.DependencyProperty;

		static ListViewItemGridCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListViewItemGridCell>();
		}

		internal ListViewItemGridCell()
		{
			this.OverrideStyleKey<ListViewItemGridCell>();
		}

		protected override DataTemplate CellContentTemplateCore => Column?.CellDisplayContentTemplate;

		protected override Binding ContentBindingCore => Column?.ActualMemberBinding;

		public ListViewItem ListViewItem
		{
			get => (ListViewItem) GetValue(ListViewItemProperty);
			internal set => this.SetReadOnlyValue(ListViewItemPropertyKey, value);
		}

		protected override Thickness PaddingCore
		{
			get
			{
				var column = Column;

				if (column == null)
					return base.PaddingCore;

				return column.GetValueSource(ListGridViewColumn.CellPaddingProperty) == PropertyValueSource.Default ? base.PaddingCore : column.CellPadding;
			}
		}

		private void OnListViewItemPropertyChangedPrivate(ListViewItem oldValue, ListViewItem newValue)
		{
		}
	}
}