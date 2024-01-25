// <copyright file="DocumentDockItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
	public class DocumentDockItem : DockItem
	{
		static DocumentDockItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DocumentDockItem>();
		}

		public DocumentDockItem()
		{
			this.OverrideStyleKey<DocumentDockItem>();
		}

		public override DockItemKind Kind => DockItemKind.DocumentDockItem;

		internal override DockTabViewItem CreateDockTabViewItem()
		{
			return new DockTabViewItem(this);
		}

		protected internal override DockItemLayout CreateItemLayout()
		{
			return new DocumentDockItemLayout(this);
		}

		protected override DockItem CreatePreviewItem(DockItemState dockState)
		{
			return new DocumentDockItem { DockState = dockState };
		}

		protected override bool IsDockStateAllowed(DockItemState state)
		{
			return state == DockItemState.Float || state == DockItemState.Document || state == DockItemState.Hidden;
		}
	}
}