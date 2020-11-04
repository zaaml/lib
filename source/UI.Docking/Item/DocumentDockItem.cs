// <copyright file="DocumentItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
  public class DocumentDockItem : DockItem
  {
    #region Ctors

    static DocumentDockItem()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DocumentDockItem>();
    }

    internal DocumentDockItem(DockItemState dockState) : base(dockState)
    {
    }

    public DocumentDockItem() : base(DockItemState.Document)
    {
      this.OverrideStyleKey<DocumentDockItem>();
    }

    #endregion

    #region Properties

    public override DockItemKind Kind => DockItemKind.Document;

    #endregion

    #region  Methods

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
      return new DocumentDockItem(dockState);
    }

    protected override bool IsDockStateAllowed(DockItemState state)
    {
      return state == DockItemState.Float || state == DockItemState.Document || state == DockItemState.Hidden;
    }

    #endregion
  }
}