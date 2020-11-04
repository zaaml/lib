// <copyright file="PanelBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Panels.Core
{
	public class PanelBase : Panel
	{
		private static readonly DependencyProperty ArrangeRectProperty = DPM.RegisterAttached<Rect, PanelBase>
			("ArrangeRect");

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			foreach (UIElement child in Children)
				child.Arrange(GetArrangeRect(child));

			return finalSize;
		}

		protected static Rect GetArrangeRect(DependencyObject element)
		{
			return (Rect) element.GetValue(ArrangeRectProperty);
		}

		protected static Size GetArrangeSize(DependencyObject element)
		{
			return GetArrangeRect(element).Size();
		}

		protected static void SetArrangeRect(DependencyObject element, Rect value)
		{
			element.SetValue(ArrangeRectProperty, value);
		}
	}
}