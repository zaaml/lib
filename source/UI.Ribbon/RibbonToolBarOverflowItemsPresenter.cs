// <copyright file="RibbonToolBarOverflowItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.Overflow;
using Zaaml.UI.Panels.Primitives;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonToolBarOverflowItemsPresenterTemplateContract))]
  public sealed class RibbonToolBarOverflowItemsPresenter : TemplateContractControl
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ToolBarPropertyKey = DPM.RegisterReadOnly<RibbonToolBar, RibbonToolBarOverflowItemsPresenter>
      ("ToolBar");

    public static readonly DependencyProperty ToolBarProperty = ToolBarPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static RibbonToolBarOverflowItemsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonToolBarOverflowItemsPresenter>();
    }

    public RibbonToolBarOverflowItemsPresenter()
    {
      this.OverrideStyleKey<RibbonToolBarOverflowItemsPresenter>();
      OverflowItems = new RibbonToolBarOverflowItemCollection(this);
    }

    #endregion

    #region Properties

    private RibbonToolBarOverflowItemsPanel ItemsHost => TemplateContract.ItemsHost;

    internal RibbonToolBarOverflowItemCollection OverflowItems { get; }

    private RibbonToolBarOverflowItemsPresenterTemplateContract TemplateContract => (RibbonToolBarOverflowItemsPresenterTemplateContract) TemplateContractInternal;

    public RibbonToolBar ToolBar
    {
      get => (RibbonToolBar) GetValue(ToolBarProperty);
      internal set => this.SetReadOnlyValue(ToolBarPropertyKey, value);
    }

    #endregion

    #region  Methods

    internal void OnItemAttached(OverflowItem<RibbonItem> item)
    {
    }

    internal void OnItemDetached(OverflowItem<RibbonItem> item)
    {
    }

    protected override void OnTemplateContractAttached()
    {
      OverflowItems.ItemsHost = ItemsHost;

      base.OnTemplateContractAttached();
    }

    protected override void OnTemplateContractDetaching()
    {
      OverflowItems.ItemsHost = null;

      base.OnTemplateContractDetaching();
    }

    #endregion
  }

  public sealed class RibbonToolBarOverflowItemsPanel : StackItemsPanelBase<OverflowItem<RibbonItem>>
  {
  }

  public sealed class RibbonToolBarOverflowItemsPresenterTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public RibbonToolBarOverflowItemsPanel ItemsHost { get; [UsedImplicitly] private set; }

    #endregion
  }
}