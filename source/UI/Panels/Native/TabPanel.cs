// <copyright file="TabPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Panels.Native
{
  public sealed class TabPanel : System.Windows.Controls.Primitives.TabPanel
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ActualTabStripPlacementPropertyKey = DPM.RegisterAttachedReadOnly<Dock, TabPanel>
      ("ActualTabStripPlacement", Dock.Top);

    public static DependencyProperty ActualTabStripPlacementProperty = ActualTabStripPlacementPropertyKey.DependencyProperty;

    public static readonly DependencyProperty TabStripPlacementProperty = DPM.Register<Dock, TabPanel>
      ("TabStripPlacement", Dock.Top, t => t.InvalidateMeasure);

    #endregion

    #region Properties

    public Dock TabStripPlacement
    {
      get => (Dock) GetValue(TabStripPlacementProperty);
      set => SetValue(TabStripPlacementProperty, value);
    }

    #endregion

    #region  Methods

    public static Dock GetActualTabStripPlacement(FrameworkElement element)
    {
      return (Dock) element.GetValue(ActualTabStripPlacementProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var tabStripPlacement = TabStripPlacement;

      for (int i = 0, count = Children.Count; i < count; i++)
      {
        var tabItem = Children[i] as TabItem;
        tabItem?.SetReadOnlyValue(ActualTabStripPlacementPropertyKey, tabStripPlacement);
      }

      return base.MeasureOverride(availableSize);
    }

    #endregion
  }
}