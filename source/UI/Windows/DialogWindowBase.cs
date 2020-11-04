// <copyright file="DialogWindowBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.UI.Windows
{
  public abstract class DialogWindowBase : WindowBase
  {
    #region Fields

    private WindowButtonsPresenter _buttonsPresenter;

    #endregion

    #region Properties

    protected abstract IEnumerable<WindowButton> ActualButtons { get; }

    private WindowButtonsPresenter ButtonsPresenter
    {
      get => _buttonsPresenter;
      set
      {
        if (ReferenceEquals(_buttonsPresenter, value))
          return;

        DetachButtons();

        _buttonsPresenter = value;

        AttachButtons();
      }
    }

    #endregion

    #region  Methods

    protected void AttachButtons()
    {
      if (ButtonsPresenter == null)
        return;

      foreach (var button in ActualButtons)
        ButtonsPresenter.Buttons.Add(button);
    }

    protected void DetachButtons()
    {
      if (ButtonsPresenter == null)
        return;

      foreach (var button in ActualButtons)
        ButtonsPresenter.Buttons.Remove(button);
    }

    internal override void OnFooterPresenterAttachedInternal(WindowFooterPresenter footerPresenter)
    {
      base.OnFooterPresenterAttachedInternal(footerPresenter);

      ButtonsPresenter = footerPresenter.ButtonsPresenterInternal;

      footerPresenter.TemplateContractAttached += OnFooterPresenterTemplateContractAttached;
      footerPresenter.TemplateContractDetaching += OnFooterPresenterTemplateContractDetaching;
    }

    private void OnFooterPresenterTemplateContractDetaching(object sender, EventArgs e)
    {
      ButtonsPresenter = null;
    }

    private void OnFooterPresenterTemplateContractAttached(object sender, EventArgs e)
    {
      ButtonsPresenter = FooterPresenter.ButtonsPresenterInternal;
    }

    internal override void OnFooterPresenterDetachingInternal(WindowFooterPresenter footerPresenter)
    {
      footerPresenter.TemplateContractAttached -= OnFooterPresenterTemplateContractAttached;
      footerPresenter.TemplateContractDetaching -= OnFooterPresenterTemplateContractDetaching;

      ButtonsPresenter = null;

      base.OnFooterPresenterDetachingInternal(footerPresenter);
    }

    #endregion
  }
}