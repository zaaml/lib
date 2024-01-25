// <copyright file="TreeGridViewHeader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	[TemplateContractType(typeof(TreeGridViewHeaderTemplateContract))]
	public sealed class TreeGridViewHeader
		: TreeGridViewCellBase<TreeGridViewHeadersPresenter,
			TreeGridViewHeadersPanel,
			TreeGridViewHeaderCollection,
			TreeGridViewHeader>
	{
		static TreeGridViewHeader()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeGridViewHeader>();
		}

		public TreeGridViewHeader()
		{
			this.OverrideStyleKey<TreeGridViewHeader>();
		}

		protected override TreeGridView GridView => (CellsPresenterInternal as TreeGridViewHeadersPresenter)?.TreeViewControl.View as TreeGridView;

		protected override GridViewLines GridViewLines => GridView?.HeaderAppearance?.ActualGridLines ?? GridViewLines.Both;

		protected override Thickness GetBorderThickness(GridViewLines gridViewLines)
		{
			var vt = gridViewLines.ShowVertical() ? 1 : 0;
			var ht = gridViewLines.ShowHorizontal() ? 1 : 0;

			return BorderThickness = new Thickness(vt, 0, vt, ht);
		}
	}
}