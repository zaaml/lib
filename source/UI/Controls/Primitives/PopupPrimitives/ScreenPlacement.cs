// <copyright file="ScreenPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Snapping;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class ScreenPlacement : PopupPlacement
  {
    #region Static Fields and Constants

    public static readonly PopupPlacement TopLeft = new ScreenPlacement(Screen.PrimaryScreen.Bounds.GetTopLeft());
    public static readonly PopupPlacement TopRight = new ScreenPlacement(Screen.PrimaryScreen.Bounds.GetTopRight());
    public static readonly PopupPlacement BottomLeft = new ScreenPlacement(Screen.PrimaryScreen.Bounds.GetBottomLeft());
    public static readonly PopupPlacement BottomRight = new ScreenPlacement(Screen.PrimaryScreen.Bounds.GetBottomRight());

    #endregion

    #region Ctors

    private ScreenPlacement(Point point)
    {
      Point = point;
    }

    #endregion

    #region Properties

    public Point Point { get; }

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