// <copyright file="ArtboardSnapGuidePanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSnapGuidePanel : ArtboardPanel
	{
		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			foreach (ArtboardSnapGuide snapGuide in Children)
			{
				var rect = new Rect(new Point(0, 0), snapGuide.DesiredSize);

				if (snapGuide.Orientation == Orientation.Horizontal)
				{
					rect.Width = finalSize.Width;
					rect.Y = FromMatrix.Transform(new Point(0, snapGuide.Location)).Y;
				}
				else
				{
					rect.Height = finalSize.Height;
					rect.X = FromMatrix.Transform(new Point(snapGuide.Location, 0)).X;
				}

				snapGuide.Arrange(rect);
			}

			return finalSize;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			foreach (ArtboardSnapGuide snapGuide in Children)
				snapGuide.Measure(XamlConstants.InfiniteSize);

			return new Size(0, 0);
		}
	}
}