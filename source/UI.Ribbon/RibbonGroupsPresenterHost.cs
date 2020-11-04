// <copyright file="RibbonGroupsPresenterHost.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  [ContentProperty(nameof(GroupsPresenter))]
  public sealed class RibbonGroupsPresenterHost : FixedTemplateControl<SingleChildPanel>
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty GroupsPresenterProperty = DPM.Register<RibbonGroupsPresenter, RibbonGroupsPresenterHost>
      ("GroupsPresenter", h => h.OnGroupsPresenterChanged);

    #endregion

    #region Fields

    private RibbonGroupsPresenter _logicalChild;

    private RibbonPage _page;
    private RibbonControl _ribbon;
    private RibbonGroupsPresenter _visualChild;

    #endregion

    #region Properties

    public RibbonGroupsPresenter GroupsPresenter
    {
      get => (RibbonGroupsPresenter) GetValue(GroupsPresenterProperty);
      set => SetValue(GroupsPresenterProperty, value);
    }

    internal RibbonGroupsPresenter LogicalChild
    {
      get => _logicalChild;
      set
      {
        if (ReferenceEquals(_logicalChild, value))
          return;

        if (_logicalChild != null)
          RemoveLogicalChild(_logicalChild);

        _logicalChild = value;

        if (_logicalChild != null)
          AddLogicalChild(_logicalChild);
      }
    }

    internal RibbonPage Page
    {
      get => _page;
      set
      {
        if (ReferenceEquals(_page, value))
          return;

        if (GroupsPresenter != null && ReferenceEquals(GroupsPresenter.LogicalHost, this))
          GroupsPresenter.LogicalHost = null;

        _page = value;

        if (GroupsPresenter != null && Page != null)
          GroupsPresenter.LogicalHost = this;
      }
    }

    internal RibbonControl Ribbon
    {
      get => _ribbon;
      set
      {
        if (ReferenceEquals(_ribbon, value))
          return;

        if (GroupsPresenter != null && ReferenceEquals(GroupsPresenter.VisualHost, this))
          GroupsPresenter.VisualHost = null;

        _ribbon = value;

        if (GroupsPresenter != null && Ribbon != null)
          GroupsPresenter.VisualHost = this;
      }
    }

    internal RibbonGroupsPresenter VisualChild
    {
      get => _visualChild;
      set
      {
        if (ReferenceEquals(_visualChild, value))
          return;

        if (_visualChild != null)
        {
          if (TemplateRoot != null)
            TemplateRoot.Child = null;
        }

        _visualChild = value;

        if (_visualChild != null)
        {
          if (TemplateRoot != null)
            TemplateRoot.Child = _visualChild;
        }
      }
    }

    #endregion

    #region  Methods

    protected override void ApplyTemplateOverride()
    {
      base.ApplyTemplateOverride();
      TemplateRoot.Child = VisualChild;
    }

    private void OnGroupsPresenterChanged(RibbonGroupsPresenter oldGroupsPresenter, RibbonGroupsPresenter newGroupsPresenter)
    {
      if (oldGroupsPresenter != null)
      {
        if (ReferenceEquals(this, oldGroupsPresenter.LogicalHost))
          oldGroupsPresenter.LogicalHost = null;

        if (ReferenceEquals(this, oldGroupsPresenter.VisualHost))
          oldGroupsPresenter.VisualHost = null;
      }

      if (newGroupsPresenter != null)
      {
        if (Page != null)
          newGroupsPresenter.LogicalHost = this;

        if (Ribbon != null)
          newGroupsPresenter.VisualHost = this;
      }
    }

    protected override void UndoTemplateOverride()
    {
      TemplateRoot.Child = null;
      base.UndoTemplateOverride();
    }

    #endregion
  }
}