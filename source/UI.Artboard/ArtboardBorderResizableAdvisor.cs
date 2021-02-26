// <copyright file="ArtboardBorderResizableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Behaviors.Resizable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal sealed class ArtboardBorderResizableAdvisor : ResizableAdvisorBase
	{
		private ArtboardBorderResizableAdvisor()
		{
		}

		public static ArtboardBorderResizableAdvisor Instance { get; } = new ArtboardBorderResizableAdvisor();

		public override Rect GetBoundingBox(UIElement element)
		{
			var border = (Border) element.GetVisualParent();
			var borderThickness = border.BorderThickness;

			if (element is FrameworkElement fre)
				return new Rect(new Point(fre.Margin.Left + borderThickness.Left, fre.Margin.Top + borderThickness.Top), new Size(fre.Width, fre.Height));

			return Rect.Empty;
		}

		public override void SetBoundingBox(UIElement element, Rect rect)
		{
			var border = (Border) element.GetVisualParent();
			var borderThickness = border.BorderThickness;

			if (element is FrameworkElement fre)
			{
				fre.Margin = new Thickness(rect.X - borderThickness.Left, rect.Y - borderThickness.Top, 0, 0);
				fre.Width = rect.Width;
				fre.Height = rect.Height;
			}
		}
	}
}