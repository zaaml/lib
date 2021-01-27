// <copyright file="TreeViewItemGridHeaderElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewItemGridHeaderElement : GridHeaderElement
	{
		static TreeViewItemGridHeaderElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItemGridHeaderElement>();
		}

		public TreeViewItemGridHeaderElement()
		{
			this.OverrideStyleKey<TreeViewItemGridHeaderElement>();
		}
	}
}