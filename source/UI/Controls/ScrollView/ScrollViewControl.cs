// <copyright file="ScrollViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.ScrollView
{
  [TemplateContractType(typeof(ScrollViewControlTemplateContract))]
  public sealed class ScrollViewControl : ScrollViewControlBase<ScrollViewPresenter, ScrollViewPanel>
  {
    #region Ctors

    static ScrollViewControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ScrollViewControl>();
    }

    public ScrollViewControl()
    {
      this.OverrideStyleKey<ScrollViewControl>();
    }

    #endregion

    #region Properties

    protected override ScrollViewPanelBase ScrollViewPanelCore => ScrollViewPresenterInternal?.ScrollViewPanel;

    #endregion

    #region  Methods

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      ScrollViewPresenterInternal.ScrollView = this;

      UpdateScrollViewPanelInternal();
    }

    protected override void OnTemplateContractDetaching()
    {
      ScrollViewPresenterInternal.ScrollView = null;

      UpdateScrollViewPanelInternal();

      base.OnTemplateContractDetaching();
    }

    #endregion
  }

  public sealed class ScrollViewControlTemplateContract : ScrollViewControlBaseTemplateContract<ScrollViewPresenter, ScrollViewPanel>
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public ScrollViewPresenter ScrollViewPresenter { get; [UsedImplicitly] private set; }

    protected override ScrollViewPresenter ScrollViewPresenterCore => ScrollViewPresenter;

    #endregion
  }
}