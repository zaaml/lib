// <copyright file="WrapPanelLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Panels
{
	internal abstract class WrapPanelLayout : PanelLayoutBase<IWrapPanel>
	{
		#region Ctors

		protected WrapPanelLayout(IWrapPanel panel) : base(panel)
		{
		}

		#endregion

		#region  Methods

		public static Size Arrange(IWrapPanel panel, Size finalSize)
		{
			var orientation = panel.Orientation;
			var lineSize = new OrientedSize(orientation);
			var maximumSize = new OrientedSize(orientation, finalSize);

			var itemWidth = panel.ItemWidth.IsNaN() ? (double?)null : panel.ItemWidth;
			var itemHeight = panel.ItemHeight.IsNaN() ? (double?)null : panel.ItemHeight;

			var lineOffset = 0.0;
			var fixedItemSize = orientation.IsHorizontal() ? itemWidth : itemHeight;

			var children = panel.Elements;
			var count = children.Count;
			var lineStart = 0;

			for (var lineEnd = 0; lineEnd < count; lineEnd++)
			{
				var element = children[lineEnd];
				var elementSize = element.GetDesiredOrientedSize(orientation, itemWidth, itemHeight);

				if (maximumSize.Direct.IsLessThan(lineSize.Direct + elementSize.Direct, XamlConstants.LayoutComparisonPrecision))
				{
					panel.ArrangeStackLine(orientation, Zaaml.Core.Range.Create(lineStart, lineEnd), lineOffset, 0, lineSize.Indirect, fixedItemSize);

					lineOffset += lineSize.Indirect;
					lineSize = elementSize;

					if (maximumSize.Direct.IsLessThan(elementSize.Direct, XamlConstants.LayoutComparisonPrecision))
					{
						panel.ArrangeStackLine(orientation, Zaaml.Core.Range.Create(lineStart, ++lineEnd), lineOffset, 0, lineSize.Indirect, fixedItemSize);

						lineOffset += lineSize.Indirect;
						lineSize = new OrientedSize(orientation);
					}

					lineStart = lineEnd;
				}
				else
				{
					lineSize.Direct += elementSize.Direct;
					lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
				}
			}

			if (lineStart < count)
				panel.ArrangeStackLine(orientation, Zaaml.Core.Range.Create(lineStart, count), lineOffset, 0, lineSize.Indirect, fixedItemSize);

			return finalSize;
		}

		public static Size Measure(IWrapPanel panel, Size availableSize)
		{
			var orientation = panel.Orientation;
			var result = new OrientedSize(orientation);
			var lineSize = new OrientedSize(orientation);
			var orientedConstraint = new OrientedSize(orientation, availableSize);

			var itemWidth = panel.ItemWidth.IsNaN() ? (double?)null : panel.ItemWidth;
			var itemHeight = panel.ItemHeight.IsNaN() ? (double?)null : panel.ItemHeight;

			var itemConstraintSize = new Size(itemWidth ?? availableSize.Width, itemHeight ?? availableSize.Height);

			foreach (var element in panel.Elements)
			{
				element.Measure(itemConstraintSize);
				var elementSize = element.GetDesiredOrientedSize(orientation, itemWidth, itemHeight);

				if (orientedConstraint.Direct.IsLessThan(lineSize.Direct + elementSize.Direct, XamlConstants.LayoutComparisonPrecision))
				{
					result = result.WrapSize(lineSize);
					lineSize = elementSize;

					if (!orientedConstraint.Direct.IsLessThan(elementSize.Direct, XamlConstants.LayoutComparisonPrecision)) continue;

					result = result.WrapSize(elementSize);
					lineSize = new OrientedSize(orientation);
				}
				else
					lineSize = lineSize.StackSize(elementSize);
			}

			return result.WrapSize(lineSize).Size;
		}

		#endregion
	}
}