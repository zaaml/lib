// <copyright file="TabDockItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class TabDockItemGroup : DockItemGroup<TabLayout>
	{
		static TabDockItemGroup()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TabDockItemGroup>();
		}

		internal TabDockItemGroup()
		{
			this.OverrideStyleKey<TabDockItemGroup>();
		}

		public override DockItemGroupKind GroupKind => DockItemGroupKind.Tab;

		public override DockItemKind Kind => DockItemKind.TabDockItemGroup;

		protected override BaseLayoutView<TabLayout> LayoutView => TabLayoutView;

		private TabLayoutView TabLayoutView => TemplateContract.LayoutView;

		private TabDockItemGroupTemplateContract TemplateContract => (TabDockItemGroupTemplateContract)TemplateContractInternal;

		protected internal override DockItemLayout CreateItemLayout()
		{
			return new TabDockItemGroupLayout(this);
		}

		protected override DockItem CreatePreviewItem(DockItemState dockState)
		{
			return new TabDockItemGroup { DockState = dockState };
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new TabDockItemGroupTemplateContract();
		}

		protected override void OnItemAdded(DockItem item)
		{
			if (item.GetType().IsSubclassOf(typeof(DockItemGroup)))
				throw new Exception("Only simple items could be added");

			base.OnItemAdded(item);
		}

		//private protected override void OnItemDockStateChanged(DockItem item, DockItemStateChangedEventArgs args)
		//{
		//	if (ShouldSyncDockGroupState(item, args))
		//	{
		//		(item.DockState, DockState) = (DockState, item.DockState);
		//	}
		//	else
		//		base.OnItemDockStateChanged(item, args);
		//}

		private bool ShouldSyncDockGroupState(DockItem item, DockItemStateChangedEventArgs args)
		{
			if (item.DockState == DockItemState.Hidden || args.OldDockState == DockItemState.Hidden)
				return false;

			if (args.OldDockState != DockState)
				return false;

			if ((item.DockState == DockItemState.AutoHide && args.OldDockState == DockItemState.Dock) == false &&
			    (item.DockState == DockItemState.Dock && args.OldDockState == DockItemState.AutoHide) == false)
				return false;

			return true;
		}
	}
}