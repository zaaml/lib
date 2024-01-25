// <copyright file="ListGridViewCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	[TemplateContractType(typeof(ListGridViewCellTemplateContract))]
	public class ListGridViewCell
		: ListGridViewCellBase<ListGridViewCellsPresenter,
			ListGridViewCellsPanel,
			ListGridViewCellCollection,
			ListGridViewCell>
	{
		private static readonly DependencyPropertyKey ListViewItemPropertyKey = DPM.RegisterReadOnly<ListViewItem, ListGridViewCell>
			("ListViewItem", d => d.OnListViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewItemProperty = ListViewItemPropertyKey.DependencyProperty;

		static ListGridViewCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListGridViewCell>();
		}

		public ListGridViewCell()
		{
			this.OverrideStyleKey<ListGridViewCell>();
		}

		protected override ListGridView GridView => (CellsPresenterInternal as ListGridViewCellsPresenter)?.ListViewItem?.ListViewControl.View as ListGridView;
		
		protected override GridViewLines GridViewLines => GridView?.CellAppearance?.ActualGridLines ?? GridViewLines.Both;

		public ListViewItem ListViewItem
		{
			get => (ListViewItem)GetValue(ListViewItemProperty);
			internal set => this.SetReadOnlyValue(ListViewItemPropertyKey, value);
		}

		private void OnListViewItemPropertyChangedPrivate(ListViewItem oldValue, ListViewItem newValue)
		{
		}
	}
}