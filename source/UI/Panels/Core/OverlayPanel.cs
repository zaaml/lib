// <copyright file="AdornerPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Panels.Core
{
	public sealed class OverlayPanel : Panel
	{
		#region Properties

		internal OverlayContentControl Control { get; set; }

		#endregion

		#region  Methods

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var finalRect = new Rect(finalSize);

			for (var i = 0; i < Children.Count; i++)
			{
				var child = Children[i];

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

				child.Measure(availableSize);

				size.Width = Math.Max(size.Width, child.DesiredSize.Width);
				size.Height = Math.Max(size.Height, child.DesiredSize.Height);
			}

			return size;
		}

		#endregion
	}
}