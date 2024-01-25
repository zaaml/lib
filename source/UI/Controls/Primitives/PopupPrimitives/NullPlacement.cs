// <copyright file="NullPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal class NullPlacement : PopupPlacement
	{
		public NullPlacement(Popup popup)
		{
			AttachPopup(popup);
		}

		protected override Rect ArrangeOverride(Size desiredSize)
		{
			return RectUtils.CalcAlignBox(ScreenBoundsCore, desiredSize.Rect(), HorizontalAlignment.Left, VerticalAlignment.Top);
		}
	}
}