// <copyright file="RibbonItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels;

namespace Zaaml.UI.Controls.Ribbon
{
  internal struct RibbonItemGroup
  {
    #region Fields

    private ulong _packedValue;

    #endregion

    #region Properties

    public RibbonItem Item1 { get; private set; }

    public RibbonItem Item2 { get; private set; }

    public RibbonItem Item3 { get; private set; }

    public RibbonItemStyle Item1Style
    {
      get => PackedDefinition.Item1Style.GetValue(_packedValue);
      private set => PackedDefinition.Item1Style.SetValue(ref _packedValue, value);
    }

    public RibbonItemStyle Item2Style
    {
      get => PackedDefinition.Item2Style.GetValue(_packedValue);
      private set => PackedDefinition.Item2Style.SetValue(ref _packedValue, value);
    }

    public RibbonItemStyle Item3Style
    {
      get => PackedDefinition.Item3Style.GetValue(_packedValue);
      private set => PackedDefinition.Item3Style.SetValue(ref _packedValue, value);
    }

    public byte Item1ReduceLevel
    {
      get => PackedDefinition.Item1ReduceLevel.GetValue(_packedValue);
      private set => PackedDefinition.Item1ReduceLevel.SetValue(ref _packedValue, value);
    }

    public byte Item2ReduceLevel
    {
      get => PackedDefinition.Item2ReduceLevel.GetValue(_packedValue);
      private set => PackedDefinition.Item2ReduceLevel.SetValue(ref _packedValue, value);
    }

    public byte Item3ReduceLevel
    {
      get => PackedDefinition.Item3ReduceLevel.GetValue(_packedValue);
      private set => PackedDefinition.Item3ReduceLevel.SetValue(ref _packedValue, value);
    }

    public short ReduceLevel
    {
      get => PackedDefinition.ReduceLevel.GetValue(_packedValue);
      private set => PackedDefinition.ReduceLevel.SetValue(ref _packedValue, value);
    }

    private RibbonControlGroupItemPosition NextReduceItem
    {
      get => PackedDefinition.NextReduceItem.GetValue(_packedValue);
      set => PackedDefinition.NextReduceItem.SetValue(ref _packedValue, value);
    }

    public RibbonControlGroupSize Size
    {
      get => PackedDefinition.Size.GetValue(_packedValue);
      private set => PackedDefinition.Size.SetValue(ref _packedValue, value);
    }

    public RibbonItemStyle Style
    {
      get => PackedDefinition.Style.GetValue(_packedValue);
      private set => PackedDefinition.Style.SetValue(ref _packedValue, value);
    }

    public int ReduceLevelCount
    {
      get
      {
        var style = Style;

        switch (style)
        {
          case RibbonItemStyle.Default:
            return 0;
          case RibbonItemStyle.Custom:
            return EnumerateItems().Cast<IRibbonCustomLayoutItem>().Sum(i => i.ReduceLevelsCount);
          default:
            return RibbonUtils.GetStyleSize(style) + 1;
        }
      }
    }

    public bool CanReduce => ReduceLevel + 1 < ReduceLevelCount && ReduceLevelCount > 0;

    public Orientation Orientation
    {
      get
      {
        if (Style == RibbonItemStyle.Custom)
          return Orientation.Horizontal;

        switch (Size)
        {
          case RibbonControlGroupSize.Small:
            return Orientation.Vertical;
          case RibbonControlGroupSize.Large:
          case RibbonControlGroupSize.LargeAndSmall:
            return Orientation.Horizontal;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    public bool IsFull => Item3 != null;

    public bool IsEmpty => Item1 == null;

    #endregion

    #region  Methods

    public RibbonItemGroup Reduce()
    {
      if (CanReduce == false)
        throw new InvalidOperationException("Group can not be reduced");

      return Style == RibbonItemStyle.Custom ? CustomReduce() : DefaultReduce();
    }

    private RibbonItemGroup DefaultReduce()
    {
      var clone = Clone();

      if (Item1 != null)
        clone.Item1Style = RibbonUtils.Reduce(Item1Style);
      if (Item2 != null)
        clone.Item2Style = RibbonUtils.Reduce(Item2Style);
      if (Item3 != null)
        clone.Item3Style = RibbonUtils.Reduce(Item3Style);

      clone.Size = RibbonControlGroupSize.Small;
      clone.Style = RibbonUtils.Reduce(Style);

      return clone;
    }

    private RibbonItem GetItem(RibbonControlGroupItemPosition itemPosition)
    {
      switch (itemPosition)
      {
        case RibbonControlGroupItemPosition.Item3:
          return Item3;
        case RibbonControlGroupItemPosition.Item2:
          return Item2;
        case RibbonControlGroupItemPosition.Item1:
          return Item1;
        default:
          throw new ArgumentOutOfRangeException(nameof(itemPosition));
      }
    }

    private byte GetCustomReduceLevel(RibbonControlGroupItemPosition itemPosition)
    {
      switch (itemPosition)
      {
        case RibbonControlGroupItemPosition.Item3:
          return Item3ReduceLevel;
        case RibbonControlGroupItemPosition.Item2:
          return Item2ReduceLevel;
        case RibbonControlGroupItemPosition.Item1:
          return Item1ReduceLevel;
        default:
          throw new ArgumentOutOfRangeException(nameof(itemPosition));
      }
    }

    private void SetCustomReduceLevel(RibbonControlGroupItemPosition itemPosition, byte level)
    {
      switch (itemPosition)
      {
        case RibbonControlGroupItemPosition.Item3:
          Item3ReduceLevel = level;
          break;
        case RibbonControlGroupItemPosition.Item2:
          Item2ReduceLevel = level;
          break;
        case RibbonControlGroupItemPosition.Item1:
          Item1ReduceLevel = level;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(itemPosition));
      }
    }

    private RibbonItemGroup Clone()
    {
      return new RibbonItemGroup
      {
        Item1 = Item1,
        Item1Style = Item1Style,
        Item1ReduceLevel = Item1ReduceLevel,
        Item2 = Item2,
        Item2Style = Item2Style,
        Item2ReduceLevel = Item2ReduceLevel,
        Item3 = Item3,
        Item3Style = Item3Style,
        Item3ReduceLevel = Item3ReduceLevel,
        Size = Size,
        Style = Style,
        NextReduceItem = NextReduceItem,
        ReduceLevel = ReduceLevel
      };
    }

    private RibbonItemGroup CustomReduce()
    {
      var nextReduceItem = NextReduceItem;

      for (var i = 0; i < 3; i++)
      {
        var item = (IRibbonCustomLayoutItem) GetItem(nextReduceItem);
        var currentReduceItem = nextReduceItem;
        nextReduceItem = GetNextReduceItem(nextReduceItem);

        if (item == null)
          continue;

        var currentReduceLevel = GetCustomReduceLevel(currentReduceItem);
        if (currentReduceLevel + 1 >= item.ReduceLevelsCount)
          continue;

        var clone = Clone();
        clone.SetCustomReduceLevel(currentReduceItem, (byte) (currentReduceLevel + 1));
        clone.NextReduceItem = nextReduceItem;
        clone.ReduceLevel++;

        return clone;
      }

      throw new InvalidOperationException("Group can not be reduced");
    }

    private RibbonControlGroupItemPosition GetNextReduceItem(RibbonControlGroupItemPosition itemPosition)
    {
      switch (itemPosition)
      {
        case RibbonControlGroupItemPosition.Item3:
          return RibbonControlGroupItemPosition.Item2;
        case RibbonControlGroupItemPosition.Item2:
          return RibbonControlGroupItemPosition.Item1;
        case RibbonControlGroupItemPosition.Item1:
          return RibbonControlGroupItemPosition.Item3;
        default:
          throw new ArgumentOutOfRangeException(nameof(itemPosition));
      }
    }

    public bool Add(RibbonItem item, RibbonItemStyle itemStyle)
    {
      if (itemStyle == RibbonItemStyle.Default)
        throw new ArgumentOutOfRangeException(nameof(itemStyle));

      if (IsFull)
        return false;

      if (itemStyle == RibbonItemStyle.Custom)
      {
        if (IsEmpty || Style == RibbonItemStyle.Custom)
          AddImpl(item, itemStyle);
        else
          return false;

        Style = RibbonItemStyle.Custom;

        return true;
      }

      if (Style == RibbonItemStyle.Custom)
        return false;

      var allowedGroupSize = item.GetAllowedGroupSize(itemStyle);
      if ((IsEmpty || allowedGroupSize == Size || (Size == RibbonControlGroupSize.LargeAndSmall && allowedGroupSize == RibbonControlGroupSize.Large)) == false)
        return false;

      AddImpl(item, itemStyle);

      Size = allowedGroupSize;
      if (Style == RibbonItemStyle.Default || RibbonUtils.GetStyleSize(Style) < RibbonUtils.GetStyleSize(itemStyle))
        Style = itemStyle;

      return true;
    }

    private void AddImpl(RibbonItem item, RibbonItemStyle itemStyle)
    {
      if (Item1 == null)
      {
        Item1 = item;
        Item1Style = itemStyle;
        NextReduceItem = RibbonControlGroupItemPosition.Item1;
      }
      else if (Item2 == null)
      {
        Item2 = item;
        Item2Style = itemStyle;
        NextReduceItem = RibbonControlGroupItemPosition.Item2;
      }
      else if (Item3 == null)
      {
        Item3 = item;
        Item3Style = itemStyle;
        NextReduceItem = RibbonControlGroupItemPosition.Item3;
      }
    }

    public IEnumerable<RibbonItem> EnumerateItems()
    {
      if (Item3 != null)
        yield return Item3;

      if (Item2 != null)
        yield return Item2;

      if (Item1 != null)
        yield return Item1;
    }

    public override string ToString()
    {
      return "{" + string.Join(", ", EnumerateItems().Select(i => i.Text ?? "Undefined")) + "}";
    }

    public RibbonItemStyle GetItemStyle(RibbonControlGroupItemPosition position)
    {
      switch (position)
      {
        case RibbonControlGroupItemPosition.Item3:
          return Item3Style;
        case RibbonControlGroupItemPosition.Item2:
          return Item2Style;
        case RibbonControlGroupItemPosition.Item1:
          return Item1Style;
        default:
          throw new ArgumentOutOfRangeException(nameof(position));
      }
    }

    public Size MeasureGroup(Size measureItemSize, bool isFinalMeasure)
    {
      var orientedSize = new OrientedSize(Orientation);

      orientedSize = orientedSize.StackSize(MeasureItem(RibbonControlGroupItemPosition.Item1, measureItemSize, isFinalMeasure, false));
      if (Item2 != null)
        orientedSize = orientedSize.StackSize(MeasureItem(RibbonControlGroupItemPosition.Item2, measureItemSize, isFinalMeasure, false));
      if (Item3 != null)
        orientedSize = orientedSize.StackSize(MeasureItem(RibbonControlGroupItemPosition.Item3, measureItemSize, isFinalMeasure, false));

      return orientedSize.Size;
    }

    private Size MeasureItem(RibbonControlGroupItemPosition position, Size measureItemSize, bool isFinalMeasure, bool forceMeasure)
    {
      var item = GetItem(position);
      var itemStyle = GetItemStyle(position);
      var reduceLevel = GetCustomReduceLevel(position);
      var itemMeasurement = item.ItemMeasurement;

      if (itemStyle == RibbonItemStyle.Custom)
      {
        var size = itemMeasurement.GetCustomSize(reduceLevel);
        forceMeasure |= size.IsEmpty;

        if (forceMeasure == false && isFinalMeasure == false)
          return size;

        var layoutItem = item.DirectCast<IRibbonCustomLayoutItem>();
        if (isFinalMeasure && forceMeasure == false && layoutItem.ReduceLevel == reduceLevel)
          item.Measure(size);
        else
        {
          layoutItem.ReduceLevel = reduceLevel;
          item.Measure(measureItemSize);
          size = item.DesiredSize;
          itemMeasurement.SetCustomSize(reduceLevel, size);
        }

        return size;
      }
      else
      {
        var size = itemMeasurement.GetSize(itemStyle);
        forceMeasure |= size.IsEmpty;

        if (forceMeasure == false && isFinalMeasure == false)
          return size;

        if (isFinalMeasure && forceMeasure == false && item.ActualItemStyle == itemStyle)
          item.Measure(size);
        else
        {
          item.ActualItemStyle = itemStyle;
          item.Measure(measureItemSize);
          size = item.DesiredSize;
          itemMeasurement.SetSize(itemStyle, size);
        }

        return size;
      }
    }

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedEnumItemDefinition<RibbonItemStyle> Item1Style;
      public static readonly PackedEnumItemDefinition<RibbonItemStyle> Item2Style;
      public static readonly PackedEnumItemDefinition<RibbonItemStyle> Item3Style;

      public static readonly PackedByteItemDefinition Item1ReduceLevel;
      public static readonly PackedByteItemDefinition Item2ReduceLevel;
      public static readonly PackedByteItemDefinition Item3ReduceLevel;

      public static readonly PackedShortItemDefinition ReduceLevel;

      public static readonly PackedEnumItemDefinition<RibbonControlGroupItemPosition> NextReduceItem;

      public static readonly PackedEnumItemDefinition<RibbonControlGroupSize> Size;
      public static readonly PackedEnumItemDefinition<RibbonItemStyle> Style;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        Item1Style = allocator.AllocateEnumItem<RibbonItemStyle>();
        Item2Style = allocator.AllocateEnumItem<RibbonItemStyle>();
        Item3Style = allocator.AllocateEnumItem<RibbonItemStyle>();

        Item1ReduceLevel = allocator.AllocateByteItem();
        Item2ReduceLevel = allocator.AllocateByteItem();
        Item3ReduceLevel = allocator.AllocateByteItem();

        ReduceLevel = allocator.AllocateShortItem();

        NextReduceItem = allocator.AllocateEnumItem<RibbonControlGroupItemPosition>();

        Style = allocator.AllocateEnumItem<RibbonItemStyle>();
        Size = allocator.AllocateEnumItem<RibbonControlGroupSize>();
      }

      #endregion
    }

    #endregion

    private static bool IsFlexible(FrameworkElement item)
    {
      return item.Width.IsNaN() && item.MaxWidth.IsPositiveInfinity() == false;
    }

    private double ShrinkItem(RibbonControlGroupItemPosition position, double target)
    {
      var item = GetItem(position);

      if (item == null || IsFlexible(item) == false)
        return 0;

      var reduceLevel = GetCustomReduceLevel(position);
      var size = item.ItemMeasurement.GetCustomSize(reduceLevel);
      var width = size.Width;
      if (width.IsGreaterThan(item.MinWidth))
      {
        var diff = Math.Min(target, width - item.MinWidth);
        size.Width -= diff;
        MeasureItem(position, size, false, true);

        return diff;
      }

      return 0;
    }

    public double Shrink(double target)
    {
      if (Style != RibbonItemStyle.Custom)
        return 0;

      var initial = target;

      target -= ShrinkItem(RibbonControlGroupItemPosition.Item3, target);

      if (target.IsZero(XamlConstants.LayoutComparisonPrecision))
        return initial;

      target -= ShrinkItem(RibbonControlGroupItemPosition.Item2, target);
      if (target.IsZero(XamlConstants.LayoutComparisonPrecision))
        return initial;

      target -= ShrinkItem(RibbonControlGroupItemPosition.Item1, target);
      if (target.IsZero(XamlConstants.LayoutComparisonPrecision))
        return initial;

      return initial - target;
    }
  }

  internal enum RibbonControlGroupItemPosition
  {
    Item3,
    Item2,
    Item1
  }
}