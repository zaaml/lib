// <copyright file="PopupPlacementWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal class PopupPlacementWrapper : PopupPlacement
	{
		private readonly PopupPlacement _placement;

		public PopupPlacementWrapper(PopupPlacement placement)
		{
			_placement = placement;
		}

		internal override PopupPlacement ActualPlacement => _placement;

		internal override Popup Popup
		{
			get => _placement.Popup;
			set => _placement.Popup = value;
		}

		protected override Rect ScreenBoundsOverride => _placement.ScreenBoundsCore;

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
	}
}