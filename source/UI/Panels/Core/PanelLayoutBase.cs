// <copyright file="PanelLayoutBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Panels.Core
{
	internal interface ILayoutInformation
	{
		Rect ArrangeRect { get; set; }
	}

	internal abstract class PanelLayoutBase<TPanel> where TPanel : IPanel
	{
		protected PanelLayoutBase(TPanel panel)
		{
			Panel = panel;
		}

		protected TPanel Panel { get; }

		public Size Arrange(Size finalSize)
		{
			return ArrangeCore(finalSize);
		}

		protected void ArrangeChild(UIElement element, Rect rect)
		{
			if (element is ILayoutInformation arrangeListener)
				arrangeListener.ArrangeRect = rect;

			element.Arrange(rect);
		}

		protected abstract Size ArrangeCore(Size finalSize);

		public Size Measure(Size availableSize)
		{
			return MeasureCore(availableSize);
		}

		protected abstract Size MeasureCore(Size availableSize);

		public virtual void OnLayoutUpdated()
		{
		}
	}
}