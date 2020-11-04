// <copyright file="RibbonHeaderPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using ContentPresenter = System.Windows.Controls.ContentPresenter;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonHeaderPresenterTemplateContract))]
  public sealed class RibbonHeaderPresenter : TemplateContractControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty QuickAccessToolBarProperty = DPM.Register<RibbonToolBar, RibbonHeaderPresenter>
      ("QuickAccessToolBar", r => r.OnQuickAccessToolBarChanged);

    public static readonly DependencyProperty PageCategoriesPresenterProperty = DPM.Register<RibbonPageCategoriesPresenter, RibbonHeaderPresenter>
      ("PageCategoriesPresenter", r => r.OnTabCategoriesPresenterChanged);

    public static readonly DependencyProperty TitleProperty = DPM.Register<object, RibbonHeaderPresenter>
      ("Title", r => r.OnTitleChanged);

    public static readonly DependencyProperty FooterProperty = DPM.Register<object, RibbonHeaderPresenter>
      ("Footer", r => r.OnFooterChanged);

    public static readonly DependencyProperty HeaderProperty = DPM.Register<object, RibbonHeaderPresenter>
      ("Header", r => r.OnHeaderChanged);

    private static readonly DependencyPropertyKey RibbonPropertyKey = DPM.RegisterReadOnly<RibbonControl, RibbonHeaderPresenter>
      ("Ribbon");

    public static readonly DependencyProperty MenuProperty = DPM.Register<FrameworkElement, RibbonHeaderPresenter>
      ("Menu", r => r.OnMenuChanged);

    public static readonly DependencyProperty PagesPresenterProperty = DPM.Register<RibbonPagesPresenter, RibbonHeaderPresenter>
      ("PagesPresenter", r => r.OnPagesPresenterChanged);


    public static readonly DependencyProperty RibbonProperty = RibbonPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static RibbonHeaderPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonHeaderPresenter>();
    }

    public RibbonHeaderPresenter()
    {
      this.OverrideStyleKey<RibbonHeaderPresenter>();
    }

    #endregion

    #region Properties

    public object Footer
    {
      get => GetValue(FooterProperty);
      set => SetValue(FooterProperty, value);
    }

    internal FrameworkElement FooterElement { get; private set; }

    public object Header
    {
      get => GetValue(HeaderProperty);
      set => SetValue(HeaderProperty, value);
    }

    internal FrameworkElement HeaderElement { get; private set; }

    private RibbonHeaderPanel HeaderPanel => TemplateContract.HeaderPanel;

    public FrameworkElement Menu
    {
      get => (FrameworkElement) GetValue(MenuProperty);
      set => SetValue(MenuProperty, value);
    }

    public RibbonPageCategoriesPresenter PageCategoriesPresenter
    {
      get => (RibbonPageCategoriesPresenter) GetValue(PageCategoriesPresenterProperty);
      set => SetValue(PageCategoriesPresenterProperty, value);
    }

    public RibbonPagesPresenter PagesPresenter
    {
      get => (RibbonPagesPresenter) GetValue(PagesPresenterProperty);
      set => SetValue(PagesPresenterProperty, value);
    }

    public RibbonToolBar QuickAccessToolBar
    {
      get => (RibbonToolBar) GetValue(QuickAccessToolBarProperty);
      set => SetValue(QuickAccessToolBarProperty, value);
    }

    public RibbonControl Ribbon
    {
      get => (RibbonControl) GetValue(RibbonProperty);
      internal set => this.SetReadOnlyValue(RibbonPropertyKey, value);
    }

    private RibbonHeaderPresenterTemplateContract TemplateContract => (RibbonHeaderPresenterTemplateContract) TemplateContractInternal;

    public object Title
    {
      get => GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
    }

    internal FrameworkElement TitleElement { get; private set; }

    #endregion

    #region  Methods

    private void OnFooterChanged()
    {
      FooterElement = WrapElement(Footer);
      HeaderPanel?.InvalidateMeasure();
    }

    private void OnHeaderChanged()
    {
      HeaderElement = WrapElement(Header);
      HeaderPanel?.InvalidateMeasure();
    }

    private void OnMenuChanged()
    {
      InvalidateMeasure();
    }

    private void OnPagesPresenterChanged()
    {
      InvalidateMeasure();
    }

    private void OnQuickAccessToolBarChanged()
    {
      HeaderPanel?.InvalidateMeasure();
    }

    private void OnTabCategoriesPresenterChanged()
    {
      HeaderPanel?.InvalidateMeasure();
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();
      HeaderPanel.Presenter = this;
    }

    protected override void OnTemplateContractDetaching()
    {
      HeaderPanel.Presenter = null;
      base.OnTemplateContractDetaching();
    }

    private void OnTitleChanged()
    {
      TitleElement = WrapElement(Title);
      HeaderPanel?.InvalidateMeasure();
    }

    private FrameworkElement WrapElement(object content)
    {
      if (content == null)
        return null;

      return content as FrameworkElement ?? new ContentPresenter {Content = content};
    }

    #endregion
  }

  public sealed class RibbonHeaderPresenterTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public RibbonHeaderPanel HeaderPanel { get; [UsedImplicitly] private set; }

    #endregion
  }
}