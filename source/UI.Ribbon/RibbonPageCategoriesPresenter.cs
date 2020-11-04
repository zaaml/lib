// <copyright file="RibbonPageCategoriesPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
  [TemplateContractType(typeof(RibbonPageCategoriesPresenterTemplateContract))]
  public class RibbonPageCategoriesPresenter : ItemsPresenterBase<RibbonControl, RibbonPageCategory, RibbonPageCategoryCollection, RibbonPageCategoriesPanel>
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey RibbonPropertyKey = DPM.RegisterReadOnly<RibbonControl, RibbonPageCategoriesPresenter>
      ("RibbonInt");

    public static readonly DependencyProperty RibbonProperty = RibbonPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static RibbonPageCategoriesPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonPageCategoriesPresenter>();
    }

    public RibbonPageCategoriesPresenter()
    {
      this.OverrideStyleKey<RibbonPageCategoriesPresenter>();
    }

    #endregion

    #region Properties

    public RibbonControl Ribbon
    {
      get => (RibbonControl) GetValue(RibbonProperty);
      internal set => this.SetReadOnlyValue(RibbonPropertyKey, value);
    }

    #endregion

    #region  Methods

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();
      ItemsHost.Presenter = this;
    }

    protected override void OnTemplateContractDetaching()
    {
      ItemsHost.Presenter = null;
      base.OnTemplateContractDetaching();
    }

    #endregion
  }

  public class RibbonPageCategoriesPresenterTemplateContract : ItemsPresenterBaseTemplateContract<RibbonPageCategoriesPanel, RibbonPageCategory>
  {
  }
}