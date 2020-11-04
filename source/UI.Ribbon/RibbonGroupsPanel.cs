// <copyright file="RibbonGroupsPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonGroupsPanel : ItemsPanel<RibbonGroup>
  {
    #region Fields

    private readonly RibbonGroup _phantomGroup;
    internal bool OddMeasure;

    #endregion

    #region Ctors

    public RibbonGroupsPanel()
    {
      _phantomGroup = new RibbonGroup
      {
        Header = "RibbonGroup",
        Items =
        {
          new RibbonButton
          {
            LargeIcon = WriteableBitmapUtils.Create(32, 32),
            SmallIcon = WriteableBitmapUtils.Create(16, 16),
            Text = "Ribbon Item",
            ActualItemStyle = RibbonItemStyle.Large
          }
        }
      };
    }

    #endregion

    #region Properties

    private IEnumerable<RibbonGroup> ActualGroupSizeReductionOrder => Page?.ActualGroupSizeReductionOrder.ToList() ?? Groups.Reverse();

    private IEnumerable<RibbonGroup> Groups => Children.Cast<RibbonGroup>().WhereNot(c => ReferenceEquals(c, _phantomGroup));

    internal RibbonGroupsPresenter GroupsPresenter { get; set; }

    internal RibbonPage Page => GroupsPresenter.Page;

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      _phantomGroup.Arrange(new Rect(0, 0, 0, _phantomGroup.DesiredSize.Height));

      this.ArrangeStackLine(Orientation.Horizontal, new Range<int>(0, Children.Count), 0, 0, finalSize.Height, null);

      return finalSize;
    }

    private Size GetFinalSize(Size availableSize, Size measureSize)
    {
      if (availableSize.Width.IsPositiveInfinity() == false)
        measureSize.Width = availableSize.Width;

      return measureSize;
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      Children.Add(_phantomGroup);

      _phantomGroup.BeginMeasurePass();
      _phantomGroup.Invalidate();
      _phantomGroup.Measure(new Size(0, double.PositiveInfinity));
      _phantomGroup.EndMeasurePass();

      var largeItemDesiredSize = _phantomGroup.DesiredSize;

      Children.Remove(_phantomGroup);

      if (Children.Count == 0)
        return new Size(0, largeItemDesiredSize.Height);

      var reductionOrder = ActualGroupSizeReductionOrder.ToList();

      OddMeasure = false;

      try
      {
        foreach (var ribbonGroup in Groups)
          ribbonGroup.BeginMeasurePass();

        var nextReduceGroupIndex = 0;
        OrientedSize finalSize;

        var reductionLevel = 0;

        do
        {
          finalSize = new OrientedSize(Orientation.Horizontal);

          var measureItemSize = new Size(double.PositiveInfinity, largeItemDesiredSize.Height);

          if (OddMeasure)
            measureItemSize.Height += 1;

          foreach (var ribbonGroup in Groups)
          {
            ribbonGroup.InvalidateInt(this);
            finalSize = finalSize.StackSize(ribbonGroup.MeasureInt(measureItemSize, false));
          }

          OddMeasure = !OddMeasure;

          if (finalSize.Direct.IsLessThanOrClose(availableSize.Width, XamlConstants.LayoutComparisonPrecision))
            return GetFinalSize(availableSize, new Size(finalSize.Width, largeItemDesiredSize.Height));

          foreach (var ribbonGroup in ActualGroupSizeReductionOrder)
          {
            var shrink = ribbonGroup.Shrink(finalSize.Direct - availableSize.Width);

            if (shrink.IsZero(XamlConstants.LayoutComparisonPrecision))
              continue;

            finalSize.Direct -= shrink;

            if (finalSize.Direct.IsLessThanOrClose(availableSize.Width, XamlConstants.LayoutComparisonPrecision))
              return GetFinalSize(availableSize, new Size(finalSize.Width, largeItemDesiredSize.Height));
          }

          reductionLevel++;
        } while (ReduceNextGroup(reductionOrder, ref nextReduceGroupIndex));

        return GetFinalSize(availableSize, new Size(finalSize.Width, largeItemDesiredSize.Height));
      }
      finally
      {
        for (var i = 0; i < (OddMeasure ? 2 : 1); i++)
        {
          var measureItemSize = new Size(double.PositiveInfinity, largeItemDesiredSize.Height);
          if (OddMeasure)
            measureItemSize.Height += 1;

          OddMeasure = !OddMeasure;

          foreach (var ribbonGroup in Groups)
          {
            ribbonGroup.InvalidateInt(this);
            ribbonGroup.MeasureInt(measureItemSize, true);
          }
        }

        foreach (var ribbonGroup in Groups)
          ribbonGroup.EndMeasurePass();
      }
    }

    internal void OnGroupAdded(RibbonGroup ribbonGroup)
    {
      Children.Add(ribbonGroup);
    }

    internal void OnGroupRemoved(RibbonGroup ribbonGroup)
    {
      Children.Remove(ribbonGroup);
    }

    private static bool ReduceNextGroup(IList<RibbonGroup> groups, ref int index)
    {
      var count = groups.Count;

      for (var i = 0; i < count; i++, index++)
      {
        if (index < 0 || index >= count)
          index = 0;

        var ribbonGroup = groups[index];
        if (ribbonGroup.ReduceLevel >= ribbonGroup.ReduceLevelsCount - 1) continue;

        ribbonGroup.ReduceLevel++;
        index++;
        return true;
      }

      return false;
    }

    #endregion
  }
}