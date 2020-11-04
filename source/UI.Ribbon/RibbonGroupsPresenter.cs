// <copyright file="RibbonGroupsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

#pragma warning disable 169

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonGroupsPresenterTemplateContract))]
  public class RibbonGroupsPresenter : ItemsPresenterBase<RibbonPage, RibbonGroup, RibbonGroupCollection, RibbonGroupsPanel>
  {
    #region Fields

    [UsedImplicitly] private IDisposable _groupsDispatcher;
    private RibbonGroupsPresenterHost _logicalHost;
    private RibbonGroupsPresenterHost _visualHost;

    #endregion

    #region Ctors

    static RibbonGroupsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonGroupsPresenter>();
    }

    public RibbonGroupsPresenter()
    {
      this.OverrideStyleKey<RibbonGroupsPresenter>();
    }

    #endregion

    #region Properties

    private RibbonGroupsPanel GroupsPanel => TemplateContract.ItemsHost;

    internal RibbonGroupsPresenterHost LogicalHost
    {
      get => _logicalHost;
      set
      {
        if (ReferenceEquals(_logicalHost, value))
          return;

        if (LogicalHost != null && ReferenceEquals(LogicalHost.LogicalChild, this))
          LogicalHost.LogicalChild = null;

        _logicalHost = value;

        UpdateHost();
      }
    }

    internal RibbonPage Page { get; set; }

    private RibbonGroupsPresenterTemplateContract TemplateContract => (RibbonGroupsPresenterTemplateContract) TemplateContractInternal;

    internal RibbonGroupsPresenterHost VisualHost
    {
      get => _visualHost;
      set
      {
        if (ReferenceEquals(_visualHost, value))
          return;

        if (VisualHost != null && ReferenceEquals(VisualHost.VisualChild, this))
          VisualHost.VisualChild = null;

        _visualHost = value;

        UpdateHost();
      }
    }

    #endregion

    #region  Methods

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();
      GroupsPanel.GroupsPresenter = this;
    }

    protected override void OnTemplateContractDetaching()
    {
      GroupsPanel.GroupsPresenter = null;
      base.OnTemplateContractDetaching();
    }

    private void UpdateHost()
    {
      if (VisualHost != null)
      {
        if (LogicalHost != null)
          LogicalHost.LogicalChild = null;

        VisualHost.VisualChild = this;
      }
      else if (LogicalHost != null)
        LogicalHost.LogicalChild = this;
    }

    #endregion
  }

  public sealed class RibbonGroupsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<RibbonGroupsPanel, RibbonGroup>
  {
  }
}