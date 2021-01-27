// <copyright file="ListViewItemGridColumnHeader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
	[TemplateContractType(typeof(ListViewItemGridColumnHeaderTemplateContract))]
	public sealed class ListViewItemGridColumnHeader
		: ListViewItemGridCellBase<ListViewItemGridColumnHeadersPresenter,
			ListViewItemGridColumnHeadersPanel,
			ListViewItemGridColumnHeaderCollection,
			ListViewItemGridColumnHeader>
	{
		static ListViewItemGridColumnHeader()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListViewItemGridColumnHeader>();
		}

		internal ListViewItemGridColumnHeader()
		{
			this.OverrideStyleKey<ListViewItemGridColumnHeader>();
		}

		protected override DataTemplate CellContentTemplateCore => Column?.HeaderContentTemplate;

		protected override Binding ContentBindingCore => Column?.ActualHeaderBinding;

		protected override Thickness PaddingCore
		{
			get
			{
				var column = Column;

				if (column == null)
					return base.PaddingCore;

				return column.GetValueSource(ListGridViewColumn.HeaderPaddingProperty) == PropertyValueSource.Default ? base.PaddingCore : column.HeaderPadding;
			}
		}
	}
}