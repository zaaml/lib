// <copyright file="RibbonPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonPresenterTemplateContract))]
  public sealed class RibbonPresenter : TemplateContractControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty RibbonProperty = DPM.Register<RibbonControl, RibbonPresenter>
      ("Ribbon", r => r.OnRibbonChanged);

    public static readonly DependencyProperty TitleProperty = DPM.Register<object, RibbonPresenter>
      ("Title", p => p.OnTitleChanged);

    public static readonly DependencyProperty HeaderProperty = DPM.Register<object, RibbonPresenter>
      ("Header", p => p.OnHeaderChanged);

    public static readonly DependencyProperty FooterProperty = DPM.Register<object, RibbonPresenter>
      ("Footer", p => p.OnFooterChanged);

    #endregion

    #region Ctors

    static RibbonPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonPresenter>();
    }

    public RibbonPresenter()
    {
      this.OverrideStyleKey<RibbonPresenter>();
    }

    #endregion

    #region Properties

    public object Footer
    {
      get => GetValue(FooterProperty);
      set => SetValue(FooterProperty, value);
    }

    public object Header
    {
      get => GetValue(HeaderProperty);
      set => SetValue(HeaderProperty, value);
    }

    //protected override IEnumerator LogicalChildren
    //{
    //	get
    //	{
    //		if (Title != null)
    //			yield return Title;

    //		if (Header != null)
    //			yield return Header;

    //		if (Footer != null)
    //			yield return Footer;
    //	}
    //}

    public RibbonControl Ribbon
    {
      get => (RibbonControl) GetValue(RibbonProperty);
      set => SetValue(RibbonProperty, value);
    }

    private RibbonPresenterTemplateContract TemplateContract => (RibbonPresenterTemplateContract) TemplateContractInternal;

    public object Title
    {
      get => GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
    }

    #endregion

    #region  Methods

    private void OnFooterChanged(object oldFooter, object newFooter)
    {
      //if (oldFooter != null)
      //	RemoveLogicalChild(oldFooter);

      //if (newFooter != null)
      //	AddLogicalChild(newFooter);
    }

    private void OnHeaderChanged(object oldHeader, object newHeader)
    {
      //if (oldHeader != null)
      //	RemoveLogicalChild(oldHeader);

      //if (newHeader != null)
      //	AddLogicalChild(newHeader);
    }

    private void OnRibbonChanged(RibbonControl oldRibbon, RibbonControl newRibbon)
    {
      if (oldRibbon != null)
        oldRibbon.Presenter = null;

      if (newRibbon != null)
        newRibbon.Presenter = this;
    }

    private void OnTitleChanged(object oldTitle, object newTitle)
    {
      //if (oldTitle != null)
      //	RemoveLogicalChild(oldTitle);

      //if (newTitle != null)
      //	AddLogicalChild(newTitle);
    }

    #endregion
  }

  public sealed class RibbonPresenterTemplateContract : TemplateContract
  {
  }
}