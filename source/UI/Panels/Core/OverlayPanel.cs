// <copyright file="OverlayPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Panels.Core
{
	public sealed class OverlayPanel : Panel
	{
		internal OverlayContentControl Control { get; set; }

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var finalRect = new Rect(finalSize);

			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is OverlayItemPresenter overlayItemPresenter)
					//child.Arrange(new Rect(new Point(overlayItemPresenter.X, overlayItemPresenter.Y), child.DesiredSize));
					child.Arrange(finalRect);
				else
					child.Arrange(finalRect);
			}

			return finalSize;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var size = new Size();

			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

				if (child is OverlayItemPresenter)
				{
					child.Measure(XamlConstants.InfiniteSize);

					continue;
				}

				child.Measure(availableSize);

				size.Width = Math.Max(size.Width, child.DesiredSize.Width);
				size.Height = Math.Max(size.Height, child.DesiredSize.Height);
			}

			return size;
		}
	}
}