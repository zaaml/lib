// <copyright file="RelativePointPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class RelativePointPlacement : RelativePlacementBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty PointProperty = DPM.Register<Point, RelativePointPlacement>
      ("Point");

    #endregion

    #region Properties

    public Point Point
    {
      get => (Point) GetValue(PointProperty);
      set => SetValue(PointProperty, value);
    }

    #endregion

    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      return new Rect(Point, desiredSize).Offset(TargetScreenBox.GetTopLeft());
    }

    #endregion
  }
}