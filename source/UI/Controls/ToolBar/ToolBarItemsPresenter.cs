// <copyright file="ToolBarItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ToolBar
{
  [TemplateContractType(typeof(ToolBarItemsPresenterTemplateContract))]
  public sealed class ToolBarItemsPresenter : ItemsPresenterBase<ToolBarControl, ToolBarItem, ToolBarItemCollection, ToolBarItemsPanel>
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ToolBarPropertyKey = DPM.RegisterReadOnly<ToolBarControl, ToolBarItemsPresenter>
      ("ToolBar");

    public static readonly DependencyProperty ToolBarProperty = ToolBarPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static ToolBarItemsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ToolBarItemsPresenter>();
    }


    public ToolBarItemsPresenter()
    {
      this.OverrideStyleKey<ToolBarItemsPresenter>();
    }

    #endregion

    #region Properties

    public ToolBarControl ToolBar
    {
      get => (ToolBarControl) GetValue(ToolBarProperty);
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

  public class ToolBarItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<ToolBarItemsPanel, ToolBarItem>
  {
  }
}