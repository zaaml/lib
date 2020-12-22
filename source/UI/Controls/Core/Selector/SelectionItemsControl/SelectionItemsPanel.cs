// <copyright file="SelectionItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.Core
{
	public class SelectionItemsPanel<TSelectionItem, TItem> : VirtualStackItemsPanel<TSelectionItem>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
		private protected override int LeadingTrailingLimitCore => 0;
		
		protected override int FocusedIndex => -1;

		protected override Orientation Orientation => Orientation.Horizontal;

		private protected override IVirtualItemCollection VirtualItemCollection => null;

		private protected override FlickeringReducer<TSelectionItem> CreateFlickeringReducer()
		{
			return null;
		}
	}
}