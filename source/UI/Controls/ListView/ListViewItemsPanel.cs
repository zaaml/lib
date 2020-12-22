// <copyright file="ListViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemsPanel : VirtualStackItemsPanel<ListViewItem>
	{
		public ListViewItemsPanel()
		{
			ReduceMouseHoverFlickering = true;
		}

		protected override int FocusedIndex => ItemsPresenter?.ListViewControl?.FocusedIndexInternal ?? -1;

		internal ListViewItemsPresenter ItemsPresenter { get; set; }

		internal ListViewControl ListViewControl => ItemsPresenter?.ListViewControl;

		protected override Orientation Orientation => Orientation.Vertical;

		private protected override IVirtualItemCollection VirtualItemCollection => ListViewControl?.VirtualItemCollection;

		private protected override FlickeringReducer<ListViewItem> CreateFlickeringReducer()
		{
			return new MouseHoverVisualStateFlickeringReducer<ListViewItem>(this);
		}
	}
}