// <copyright file="TabViewContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

namespace Zaaml.UI.Controls.TabView
{
  [TemplateContractType(typeof(TabViewControlContentPresenterTemplateContract))]
  public sealed class TabViewContentPresenter : TemplateContractControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ContentProperty = DPM.Register<object, TabViewContentPresenter>
      ("Content");

    public static readonly DependencyProperty BackContentProperty = DPM.Register<object, TabViewContentPresenter>
      ("BackContent");

    private static readonly DependencyPropertyKey TabViewControlPropertyKey = DPM.RegisterReadOnly<TabViewControl, TabViewContentPresenter>
      ("TabViewControl");

    public static readonly DependencyProperty TabViewControlProperty = TabViewControlPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static TabViewContentPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<TabViewContentPresenter>();
    }

    public TabViewContentPresenter()
    {
      this.OverrideStyleKey<TabViewContentPresenter>();
    }

    #endregion

    #region Properties

    public object BackContent
    {
      get => GetValue(BackContentProperty);
      set => SetValue(BackContentProperty, value);
    }

    public object Content
    {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
    }

    public TabViewControl TabViewControl
    {
      get => (TabViewControl) GetValue(TabViewControlProperty);
      internal set => this.SetReadOnlyValue(TabViewControlPropertyKey, value);
    }

    private TabViewControlContentPresenterTemplateContract TemplateContract => (TabViewControlContentPresenterTemplateContract) TemplateContractCore;

    #endregion
  }

  public class TabViewControlContentPresenterTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart]
    public ContentPresenter BackContentPresenter { get; [UsedImplicitly] private set; }

    [TemplateContractPart]
    public ContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}