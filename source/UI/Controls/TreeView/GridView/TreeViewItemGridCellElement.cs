// <copyright file="TreeViewItemGridCellElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewItemGridCellElement : GridElement
	{
		static TreeViewItemGridCellElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItemGridCellElement>();
		}

		public TreeViewItemGridCellElement()
		{
			this.OverrideStyleKey<TreeViewItemGridCellElement>();
		}
	}
}