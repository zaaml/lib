// <copyright file="AutoHideTabViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Docking
{
  [TemplateContractType(typeof(AutoHideTabViewControlTemplateContract))]
  public sealed class AutoHideTabViewControl : TemplateContractControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty DockContentProperty = DPM.Register<object, AutoHideTabViewControl>
      ("DockContent");

    #endregion

    #region Ctors

    static AutoHideTabViewControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<AutoHideTabViewControl>();
    }

    public AutoHideTabViewControl()
    {
      this.OverrideStyleKey<AutoHideTabViewControl>();
    }

    #endregion

    #region Properties

    private List<AutoHideTabViewItem> AutoHideTabViewItems { get; } = new List<AutoHideTabViewItem>();

    public object DockContent
    {
      get => GetValue(DockContentProperty);
      set => SetValue(DockContentProperty, value);
    }

    private AutoHideTabViewControlTemplateContract TemplateContract => (AutoHideTabViewControlTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    public void AddItem(DockItem item)
    {
      var autoHideTabViewItem = item.AutoHideTabViewItem;

      autoHideTabViewItem.AutoHideTabViewControl = this;

      AutoHideTabViewItems.Add(autoHideTabViewItem);

      if (IsTemplateAttached)
        AttachItem(autoHideTabViewItem);

      autoHideTabViewItem.AttachItem();
    }

    private void AttachItem(AutoHideTabViewItem autoHideTabViewItem)
    {
      GetItemsPresenter(autoHideTabViewItem)?.AddItem(autoHideTabViewItem);

      TemplateContract.DockItemHost.Children.Add(autoHideTabViewItem.ItemPresenter);
    }

    private void AttachItems()
    {
      foreach (var autoHideTabViewItem in AutoHideTabViewItems)
        AttachItem(autoHideTabViewItem);
    }

    private void DetachItem(AutoHideTabViewItem autoHideTabViewItem)
    {
      GetItemsPresenter(autoHideTabViewItem)?.RemoveItem(autoHideTabViewItem);

      TemplateContract.DockItemHost.Children.Remove(autoHideTabViewItem.ItemPresenter);
    }

    private void DetachItems()
    {
      foreach (var autoHideTabViewItem in AutoHideTabViewItems)
        DetachItem(autoHideTabViewItem);
    }

    private AutoHideTabViewItemsPresenter GetItemsPresenter(AutoHideTabViewItem item)
    {
      return GetItemsPresenter(AutoHideLayout.GetDockSide(item.DockItem));
    }

    private AutoHideTabViewItemsPresenter GetItemsPresenter(Dock dockSide)
    {
      if (TemplateContract == null)
        return null;

      switch (dockSide)
      {
        case Dock.Left:

          return TemplateContract.LeftItemsPresenter;

        case Dock.Top:

          return TemplateContract.TopItemsPresenter;

        case Dock.Right:

          return TemplateContract.RightItemsPresenter;

        case Dock.Bottom:

          return TemplateContract.BottomItemsPresenter;

        default:

          throw new ArgumentOutOfRangeException();
      }
    }

    public void OnItemDockSideChanged(AutoHideTabViewItem item, Dock oldDockSide, Dock newDockSide)
    {
      GetItemsPresenter(oldDockSide)?.RemoveItem(item);
      GetItemsPresenter(newDockSide)?.AddItem(item);
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

    public void RemoveItem(DockItem item)
    {
      var autoHideTabViewItem = item.AutoHideTabViewItem;

      autoHideTabViewItem.AutoHideTabViewControl = null;

      if (IsTemplateAttached)
        DetachItem(autoHideTabViewItem);

      AutoHideTabViewItems.Remove(autoHideTabViewItem);

      autoHideTabViewItem.DetachItem();
    }

    #endregion
  }

  public sealed class AutoHideTabViewControlTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public AutoHideTabViewItemsPresenter BottomItemsPresenter { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public Panel DockItemHost { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public AutoHideTabViewItemsPresenter LeftItemsPresenter { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public AutoHideTabViewItemsPresenter RightItemsPresenter { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public AutoHideTabViewItemsPresenter TopItemsPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}