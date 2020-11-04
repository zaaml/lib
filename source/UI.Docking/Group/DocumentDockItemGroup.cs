// <copyright file="DocumentGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class DocumentDockItemGroup : DockItemGroup<DocumentLayout>
  {
    #region Ctors

    static DocumentDockItemGroup()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DocumentDockItemGroup>();
    }

    internal DocumentDockItemGroup() : this(DockItemState.Document)
    {
    }

    internal DocumentDockItemGroup(DockItemState dockState) : base(dockState)
    {
      this.OverrideStyleKey<DocumentDockItemGroup>();
    }

    #endregion

    #region Properties

    internal override bool AllowSingleItem => true;

    private DocumentLayoutView DocumentLayoutView => TemplateContract.LayoutView;

    public override DockItemGroupKind GroupKind => DockItemGroupKind.Document;

    public override DockItemKind Kind => DockItemKind.DocumentGroup;

    protected override BaseLayoutView<DocumentLayout> LayoutView => DocumentLayoutView;

    private DocumentDockItemGroupTemplateContract TemplateContract => (DocumentDockItemGroupTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    protected internal override DockItemLayout CreateItemLayout()
    {
      return new DocumentDockItemGroupLayout(this);
    }

    protected override DockItem CreatePreviewItem(DockItemState dockState)
    {
      return new DocumentDockItemGroup(dockState);
    }

    protected override TemplateContract CreateTemplateContract()
    {
      return new DocumentDockItemGroupTemplateContract();
    }

    protected override bool IsDockStateAllowed(DockItemState state)
    {
      return state == DockItemState.Document || state == DockItemState.Hidden;
    }

    protected override void OnItemAdded(DockItem item)
    {
      if (item is DockItemGroup)
        throw new Exception("Only simple windows could be added");

      base.OnItemAdded(item);
    }

    #endregion
  }
}