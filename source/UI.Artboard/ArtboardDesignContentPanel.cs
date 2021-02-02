// <copyright file="ArtboardDesignContentPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardDesignContentPanel : ArtboardPanel
	{
		//protected override Size ArrangeOverrideCore(Size finalSize)
		//{
		//	var designRect = CalcDesignRect();

		//	foreach (UIElement adorner in Children)
		//		adorner.Arrange(designRect);

		//	return finalSize;
		//}

		//private Rect CalcDesignRect()
		//{
		//	return ScrollViewTransform.Transform.TransformBounds(new Rect(new Size(DesignWidth, DesignHeight)));
		//}

		//protected override Size MeasureOverrideCore(Size availableSize)
		//{
		//	var designRect = CalcDesignRect();

		//	foreach (var adorner in Children.OfType<ArtboardAdorner>())
		//		adorner.Measure(designRect.Size);

		//	return new Size(0, 0);
		//}

		//protected override void OnDesignHeightChanged()
		//{
		//	base.OnDesignHeightChanged();

		//	InvalidateMeasure();
		//}

		//protected override void OnDesignWidthChanged()
		//{
		//	base.OnDesignWidthChanged();

		//	InvalidateMeasure();
		//}
	}
}