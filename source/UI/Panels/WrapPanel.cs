// <copyright file="WrapPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Panel = Zaaml.UI.Panels.Core.Panel;
using Range = Zaaml.Core.Range;

namespace Zaaml.UI.Panels
{
  public sealed class WrapPanel : Panel
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, WrapPanel>
      ("Orientation", w => w.InvalidateMeasure, w => OnCoerceOrientation);

    public static readonly DependencyProperty ItemWidthProperty = DPM.Register<double, WrapPanel>
      ("ItemWidth", double.NaN, w => w.InvalidateMeasure, w => OnCoerceItemSize);

    public static readonly DependencyProperty ItemHeightProperty = DPM.Register<double, WrapPanel>
      ("ItemHeight", double.NaN, w => w.InvalidateMeasure, w => OnCoerceItemSize);

    #endregion

    #region Properties

    protected override bool HasLogicalOrientation => true;

    public double ItemHeight
    {
      get => (double) GetValue(ItemHeightProperty);
      set => SetValue(ItemHeightProperty, value);
    }

    public double ItemWidth
    {
      get => (double) GetValue(ItemWidthProperty);
      set => SetValue(ItemWidthProperty, value);
    }

    protected override Orientation LogicalOrientation => Orientation;

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      var orientation = Orientation;
      var lineSize = new OrientedSize(orientation);
      var maximumSize = new OrientedSize(orientation, finalSize);

      var itemWidth = ItemWidth.IsNaN() ? (double?) null : ItemWidth;
      var itemHeight = ItemHeight.IsNaN() ? (double?) null : ItemHeight;

      var lineOffset = 0.0;
      var fixedItemSize = orientation.IsHorizontal() ? itemWidth : itemHeight;

      var children = Children;
      var count = children.Count;
      var lineStart = 0;
      for (var lineEnd = 0; lineEnd < count; lineEnd++)
      {
        var element = children[lineEnd];
        var elementSize = element.GetDesiredOrientedSize(orientation, itemWidth, itemHeight);

        if (maximumSize.Direct.IsLessThan(lineSize.Direct + elementSize.Direct, XamlConstants.LayoutComparisonPrecision))
        {
          this.ArrangeStackLine(orientation, Range.Create(lineStart, lineEnd), lineOffset, 0, lineSize.Indirect, fixedItemSize);

          lineOffset += lineSize.Indirect;
          lineSize = elementSize;

          if (maximumSize.Direct.IsLessThan(elementSize.Direct, XamlConstants.LayoutComparisonPrecision))
          {
            this.ArrangeStackLine(orientation, Range.Create(lineStart, ++lineEnd), lineOffset, 0, lineSize.Indirect, fixedItemSize);

            lineOffset += lineSize.Indirect;
            lineSize = new OrientedSize(orientation);
          }

          lineStart = lineEnd;
        }
        else
        {
          lineSize.Direct += elementSize.Direct;
          lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
        }
      }

      if (lineStart < count)
        this.ArrangeStackLine(orientation, Range.Create(lineStart, count), lineOffset, 0, lineSize.Indirect, fixedItemSize);

      return finalSize;
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      var orientation = Orientation;
      var result = new OrientedSize(orientation);
      var lineSize = new OrientedSize(orientation);
      var orientedConstraint = new OrientedSize(orientation, availableSize);

      var itemWidth = ItemWidth.IsNaN() ? (double?) null : ItemWidth;
      var itemHeight = ItemHeight.IsNaN() ? (double?) null : ItemHeight;

      var itemConstraintSize = new Size(itemWidth ?? availableSize.Width, itemHeight ?? availableSize.Height);

      foreach (var element in Children.Cast<UIElement>())
      {
        element.Measure(itemConstraintSize);
        var elementSize = element.GetDesiredOrientedSize(orientation, itemWidth, itemHeight);

        if (orientedConstraint.Direct.IsLessThan(lineSize.Direct + elementSize.Direct, XamlConstants.LayoutComparisonPrecision))
        {
          result = result.WrapSize(lineSize);
          lineSize = elementSize;

          if (!orientedConstraint.Direct.IsLessThan(elementSize.Direct, XamlConstants.LayoutComparisonPrecision)) continue;

          result = result.WrapSize(elementSize);
          lineSize = new OrientedSize(orientation);
        }
        else
          lineSize = lineSize.StackSize(elementSize);
      }

      return result.WrapSize(lineSize).Size;
    }

    private static object OnCoerceItemSize(object baseValue)
    {
      var newItemSize = (double) baseValue;
      if (!newItemSize.IsNaN() && (newItemSize <= 0.0 || newItemSize.IsInfinity()))
        throw new ArgumentOutOfRangeException("Item Width or Height property set to wrong value");

      return newItemSize;
    }

    private static object OnCoerceOrientation(object baseValue)
    {
      var newOrientation = (Orientation) baseValue;

      newOrientation.ValidateValue("Orientation property set to wrong value");

      return newOrientation;
    }

    #endregion
  }
}