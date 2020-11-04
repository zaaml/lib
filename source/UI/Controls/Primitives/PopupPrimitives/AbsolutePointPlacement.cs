// <copyright file="AbsolutePointPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Snapping;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class AbsolutePointPlacement : AbsolutePlacementBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty PointProperty = DPM.Register<Point, AbsolutePointPlacement>
      ("Point");

    #endregion

    #region Properties

    public Point Point
    {
      get => (Point) GetValue(PointProperty);
      set => SetValue(PointProperty, value);
    }

    protected override Rect ScreenBoundsOverride => Screen.FromPoint(Point).Bounds;

    internal override bool ShouldConstraint => false;

    #endregion

    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      return Snapper.Snap(ScreenBoundsCore, new Rect(Point, XamlConstants.ZeroSize), desiredSize.Rect(), ConvertOptions(Popup.PlacementOptions), SnapDefinition.Default, SnapAdjustment.ZeroAdjustment, SnapAdjustment.ZeroAdjustment, SnapSide.Bottom);
    }

    #endregion
  }
}