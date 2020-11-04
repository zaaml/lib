// <copyright file="RibbonItemsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonItemsPanel : ItemsPanel<RibbonItem>
  {
    #region Fields

    private readonly List<RibbonItemGroup> _groups = new List<RibbonItemGroup>();

    #endregion

    #region Properties

    internal RibbonGroup Group => ItemsPresenter?.Group;

    private bool IsFullMeasurePass => Group?.IsFullMeasurePass == true;

    internal RibbonItemsPresenter ItemsPresenter { get; set; }

    #endregion

    #region  Methods

    private static double ArrangeControlGroup(RibbonItemGroup itemGroup, Size finalSize, double offset)
    {
      var orientation = itemGroup.Orientation;

      switch (orientation)
      {
        case Orientation.Vertical:
          var lineWidth = itemGroup.EnumerateItems().Max(i => i.DesiredSize.Width);
          return offset + PanelHelper.ArrangeStackLine(itemGroup.EnumerateItems(), orientation, offset, 0, lineWidth, finalSize.Height / 3).Indirect;
        case Orientation.Horizontal:
          return offset + PanelHelper.ArrangeStackLine(itemGroup.EnumerateItems(), orientation, 0, offset, finalSize.Height, null).Direct;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      if (_groups.Any())
        _groups.Aggregate(0.0, (current, controlGroup) => ArrangeControlGroup(controlGroup, finalSize, current));
      else
        this.ArrangeStackLine(Orientation.Horizontal, new Range<int>(0, Children.Count), 0, 0, finalSize.Height, null);

      return finalSize;
    }

    private static Size MeasureControlGroup(RibbonItemGroup itemGroup, Size measureItemSize, bool forceMeasure)
    {
      return itemGroup.MeasureGroup(measureItemSize, forceMeasure);
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      var orientedSize = new OrientedSize(Orientation.Horizontal);
      var measureItemSize = new Size(double.PositiveInfinity, availableSize.Height);

      _groups.Clear();

      if (IsFullMeasurePass == false)
      {
        foreach (var child in Children.Cast<RibbonItem>())
        {
          child.ActualItemStyle = RibbonItemStyle.Large;
          child.Measure(measureItemSize);

          var childDesiredSize = child.DesiredSize;

          orientedSize = orientedSize.StackSize(childDesiredSize);
        }
      }
      else
      {
        _groups.AddRange(Group.GetGroups());
        orientedSize = _groups.Aggregate(orientedSize, (current, group) => current.StackSize(MeasureControlGroup(group, measureItemSize, Group.IsFinalMeasure)));
      }

      return orientedSize.Size;
    }

    #endregion
  }
}