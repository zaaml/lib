// <copyright file="TabGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class TabDockItemGroup : DockItemGroup<TabLayout>
  {
    #region Ctors

    static TabDockItemGroup()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<TabDockItemGroup>();
    }

    internal TabDockItemGroup() : this(DockItemState.Float)
    {
    }

    internal TabDockItemGroup(DockItemState dockState) : base(dockState)
    {
      this.OverrideStyleKey<TabDockItemGroup>();
    }

    #endregion

    #region Properties

    public override DockItemGroupKind GroupKind => DockItemGroupKind.Tab;

    public override DockItemKind Kind => DockItemKind.TabGroup;

    protected override BaseLayoutView<TabLayout> LayoutView => TabLayoutView;

    private TabLayoutView TabLayoutView => TemplateContract.LayoutView;

    private TabDockItemGroupTemplateContract TemplateContract => (TabDockItemGroupTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    protected internal override DockItemLayout CreateItemLayout()
    {
      return new TabDockItemGroupLayout(this);
    }

    protected override DockItem CreatePreviewItem(DockItemState dockState)
    {
      return new TabDockItemGroup(dockState);
    }

    protected override TemplateContract CreateTemplateContract()
    {
      return new TabDockItemGroupTemplateContract();
    }

    internal override BaseLayout GetItemTargetLayout(DockItem item, bool arrange) => DockState == DockItemState.AutoHide ? Controller?.GetLayout(DockItemState.AutoHide) : base.GetItemTargetLayout(item, arrange);

    protected override void OnItemAdded(DockItem item)
    {
      if (item.GetType().IsSubclassOf(typeof(DockItemGroup)))
        throw new Exception("Only simple items could be added");

      base.OnItemAdded(item);
    }

    #endregion
  }
}