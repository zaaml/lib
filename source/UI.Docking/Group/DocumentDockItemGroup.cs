// <copyright file="DocumentDockItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class DocumentDockItemGroup : DockItemGroup<DocumentLayout>
	{
		static DocumentDockItemGroup()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DocumentDockItemGroup>();
		}

		internal DocumentDockItemGroup()
		{
			this.OverrideStyleKey<DocumentDockItemGroup>();
		}

		internal override bool AllowSingleItem => true;

		private DocumentLayoutView DocumentLayoutView => TemplateContract.LayoutView;

		public override DockItemGroupKind GroupKind => DockItemGroupKind.Document;

		public override DockItemKind Kind => DockItemKind.DocumentDockItemGroup;

		protected override BaseLayoutView<DocumentLayout> LayoutView => DocumentLayoutView;

		private DocumentDockItemGroupTemplateContract TemplateContract => (DocumentDockItemGroupTemplateContract)TemplateContractInternal;

		protected internal override DockItemLayout CreateItemLayout()
		{
			return new DocumentDockItemGroupLayout(this);
		}

		protected override DockItem CreatePreviewItem(DockItemState dockState)
		{
			return new DocumentDockItemGroup { DockState = dockState };
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new DocumentDockItemGroupTemplateContract();
		}

		protected override bool IsDockStateAllowed(DockItemState state)
		{
			return state is DockItemState.Document or DockItemState.Hidden;
		}

		protected override void OnItemAdded(DockItem item)
		{
			if (item is DockItemGroup)
				throw new Exception("Only simple windows could be added");

			base.OnItemAdded(item);
		}
	}
}