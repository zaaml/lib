// <copyright file="ListGridViewCellElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public class ListGridViewCellElement : ListGridViewElement
	{
		static ListGridViewCellElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListGridViewCellElement>();
		}

		public ListGridViewCellElement(ListGridViewCellsPanel cellsPanel)
		{
			this.OverrideStyleKey<ListGridViewCellElement>();

			CellsPanel = cellsPanel;
		}

		public ListGridViewCellsPanel CellsPanel { get; }

		protected override GridViewLines GridViewLines => CellsPanel.View?.CellAppearance?.ActualGridLines ?? GridViewLines.Both;
	}
}