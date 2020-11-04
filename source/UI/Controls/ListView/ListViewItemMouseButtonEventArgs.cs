// <copyright file="ListViewItemMouseButtonEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewItemMouseButtonEventArgs : EventArgs
	{
		public ListViewItemMouseButtonEventArgs(ListViewItem item, MouseButtonEventArgs mouseEventArgs)
		{
			Item = item;
			MouseEventArgs = mouseEventArgs;
		}

		public ListViewItem Item { get; }

		public MouseButtonEventArgs MouseEventArgs { get; }
	}
}