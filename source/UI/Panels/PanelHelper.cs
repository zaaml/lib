// <copyright file="PanelHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Panels
{
  internal static class PanelHelper
  {
    #region  Methods

    public static OrientedSize ArrangeStackLine(this Panel panel, Orientation orientation, Range<int> line, double lineOffset, double itemOffset, double? fixedLineSize, double? fixedItemSize)
    {
      return ArrangeStackLine(panel.Children.Cast<UIElement>().Skip(line.Minimum).Take(line.Length()), orientation, lineOffset, itemOffset, fixedLineSize, fixedItemSize);
    }

    public static OrientedSize ArrangeStackLine(IEnumerable<UIElement> elements, Orientation orientation, double lineOffset, double itemOffset, double? fixedLineSize, double? fixedItemSize)
    {
      var isHorizontal = orientation.IsHorizontal();

      var orientedSize = new OrientedSize(orientation);

      foreach (var element in elements)
      {
        var elementSize = element.DesiredSize.AsOriented(orientation);

        var itemSize = fixedItemSize ?? elementSize.Direct;
        var lineSize = fixedLineSize ?? elementSize.Indirect;

        var bounds = isHorizontal
          ? new Rect(itemOffset, lineOffset, itemSize, lineSize)
          : new Rect(lineOffset, itemOffset, lineSize, itemSize);

	      //bounds = bounds.LayoutRoundToEven();

        element.Arrange(bounds);
        itemOffset += itemSize;
	      //itemOffset = itemOffset.LayoutRoundMidPointFromZero(orientation);

        orientedSize = orientedSize.StackSize(elementSize);
      }

      return orientedSize;
    }

    internal static Size ConstraintSize(this Size desiredSize, Size availableSize)
    {
      return new Size(desiredSize.Width.Clamp(0, availableSize.Width), desiredSize.Height.Clamp(0, availableSize.Height));
    }

    internal static OrientedSize ConstraintSize(this OrientedSize desiredSize, OrientedSize availableSize)
    {
      if (desiredSize.Orientation != availableSize.Orientation)
        throw new InvalidOperationException();

      return new OrientedSize(desiredSize.Orientation, desiredSize.Size.ConstraintSize(availableSize.Size));
    }

	  internal static OrientedSize Clamp(this OrientedSize self, OrientedSize min, OrientedSize max)
	  {
			if (self.Orientation != min.Orientation || min.Orientation != max.Orientation)
				throw new InvalidOperationException();

		  return new OrientedSize(self.Orientation)
		  {
			  Direct = self.Direct.Clamp(min.Direct, max.Direct),
			  Indirect = self.Indirect.Clamp(min.Indirect, max.Indirect),
		  };
	  }

    internal static Rect FillRect(this FrameworkElement fre, Size availableSize)
    {
      var rect = fre.DesiredSize.Rect();

      if (fre.ShouldFill(Orientation.Horizontal))
        rect.Width = availableSize.Width;
      if (fre.ShouldFill(Orientation.Vertical))
        rect.Height = availableSize.Height;

      return rect;
    }

    internal static Rect FillRectIndirect(this FrameworkElement fre, Orientation orientation, Size availableSize)
    {
      if (!fre.ShouldFill(orientation)) return fre.DesiredSize.Rect();

      var orientedSize = fre.DesiredSize.AsOriented(orientation);
      orientedSize.Indirect = availableSize.AsOriented(orientation).Indirect;

      return orientedSize.Size.Rect();
    }

    public static OrientedSize GetDesiredOrientedSize(this UIElement uie, Orientation orientation)
    {
      return new OrientedSize(orientation, uie.DesiredSize.Width, uie.DesiredSize.Height);
    }

    public static OrientedSize GetDesiredOrientedSize(this UIElement uie, Orientation orientation, double? fixedWidth, double? fixedHeight)
    {
      return new OrientedSize(orientation, fixedWidth ?? uie.DesiredSize.Width, fixedHeight ?? uie.DesiredSize.Height);
    }

    private static Size Measure(UIElement element, Size size)
    {
      element.Measure(size);
      return element.DesiredSize;
    }

    public static Size MeasureSimple(IEnumerable<UIElement> elements, Size availableSize)
    {
      return elements.Aggregate(XamlConstants.ZeroSize, (current, element) => current.ExpandTo(Measure(element, availableSize)));
    }

    //public static OrientedSize MeasureStackLine(this Panel panel, Orientation orientation, Range<int> line, double lineOffset, double itemOffset, double? fixedLineSize, double? fixedItemSize)
    //{
    //  return MeasureStackLine(panel.Children.Cast<UIElement>().Skip(line.Minimum).Take(line.Length()), orientation, null, null, fixedLineSize, fixedItemSize);
    //}

    public static OrientedSize MeasureStackLine(IEnumerable<UIElement> elements, OrientedSize availableSize, OrientedSize minimumSize, double? fixedLineSize, double? fixedItemSize)
    {
	    var orientation = availableSize.Orientation;
      var orientedSize = new OrientedSize(orientation);

      foreach (var element in elements)
      {
	      var currentConstraint = new OrientedSize(orientation)
	      {
		      Direct = (availableSize.Direct - orientedSize.Direct).Clamp(minimumSize.Direct, double.PositiveInfinity),
		      Indirect = availableSize.Indirect.Clamp(minimumSize.Indirect, double.PositiveInfinity)
				};

	      var measureSize = new OrientedSize(orientation)
	      {
		      Direct = Math.Min(currentConstraint.Direct, fixedItemSize ?? double.PositiveInfinity),
		      Indirect = Math.Min(currentConstraint.Indirect, fixedLineSize ?? double.PositiveInfinity)
	      };

				element.Measure(measureSize.Size);

	      var desired = element.DesiredSize.AsOriented(orientation);

        orientedSize = orientedSize.StackSize(desired);
      }

      return orientedSize;
    }

    public static bool ShouldFill(this FrameworkElement fre, Orientation orientation)
    {
      return orientation.IsHorizontal() ? fre.Width.IsNaN() && fre.HorizontalAlignment == HorizontalAlignment.Stretch : fre.Height.IsNaN() && fre.VerticalAlignment == VerticalAlignment.Stretch;
    }

		public static bool ShouldFill(this IFrameworkElement fre, Orientation orientation)
		{
			return orientation.IsHorizontal() ? fre.Width.IsNaN() && fre.HorizontalAlignment == HorizontalAlignment.Stretch : fre.Height.IsNaN() && fre.VerticalAlignment == VerticalAlignment.Stretch;
		}

		public static bool ShouldFill(this UIElement uie, Orientation orientation)
    {
      var fre = uie as FrameworkElement;
      return fre != null && fre.ShouldFill(orientation);
    }

    public static OrientedSize StackSize(this OrientedSize self, OrientedSize itemSize)
    {
      self.Indirect = Math.Max(itemSize.Indirect, self.Indirect);
      self.Direct += itemSize.Direct;
      return self;
    }

    public static OrientedSize StackSize(this OrientedSize self, Size itemSize)
    {
      return self.StackSize(itemSize.AsOriented(self.Orientation));
    }

    public static OrientedSize WrapSize(this OrientedSize self, OrientedSize itemSize)
    {
      self.Direct = Math.Max(itemSize.Direct, self.Direct);
      self.Indirect += itemSize.Indirect;
      return self;
    }

    public static OrientedSize WrapSize(this OrientedSize self, Size itemSize)
    {
      return self.WrapSize(itemSize.AsOriented(self.Orientation));
    }

    #endregion
  }
}