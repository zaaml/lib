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
		private Point _mousePoint;

		protected override Rect ScreenBoundsOverride => Screen.FromPoint(_mousePoint).Bounds;

		internal override bool ShouldConstraint => false;

		protected override Rect ArrangeOverride(Size desiredSize)
		{
			return Snapper.Snap(ScreenBoundsCore, new Rect(_mousePoint, XamlConstants.ZeroSize), desiredSize.Rect(), SnapOptions.Fit | SnapOptions.Move, SnapDefinition.Default, SnapAdjustment.ZeroAdjustment, SnapAdjustment.ZeroAdjustment,
				SnapSide.Bottom);
		}

		internal override void OnPopupOpenedInternal()
		{
			_mousePoint = MouseInternal.ScreenLogicalPosition;

			base.OnPopupOpenedInternal();
		}
	}
}