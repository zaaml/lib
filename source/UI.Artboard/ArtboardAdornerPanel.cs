// <copyright file="ArtboardAdornerPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardAdornerPanel : ArtboardPanel
	{
		internal ArtboardAdornerPresenter Presenter { get; set; }

		internal void ArrangeAdorner(ArtboardAdorner adorner)
		{
			var adornerRect = adorner.AdornerRect;

			if (adornerRect.IsEmpty)
				adornerRect = new Rect(new Size(0,0));

			adorner.Arrange(ScrollViewTransform.Transform.TransformBounds(adornerRect));
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var transform = ScrollViewTransform.Transform;

			foreach (var adorner in Children.OfType<ArtboardAdorner>())
			{
				var adornerRect = adorner.AdornerRect;

				if (adornerRect.IsEmpty)
					adornerRect = new Rect(new Size(0, 0));

				adorner.Arrange(transform.TransformBounds(adornerRect));
			}

			return finalSize;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var transform = ScrollViewTransform.Transform;

			foreach (var adorner in Children.OfType<ArtboardAdorner>())
			{
				var adornerRect = adorner.AdornerRect;

				if (adornerRect.IsEmpty)
					adornerRect = new Rect(new Size(0, 0));

				adorner.Measure(transform.TransformBounds(adornerRect).Size);
			}

			return new Size(0, 0);
		}
	}
}