// <copyright file="RibbonToolBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ToolBar;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonToolBarTemplateContract))]
  public class RibbonToolBar : ToolBarControlBase<Control, RibbonItem, RibbonItemCollection, RibbonToolBarItemsPresenter, RibbonToolBarItemsPanel>
  {
    #region Ctors

    static RibbonToolBar()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonToolBar>();
    }

    public RibbonToolBar()
    {
      this.OverrideStyleKey<RibbonToolBar>();
    }

    #endregion

    #region Properties

    private RibbonToolBarItemsPanel ItemsHost => ItemsPresenter?.ItemsHostInternal;

    protected RibbonToolBarOverflowItemsPresenter OverflowItemsPresenter => TemplateContract.OverflowItemsPresenter;

    private RibbonToolBarTemplateContract TemplateContract => (RibbonToolBarTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    protected override RibbonItemCollection CreateItemCollection()
    {
      return new RibbonItemCollection(this);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var hasOverflowedItems = HasOverflowItems;
      HasOverflowItems = false;

      var measureOverride = base.MeasureOverride(availableSize);

      HasOverflowItems = ItemsHost?.HasOverflowChildren ?? false;

      if (HasOverflowItems == false && hasOverflowedItems == false)
        return measureOverride;

      if (ItemsHost != null)
        PanelUtils.InvalidateAncestorsMeasure(ItemsHost, this);

      return base.MeasureOverride(availableSize);
    }

    internal override void OnItemAttachedInternal(RibbonItem item)
    {
      item.ToolBar = this;
      item.ActualItemStyle = RibbonItemStyle.Small;

      base.OnItemAttachedInternal(item);
    }

    internal override void OnItemDetachedInternal(RibbonItem item)
    {
      base.OnItemDetachedInternal(item);

      item.ActualItemStyle = RibbonItemStyle.Large;
      item.ToolBar = null;
    }

    protected override void OnTemplateContractAttached()
    {
      ItemsPresenter.ToolBar = this;
      OverflowItemsPresenter.ToolBar = this;
      OverflowItemsPresenter.OverflowItems.SourceInternal = Items;

      base.OnTemplateContractAttached();
    }

    protected override void OnTemplateContractDetaching()
    {
      base.OnTemplateContractDetaching();

      OverflowItemsPresenter.OverflowItems.SourceInternal = null;
      OverflowItemsPresenter.ToolBar = null;
      ItemsPresenter.ToolBar = null;
    }

    #endregion
  }

  public class RibbonToolBarTemplateContract : ToolBarControlBaseTemplateContract<Control, RibbonItem, RibbonItemCollection, RibbonToolBarItemsPresenter, RibbonToolBarItemsPanel>
  {
    #region Properties

    [TemplateContractPart(Required = false)]
    public RibbonToolBarOverflowItemsPresenter OverflowItemsPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}