// <copyright file="PopupPlacementWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal class PopupPlacementWrapper : PopupPlacement
  {
    #region Fields

    private readonly PopupPlacement _placement;

    #endregion

    #region Ctors

    public PopupPlacementWrapper(PopupPlacement placement)
    {
      _placement = placement;
    }

    #endregion

    #region Properties

    internal override Popup Popup
    {
      get => _placement.Popup;
      set => _placement.Popup = value;
    }

    protected override Rect ScreenBoundsOverride => _placement.ScreenBoundsCore;

    #endregion

    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      return _placement.Arrange(desiredSize);
    }

    internal override void OnPopupClosedInt()
    {
      _placement.OnPopupClosedInt();
    }

    internal override void OnPopupOpenedInt()
    {
      _placement.OnPopupOpenedInt();
    }

    #endregion
  }
}
