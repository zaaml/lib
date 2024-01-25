// <copyright file="AutoHideTabViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Docking
{
  [TemplateContractType(typeof(AutoHideTabViewItemsPresenterTemplateContract))]
  public sealed class AutoHideTabViewItemsPresenter : TemplateContractControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, AutoHideTabViewItemsPresenter>
      ("Orientation");

    #endregion

    #region Ctors

    static AutoHideTabViewItemsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<AutoHideTabViewItemsPresenter>();
    }

    public AutoHideTabViewItemsPresenter()
    {
      this.OverrideStyleKey<AutoHideTabViewItemsPresenter>();
    }

    #endregion

    #region Properties

    private List<AutoHideTabViewItem> AutoHideTabViewItems { get; } = new List<AutoHideTabViewItem>();

    private Panel ItemsHost => TemplateContract.ItemsHost;

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    private AutoHideTabViewItemsPresenterTemplateContract TemplateContract => (AutoHideTabViewItemsPresenterTemplateContract) TemplateContractCore;

    #endregion

    #region  Methods

    public void AddItem(AutoHideTabViewItem autoHideTabViewItem)
    {
      AutoHideTabViewItems.Add(autoHideTabViewItem);
      ItemsHost?.Children.Add(autoHideTabViewItem);
    }

    private void AttachItems()
    {
      foreach (var autoHideTabViewItem in AutoHideTabViewItems)
        ItemsHost.Children.Add(autoHideTabViewItem);
    }

    private void DetachItems()
    {
      foreach (var autoHideTabViewItem in AutoHideTabViewItems)
        ItemsHost.Children.Remove(autoHideTabViewItem);
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      AttachItems();
    }

    protected override void OnTemplateContractDetaching()
    {
      DetachItems();

      base.OnTemplateContractDetaching();
    }

    public void RemoveItem(AutoHideTabViewItem autoHideTabViewItem)
    {
      ItemsHost?.Children.Remove(autoHideTabViewItem);
      AutoHideTabViewItems.Remove(autoHideTabViewItem);
    }

    #endregion
  }

  public sealed class AutoHideTabViewItemsPresenterTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public Panel ItemsHost { get; [UsedImplicitly] private set; }

    #endregion
  }
}