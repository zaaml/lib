// <copyright file="ListViewItemGridCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
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
	}
}