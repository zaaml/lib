// <copyright file="PopupPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Snapping;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public abstract class PopupPlacement : InheritanceContextObject
  {
    #region Fields

    private bool _isPopupOpen;

    private Popup _popup;

    #endregion

    #region Ctors

    internal PopupPlacement()
    {
    }

    #endregion

    #region Properties

    internal virtual PopupPlacement ActualPlacement => this;

    protected bool IsPopupOpen
    {
      get => _isPopupOpen;
      private set
      {
        if (_isPopupOpen == value)
          return;

        _isPopupOpen = value;

        if (_isPopupOpen)
          OnPopupOpenedInt();
        else
          OnPopupClosedInt();
      }
    }

    internal virtual Popup Popup
    {
      get => _popup;
      set
      {
        if (_popup != null && value != null)
          throw new InvalidOperationException("PopupPlacement already attached to popup.");

        if (_popup != null)
          OnPopupDetaching();

        _popup = value;

        if (_popup != null)
          OnPopupAttached();

        IsPopupOpen = _popup?.IsOpen ?? false;
      }
    }

    internal virtual Rect ScreenBoundsCore => ScreenBoundsOverride;

    protected virtual Rect ScreenBoundsOverride => Screen.FromElement(Popup.Child).Bounds;

    internal virtual bool ShouldConstraint => true;

    #endregion

    #region  Methods

    internal Rect Arrange(Size desiredSize)
    {
      return Constraint(ArrangeOverride(desiredSize));
    }

		// TODO Implement preview arrange logic
    //internal Rect PreviewArrange(Size desiredSize)
    //{
    //}
		
		internal static SnapOptions ConvertOptions(PopupPlacementOptions options)
    {
	    var snapOptions = SnapOptions.None;

	    if ((options & PopupPlacementOptions.Fit) != 0)
		    snapOptions |= SnapOptions.Fit;

	    if ((options & PopupPlacementOptions.Move) != 0)
		    snapOptions |= SnapOptions.Move;

	    if ((options & PopupPlacementOptions.Fit) != 0)
		    snapOptions |= SnapOptions.Fit;

	    return snapOptions;
    }

    private Rect Constraint(Rect arrangeOverride)
    {
      return ShouldConstraint ? Snapper.Constraint(ScreenBoundsCore, arrangeOverride, ConvertOptions(Popup?.PlacementOptions ?? PopupPlacementOptions.None)) : arrangeOverride;
    }

    protected abstract Rect ArrangeOverride(Size desiredSize);

    //protected abstract Rect PreviewArrangeOverride(Size desiredSize);

    protected void Invalidate()
    {
      _popup?.InvalidatePlacement();
    }

    protected virtual void OnPopupAttached()
    {
    }

    protected virtual void OnPopupClosed()
    {
    }

    internal virtual void OnPopupClosedInt()
    {
      IsPopupOpen = false;

      OnPopupClosed();
    }

    protected virtual void OnPopupDetaching()
    {
    }

    protected virtual void OnPopupOpened()
    {
    }

    internal virtual void OnPopupOpenedInt()
    {
      IsPopupOpen = true;

      OnPopupOpened();
    }

    #endregion
  }
}