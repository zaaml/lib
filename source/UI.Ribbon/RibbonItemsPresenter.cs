// <copyright file="RibbonItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonItemsPresenterTemplateContract))]
  public sealed class RibbonItemsPresenter : ItemsPresenterBase<Control, RibbonItem, RibbonItemCollection, RibbonItemsPanel>
  {
    #region Ctors

    static RibbonItemsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonItemsPresenter>();
    }

    public RibbonItemsPresenter()
    {
      this.OverrideStyleKey<RibbonItemsPresenter>();
    }

    #endregion

    #region Properties

    internal RibbonGroup Group { get; set; }

    #endregion

    #region  Methods

    internal void InvalidateInt(RibbonGroupsPanel ribbonGroupPanel)
    {
      if (ItemsHost != null)
        PanelUtils.InvalidateAncestorsMeasure(ItemsHost, ribbonGroupPanel);

      foreach (var ribbonItem in Items)
        ribbonItem.InvalidateMeasureInt();
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();
      ItemsHost.ItemsPresenter = this;
    }

    protected override void OnTemplateContractDetaching()
    {
      ItemsHost.ItemsPresenter = null;
      base.OnTemplateContractDetaching();
    }

    #endregion
  }

  public sealed class RibbonItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<RibbonItemsPanel, RibbonItem>
  {
  }
}