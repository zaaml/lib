// <copyright file="ListGridViewHeader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	[TemplateContractType(typeof(ListGridViewHeaderTemplateContract))]
	public sealed class ListGridViewHeader
		: ListGridViewCellBase<ListGridViewHeadersPresenter,
			ListGridViewHeadersPanel,
			ListGridViewHeaderCollection,
			ListGridViewHeader>
	{
		static ListGridViewHeader()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListGridViewHeader>();
		}

		public ListGridViewHeader()
		{
			this.OverrideStyleKey<ListGridViewHeader>();
		}

		protected override ListGridView GridView => (CellsPresenterInternal as ListGridViewHeadersPresenter)?.ListViewControl.View as ListGridView;

		protected override GridViewLines GridViewLines => GridView?.HeaderAppearance?.ActualGridLines ?? GridViewLines.Both;

		protected override Thickness GetBorderThickness(GridViewLines gridViewLines)
		{
			var vt = gridViewLines.ShowVertical() ? 1 : 0;
			var ht = gridViewLines.ShowHorizontal() ? 1 : 0;

			return BorderThickness = new Thickness(vt, 0, vt, ht);
		}
	}
}