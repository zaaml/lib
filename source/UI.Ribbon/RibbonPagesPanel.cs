// <copyright file="RibbonPagesPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonPagesPanel : ItemsPanel<RibbonPage>, IFlexPanel
  {
    #region Ctors

    public RibbonPagesPanel()
    {
      Layout = new FlexPanelLayout(this);
    }

    #endregion

    #region Properties

    private FlexPanelLayout Layout { get; }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      return Layout.Arrange(finalSize);
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      return Layout.Measure(availableSize);
    }

    #endregion

    #region Interface Implementations

    #region IFlexPanel

    IFlexDistributor IFlexPanel.Distributor => FlexDistributor.Equalizer;

    bool IFlexPanel.HasHiddenChildren { get; set; }

    double IFlexPanel.Spacing => 0.0;

    FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

    FlexElement IFlexPanel.GetFlexElement(UIElement child)
    {
      return child.GetFlexElement(this).WithStretchDirection(FlexStretchDirection.Shrink);
    }

    bool IFlexPanel.GetIsHidden(UIElement child)
    {
      return FlexPanel.GetIsHidden(child);
    }

    void IFlexPanel.SetIsHidden(UIElement child, bool value)
    {
      FlexPanel.SetIsHidden(child, value);
    }

    #endregion

    #region IOrientedPanel

    Orientation IOrientedPanel.Orientation => Orientation.Horizontal;

    #endregion

    #endregion
  }
}