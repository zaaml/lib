// <copyright file="TreeViewItemMouseButtonEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewItemEventEventArgs : EventArgs
	{
		public TreeViewItemEventEventArgs(TreeViewItem item)
		{
			Item = item;
		}

		public TreeViewItem Item { get; }
	}

	public class TreeViewItemMouseButtonEventArgs : TreeViewItemEventEventArgs
	{
		public TreeViewItemMouseButtonEventArgs(TreeViewItem item, MouseButtonEventArgs mouseEventArgs) : base(item)
		{
			MouseEventArgs = mouseEventArgs;
		}

		public MouseButtonEventArgs MouseEventArgs { get; }
	}
}