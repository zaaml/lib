// <copyright file="RibbonToolBarItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonToolBarItemsPresenterTemplateContract))]
  public class RibbonToolBarItemsPresenter : ItemsPresenterBase<Control, RibbonItem, RibbonItemCollection, RibbonToolBarItemsPanel>
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ToolBarPropertyKey = DPM.RegisterReadOnly<RibbonToolBar, RibbonToolBarItemsPresenter>
      ("ToolBar");

    public static readonly DependencyProperty ToolBarProperty = ToolBarPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static RibbonToolBarItemsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonToolBarItemsPresenter>();
    }

    public RibbonToolBarItemsPresenter()
    {
      this.OverrideStyleKey<RibbonToolBarItemsPresenter>();
    }

    #endregion

    #region Properties

    public RibbonToolBar ToolBar
    {
      get => (RibbonToolBar) GetValue(ToolBarProperty);
      internal set => this.SetReadOnlyValue(ToolBarPropertyKey, value);
    }

    #endregion

    #region  Methods

    protected override void OnTemplateContractAttached()
    {
      ItemsHost.ItemsPresenter = this;
      base.OnTemplateContractAttached();
    }

    protected override void OnTemplateContractDetaching()
    {
      base.OnTemplateContractDetaching();
      ItemsHost.ItemsPresenter = null;
    }

    #endregion
  }

  public class RibbonToolBarItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<RibbonToolBarItemsPanel, RibbonItem>
  {
  }
}