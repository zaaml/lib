// <copyright file="TreeGridViewCellElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeGridViewCellElement : TreeGridViewElement
	{
		static TreeGridViewCellElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeGridViewCellElement>();
		}

		public TreeGridViewCellElement(TreeGridViewCellsPanel cellsPanel)
		{
			this.OverrideStyleKey<TreeGridViewCellElement>();

			CellsPanel = cellsPanel;
		}

		public TreeGridViewCellsPanel CellsPanel { get; }

		protected override GridViewLines GridViewLines => CellsPanel.View?.CellAppearance?.ActualGridLines ?? GridViewLines.Both;
	}
}