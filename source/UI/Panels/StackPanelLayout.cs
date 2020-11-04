// <copyright file="StackPanelLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels
{
  internal abstract class StackPanelLayout : PanelLayoutBase<IStackPanel>
  {
    #region Ctors

    protected StackPanelLayout(IStackPanel panel) : base(panel)
    {
    }

    #endregion

    #region  Methods

    public static Size Arrange(IStackPanel panel, Size finalSize)
    {
	    var spacing = panel is IStackPanelAdvanced advanced ? advanced.Spacing : 0.0;
      var orientation = panel.Orientation;
      var offset = new OrientedPoint(orientation);
      var finalOriented = finalSize.AsOriented(orientation);

      foreach (var child in panel.Elements)
      {
        var size = child.DesiredSize.AsOriented(orientation);

        size.Indirect = finalOriented.Indirect;

        var rect = new Rect(offset.Point, size.Size);

        child.Arrange(rect);
        offset.Direct += size.Direct + spacing;
      }

      return finalSize;
    }

    public static Size Measure(IStackPanel panel, Size availableSize)
    {
	    var spacing = panel is IStackPanelAdvanced advanced ? advanced.Spacing : 0.0;
      var orientation = panel.Orientation;
      var childConstraint = availableSize.AsOriented(orientation).ChangeDirect(double.PositiveInfinity);
      var result = new OrientedSize(orientation);
			var elementsCount = panel.Elements.Count;
			var spacingSize = new OrientedSize().ChangeDirect(spacing);

      for (var index = 0; index < elementsCount; index++)
      {
	      var child = panel.Elements[index];

	      child.Measure(childConstraint.Size);

	      result = result.StackSize(child.DesiredSize);

				if (index + 1 < elementsCount)
					result = result.StackSize(spacingSize);
      }

      return result.Size;
    }

    #endregion
  }
}