// <copyright file="RibbonToolBarItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.Overflow;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonToolBarItemsPanel : ItemsPanel<RibbonItem>, IFlexPanel
  {
    #region Fields

    private RibbonToolBarItemsPresenter _itemsPresenter;

    #endregion

    #region Ctors

    public RibbonToolBarItemsPanel()
    {
      Layout = new FlexPanelLayout(this);
    }

    #endregion

    #region Properties

    internal bool HasOverflowChildren { get; private set; }

    internal RibbonToolBarItemsPresenter ItemsPresenter
    {
      get => _itemsPresenter;
      set
      {
        if (ReferenceEquals(_itemsPresenter, value))
          return;

        _itemsPresenter = value;

        InvalidateMeasure();
      }
    }

    private FlexPanelLayout Layout { get; }

    [UsedImplicitly]
    private RibbonToolBar ToolBar => ItemsPresenter?.ToolBar;

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      return Layout.Arrange(finalSize);
    }

    internal override ItemHostCollection<RibbonItem> CreateHostCollectionInternal()
    {
      return new RibbonToolBarItemHostCollection(this);
    }

    private static bool GetIsOverflow(UIElement child)
    {
      var overflowItem = (OverflowItem<RibbonItem>) child;
      var ribbonItem = overflowItem.Item;

      return ribbonItem.IsOverflow;
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      return Layout.Measure(availableSize);
    }

    internal bool ResumeOverflow()
    {
      var remeasure = false;

      foreach (var ribbonItem in ItemsInternal)
        remeasure |= ribbonItem.OverflowController.Resume();

      return remeasure;
    }

    private static void SetIsOverflow(UIElement child, bool value)
    {
      var overflowItem = (OverflowItem<RibbonItem>) child;
      var ribbonItem = overflowItem.Item;

      ribbonItem.IsOverflow = value;
    }

    internal void SuspendOverflow()
    {
      foreach (var ribbonItem in ItemsInternal)
        ribbonItem.OverflowController.Suspend();
    }

    #endregion

    #region Interface Implementations

    #region IFlexPanel

    IFlexDistributor IFlexPanel.Distributor => FlexDistributor.LastToFirst;

    bool IFlexPanel.HasHiddenChildren
    {
      get => HasOverflowChildren;
      set => HasOverflowChildren = value;
    }

    double IFlexPanel.Spacing => 0;

    FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

    FlexElement IFlexPanel.GetFlexElement(UIElement child)
    {
      var overflowBehavior = Children.Count > 0 && ReferenceEquals(child, Children[0]) ? FlexOverflowBehavior.Pin : FlexOverflowBehavior.Hide;

      return child.GetFlexElement(this).WithStretchDirection(FlexStretchDirection.None).WithOverflowBehavior(overflowBehavior);
    }

    bool IFlexPanel.GetIsHidden(UIElement child) => GetIsOverflow(child);

    void IFlexPanel.SetIsHidden(UIElement child, bool value) => SetIsOverflow(child, value);

    #endregion

    #region IOrientedPanel

    Orientation IOrientedPanel.Orientation => Orientation.Horizontal;

    #endregion

    #endregion

    #region  Nested Types

    private sealed class RibbonToolBarItemHostCollection : ItemHostCollection<RibbonItem>
    {
      #region Ctors

      public RibbonToolBarItemHostCollection(RibbonToolBarItemsPanel panel)
      {
        Panel = panel;
      }

      #endregion

      #region Properties

      private RibbonToolBarItemsPanel Panel { get; }

      #endregion

      #region  Methods

      protected override void ClearCore()
      {
        Panel.Children.Clear();
      }

      protected override void InitCore(ICollection<RibbonItem> items)
      {
        Panel.Children.Clear();

        foreach (var toolBarItem in items)
          Panel.Children.Add(toolBarItem.OverflowController.VisibleHost);
      }

      protected override void InsertCore(int index, RibbonItem item)
      {
        Panel.Children.Insert(index, item.OverflowController.VisibleHost);
      }

      protected override void RemoveAtCore(int index)
      {
        Panel.Children.RemoveAt(index);
      }

      #endregion
    }

    #endregion
  }
}