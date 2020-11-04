// <copyright file="TreeViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewPanel : VirtualStackItemsPanel<TreeViewItem>
	{
		public TreeViewPanel()
		{
			ReduceMouseHoverFlickering = true;
		}

		protected override int FocusedIndex => TreeViewControl?.FocusedIndexInternal ?? -1;

		internal TreeViewItemsPresenter ItemsPresenter { get; set; }

		protected override Orientation Orientation => Orientation.Vertical;

		internal TreeViewControl TreeViewControl => ItemsPresenter?.TreeViewControl;

		private protected override IVirtualItemCollection VirtualItemCollection => TreeViewControl?.VirtualItemCollection;

		private protected override FlickeringReducer<TreeViewItem> CreateFlickeringReducer()
		{
			return new MouseHoverVisualStateFlickeringReducer<TreeViewItem>(this);
		}
	}
}