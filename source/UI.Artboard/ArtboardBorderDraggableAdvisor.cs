// <copyright file="ArtboardBorderDraggableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardBorderDraggableAdvisor : DraggableAdvisorBase
	{
		private ArtboardBorderDraggableAdvisor()
		{
		}

		public static ArtboardBorderDraggableAdvisor Instance { get; } = new ArtboardBorderDraggableAdvisor();

		public override Point GetPosition(UIElement element)
		{
			var border = (Border) element.GetVisualParent();
			var borderThickness = border.BorderThickness;

			if (element is FrameworkElement fre)
				return new Point(fre.Margin.Left + borderThickness.Left, fre.Margin.Top + borderThickness.Top);

			return new Point();
		}

		public override void SetPosition(UIElement element, Point value)
		{
			var border = (Border) element.GetVisualParent();
			var borderThickness = border.BorderThickness;

			if (element is FrameworkElement fre)
				fre.Margin = new Thickness(value.X - borderThickness.Left, value.Y - borderThickness.Top, 0, 0);
		}
	}
}