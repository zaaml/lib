// <copyright file="TreeViewItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;
using Zaaml.UI.Panels.VirtualStackPanelLayout;
using Zaaml.UI.Utils;
using ScrollUnit = Zaaml.UI.Controls.ScrollView.ScrollUnit;

namespace Zaaml.UI.Controls.TreeView
{
	public sealed class TreeViewItemsPanel : VirtualStackItemsPanel<TreeViewItem>
	{
		public TreeViewItemsPanel()
		{
			ReduceMouseHoverFlickering = true;
		}

		protected override int FocusedIndex => TreeViewControl?.FocusedIndexInternal ?? -1;

		internal TreeViewItemsPresenter ItemsPresenter { get; set; }

		protected override Orientation Orientation => Orientation.Vertical;

		protected override ScrollUnit ScrollUnit => TreeViewControl?.ScrollUnit ?? base.ScrollUnit;

		internal TreeViewControl TreeViewControl => ItemsPresenter?.TreeViewControl;

		private protected override IVirtualItemCollection VirtualItemCollection => TreeViewControl?.VirtualItemCollection;

		private protected override FlickeringReducer<TreeViewItem> CreateFlickeringReducer()
		{
			return new MouseHoverVisualStateFlickeringReducer<TreeViewItem>(this);
		}

		private protected override VirtualPixelStackPanelLayout CreateVirtualPixelStackPanelLayout()
		{
			return new TreeVirtualPixelStackPanelLayout(this);
		}

		private protected override VirtualUnitStackPanelLayout CreateVirtualUnitStackPanelLayout()
		{
			return new TreeVirtualUnitStackPanelLayout(this);
		}

		private sealed class TreeVirtualPixelStackPanelLayout : VirtualPixelStackPanelLayout
		{
			public TreeVirtualPixelStackPanelLayout(IVirtualStackPanel panel) : base(panel)
			{
			}

			private protected override ScrollInfo CalcScrollInfo(ref VirtualMeasureContext context)
			{
				var scrollInfo = base.CalcScrollInfo(ref context);

				if (context.BringIntoViewRequest != null && context.BringIntoViewResult) 
					scrollInfo = scrollInfo.WithOffset(new Vector(scrollInfo.ScrollableSize.Width, scrollInfo.Offset.Y));

				return scrollInfo;
			}
		}

		private sealed class TreeVirtualUnitStackPanelLayout : VirtualUnitStackPanelLayout
		{
			public TreeVirtualUnitStackPanelLayout(IVirtualStackPanel panel) : base(panel)
			{
			}
		}
	}
}