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
			adorner.Arrange(ScrollViewTransform.Transform.TransformBounds(adorner.AdornerRect));
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var transform = ScrollViewTransform.Transform;

			foreach (var adorner in Children.OfType<ArtboardAdorner>())
				adorner.Arrange(transform.TransformBounds(adorner.AdornerRect));

			return finalSize;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var transform = ScrollViewTransform.Transform;

			foreach (var adorner in Children.OfType<ArtboardAdorner>())
				adorner.Measure(transform.TransformBounds(adorner.AdornerRect).Size);

			return new Size(0, 0);
		}
	}
}