// <copyright file="ListViewItemMouseButtonEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.UI.Controls.ListView
{
	public abstract class ListViewItemEventArgs : EventArgs
	{
		protected ListViewItemEventArgs(ListViewItem item)
		{
			Item = item;
		}

		public ListViewItem Item { get; }
	}

	public class ListViewItemMouseButtonEventArgs : ListViewItemEventArgs
	{
		public ListViewItemMouseButtonEventArgs(ListViewItem item, MouseButtonEventArgs mouseEventArgs) : base(item)
		{
			MouseEventArgs = mouseEventArgs;
		}

		public MouseButtonEventArgs MouseEventArgs { get; }
	}

	public class ListViewItemClickEventArgs : ListViewItemEventArgs
	{
		public ListViewItemClickEventArgs(ListViewItem item) : base(item)
		{
		}
	}
}