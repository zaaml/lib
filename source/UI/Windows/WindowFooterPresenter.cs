// <copyright file="WindowFooterPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
  [TemplateContractType(typeof(WindowFooterPresenterTemplateContract))]
  public class WindowFooterPresenter : WindowButtonsElement
  {
    #region Ctors

    static WindowFooterPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<WindowFooterPresenter>();
    }

    public WindowFooterPresenter()
    {
      this.OverrideStyleKey<WindowFooterPresenter>();
    }

    #endregion

    #region Properties

    private WindowBase WindowBase => (WindowBase) Window;

    #endregion

    #region  Methods

    protected override void OnWindowAttached()
    {
      base.OnWindowAttached();

      WindowBase.FooterPresenter = this;
    }

    protected override void OnWindowDetaching()
    {
      WindowBase.FooterPresenter = null;

      base.OnWindowDetaching();
    }

    #endregion
  }

  public class WindowFooterPresenterTemplateContract : WindowButtonsElementTemplateContract
  {
  }
}