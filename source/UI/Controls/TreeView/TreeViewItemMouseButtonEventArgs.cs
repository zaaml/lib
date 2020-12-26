// <copyright file="TreeViewItemMouseButtonEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewItemEventArgs : EventArgs
	{
		public TreeViewItemEventArgs(TreeViewItem item)
		{
			Item = item;
		}

		public TreeViewItem Item { get; }
	}

	public class TreeViewItemMouseButtonEventArgs : TreeViewItemEventArgs
	{
		public TreeViewItemMouseButtonEventArgs(TreeViewItem item, MouseButtonEventArgs mouseEventArgs) : base(item)
		{
			MouseEventArgs = mouseEventArgs;
		}

		public MouseButtonEventArgs MouseEventArgs { get; }
	}

	public class TreeViewItemClickEventArgs : TreeViewItemEventArgs
	{
		public TreeViewItemClickEventArgs(TreeViewItem item) : base(item)
		{
		}
	}
}