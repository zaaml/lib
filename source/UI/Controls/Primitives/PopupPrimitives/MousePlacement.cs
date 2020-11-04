// <copyright file="MousePlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Snapping;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal sealed class MousePlacement : AbsolutePlacementBase
  {
    #region Fields

    private Point _mousePoint;

    #endregion

    #region Properties

    protected override Rect ScreenBoundsOverride => Screen.FromPoint(_mousePoint).Bounds;

    internal override bool ShouldConstraint => false;

    #endregion

    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      return Snapper.Snap(ScreenBoundsCore, new Rect(_mousePoint, XamlConstants.ZeroSize), desiredSize.Rect(), SnapOptions.Fit | SnapOptions.Move, SnapDefinition.Default, SnapAdjustment.ZeroAdjustment, SnapAdjustment.ZeroAdjustment, SnapSide.Bottom);
    }

    internal override void OnPopupOpenedInt()
    {
      _mousePoint = MouseInternal.ScreenPosition;
      base.OnPopupOpenedInt();
    }

    #endregion
  }
}