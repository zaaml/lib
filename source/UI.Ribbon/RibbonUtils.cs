// <copyright file="RibbonUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.UI.Controls.Ribbon
{
  internal static class RibbonUtils
  {
    #region  Methods

    public static bool CanBeLarge(RibbonItemStyle itemStyle)
    {
      return itemStyle == RibbonItemStyle.Large || itemStyle == RibbonItemStyle.Default;
    }

    public static bool CanBeLarge(RibbonControlGroupSize groupSize)
    {
      return groupSize == RibbonControlGroupSize.Large || groupSize == RibbonControlGroupSize.LargeAndSmall;
    }

    public static bool CanBeLarge(RibbonItem item)
    {
      var sizeDefinition = item.SizeDefinition;
      if (sizeDefinition == null)
        return item.LargeIcon != null;

      return CanBeLarge(sizeDefinition.ItemStyle);
    }

    public static bool CanBeSmall(RibbonControlGroupSize groupSize)
    {
      return groupSize == RibbonControlGroupSize.Small || groupSize == RibbonControlGroupSize.LargeAndSmall;
    }

    public static bool CanBeSmall(RibbonItemStyle itemStyle)
    {
      return itemStyle == RibbonItemStyle.Small || itemStyle == RibbonItemStyle.Medium || itemStyle == RibbonItemStyle.Default;
    }

    public static bool CanBeSmall(RibbonItem item)
    {
      var sizeDefinition = item.SizeDefinition;
      if (sizeDefinition == null)
        return item.SmallIcon != null;

      return CanBeSmall(sizeDefinition.ItemStyle);
    }

    public static RibbonItemStyle CoerceItemStyle(RibbonItem item, RibbonItemStyle itemStyle)
    {
      if (item is IRibbonCustomLayoutItem)
        return RibbonItemStyle.Custom;

      if (itemStyle == RibbonItemStyle.Default)
        itemStyle = item.GetDefaultRibbonItemStyle();

      if (itemStyle != RibbonItemStyle.Default)
        return itemStyle;

      var canBeLarge = item.CanBeLarge();
      var canBeSmall = item.CanBeSmall();

      if (canBeLarge && canBeSmall)
        itemStyle = RibbonItemStyle.Large;
      else if (canBeLarge == false)
        itemStyle = RibbonItemStyle.Medium;
      else
        itemStyle = RibbonItemStyle.Large;

      return itemStyle;
    }

    public static RibbonItemStyle GetActualItemStyle(RibbonItem item)
    {
      return item.SizeDefinition?.ItemStyle ?? item.ActualItemStyle;
    }

    public static RibbonControlGroupSize GetAllowedGroupSize(RibbonItem item, RibbonItemStyle itemStyle)
    {
      return itemStyle != RibbonItemStyle.Default ? GetAllowedGroupSize(itemStyle) : GetAllowedGroupSize(CanBeSmall(item), CanBeLarge(item));
    }

    public static RibbonControlGroupSize GetAllowedGroupSize(RibbonItemStyle itemStyle)
    {
      return GetAllowedGroupSize(CanBeSmall(itemStyle), CanBeLarge(itemStyle));
    }

    public static RibbonControlGroupSize GetAllowedGroupSize(bool canBeSmall, bool canBeLarge)
    {
      if (canBeLarge && canBeSmall)
        return RibbonControlGroupSize.LargeAndSmall;
      if (canBeLarge)
        return RibbonControlGroupSize.Large;
      if (canBeSmall)
        return RibbonControlGroupSize.Small;

      throw new ArgumentException();
    }

    public static RibbonItemStyle GetCoercedActualItemStyle(RibbonItem item)
    {
      return CoerceItemStyle(item, GetActualItemStyle(item));
    }

    public static int GetStyleSize(RibbonItemStyle itemStyle)
    {
      switch (itemStyle)
      {
        case RibbonItemStyle.Default:
          return int.MaxValue;
        case RibbonItemStyle.Large:
          return 2;
        case RibbonItemStyle.Medium:
          return 1;
        case RibbonItemStyle.Small:
          return 0;
        case RibbonItemStyle.Custom:
          return -1;
        default:
          throw new ArgumentOutOfRangeException(nameof(itemStyle));
      }
    }

    public static RibbonControlGroupCollection GroupItems(IList<RibbonItem> ribbonItems, Func<RibbonItem, RibbonItemStyle> itemStyleProvider)
    {
      var actualItemStyleProvider = itemStyleProvider ?? GetActualItemStyle;
      var items = ribbonItems;
      var list = new RibbonControlGroupCollection();

      var group = new RibbonItemGroup();

      for (var i = items.Count - 1; i >= 0; i--)
      {
        var item = items[i];
        var itemStyle = CoerceItemStyle(item, actualItemStyleProvider(item));

        if (group.Add(item, itemStyle)) continue;

        list.Add(group);
        group = new RibbonItemGroup();
        group.Add(item, itemStyle);
      }

      if (group.IsEmpty == false)
        list.Add(group);

      list.Reverse();

      return list;
    }

    public static bool IsStyleAllowed(RibbonItem item, RibbonItemStyle itemStyle)
    {
      if (itemStyle == RibbonItemStyle.Default)
        return true;

      var sizeDefinition = item.SizeDefinition;
      if (sizeDefinition != null && sizeDefinition.ItemStyle != RibbonItemStyle.Default)
        return sizeDefinition.ItemStyle == itemStyle;

      switch (itemStyle)
      {
        case RibbonItemStyle.Large:
          return CanBeLarge(item);
        case RibbonItemStyle.Medium:
        case RibbonItemStyle.Small:
          return CanBeSmall(item);
      }

      return false;
    }

    public static RibbonItemStyle Reduce(RibbonItemStyle itemStyle)
    {
      switch (itemStyle)
      {
        case RibbonItemStyle.Large:
          return RibbonItemStyle.Medium;
        case RibbonItemStyle.Medium:
          return RibbonItemStyle.Small;
        default:
          throw new ArgumentOutOfRangeException(nameof(itemStyle));
      }
    }

    #endregion
  }
}