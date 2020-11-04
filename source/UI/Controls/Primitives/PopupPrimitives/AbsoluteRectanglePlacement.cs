// <copyright file="AbsoluteRectanglePlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class AbsoluteRectanglePlacement : AbsolutePlacementBase
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty RectangleProperty = DPM.Register<Rect, AbsoluteRectanglePlacement>
			("Rectangle");

		#endregion

		#region Properties

		public Rect Rectangle
		{
			get => (Rect) GetValue(RectangleProperty);
			set => SetValue(RectangleProperty, value);
		}

		protected override Rect ScreenBoundsOverride => Screen.FromPoint(Rectangle.GetTopLeft()).Bounds;

		#endregion

		#region  Methods

		protected override Rect ArrangeOverride(Size desiredSize)
		{
			return Rectangle;
		}

		#endregion
	}
}