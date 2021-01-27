// <copyright file="TreeViewItemGridColumnHeader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.TreeView
{
	[TemplateContractType(typeof(TreeViewItemGridColumnHeaderTemplateContract))]
	public sealed class TreeViewItemGridColumnHeader
		: TreeViewItemGridCellBase<TreeViewItemGridColumnHeadersPresenter,
			TreeViewItemGridColumnHeadersPanel,
			TreeViewItemGridColumnHeaderCollection,
			TreeViewItemGridColumnHeader>
	{
		static TreeViewItemGridColumnHeader()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItemGridColumnHeader>();
		}

		internal TreeViewItemGridColumnHeader()
		{
			this.OverrideStyleKey<TreeViewItemGridColumnHeader>();

			BorderThickness = new Thickness(1, 0, 1, 1);
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

				return column.GetValueSource(TreeGridViewColumn.HeaderPaddingProperty) == PropertyValueSource.Default ? base.PaddingCore : column.HeaderPadding;
			}
		}
	}
}