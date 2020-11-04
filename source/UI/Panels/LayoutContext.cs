// <copyright file="LayoutContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Panels
{
	internal abstract class LayoutContext
	{
		public abstract ArrangeContextPass ArrangeContextPass { get; }

		public abstract Size MaxAvailableSize { get; }

		public abstract MeasureContextPass MeasureContextPass { get; }

		public static LayoutContext GetContext(UIElement element)
		{
			return element.GetVisualAncestors().OfType<ILayoutContextPanel>().FirstOrDefault()?.Context;
		}

		public virtual void OnDescendantMeasureDirty(UIElement element)
		{
		}
	}

	internal enum MeasureContextPass
	{
		MeasureDirty,
		MeasureToContent,
		PreviewMeasureDirty,
		PreviewMeasure,
		FinalMeasureDirty,
		FinalMeasure,
		MeasureClean
	}

	internal enum ArrangeContextPass
	{
		ArrangeDirty,
		PreviewArrange,
		FinalArrange,
		ArrangeClean
	}

	internal interface ILayoutContextPanel
	{
		LayoutContext Context { get; }
	}
}