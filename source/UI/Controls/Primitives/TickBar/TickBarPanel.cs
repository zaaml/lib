// <copyright file="TickBarPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	public class TickBarPanel : ItemsPanel<TickBarItem>
	{
		private TickBarItemsPresenter _itemsPresenter;

		public TickBarItemsPresenter ItemsPresenter
		{
			get => _itemsPresenter;
			internal set
			{
				if (ReferenceEquals(_itemsPresenter, value))
					return;

				_itemsPresenter = value;

				InvalidateMeasure();
			}
		}

		private Orientation Orientation => TickBarControl?.Orientation ?? Orientation.Horizontal;

		internal TickBarControl TickBarControl => ItemsPresenter?.TickBarControl;

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var tickBarControl = TickBarControl;

			if (tickBarControl == null || Children.Count == 0)
				return finalSize;

			var orientation = Orientation;
			var orientedSize = new OrientedSize(orientation, finalSize);
			var delta = Children.Count > 1 ? orientedSize.Direct / (Children.Count - 1) : 0;
			var offset = new OrientedPoint(orientation);

			if (Children.Count == 1) offset.Direct += orientedSize.Direct / 2;

			foreach (TickBarItem item in Children)
			{
				var desiredOriented = item.DesiredSize.AsOriented(orientation).WithIndirect(orientedSize.Indirect);
				var alignOffset = new OrientedPoint(orientation).WithDirect(-desiredOriented.Direct / 2);
				var rect = new Rect(desiredOriented.Size).WithOffset(offset).WithOffset(alignOffset);

				item.Arrange(rect);

				offset.Direct += delta;
			}

			return finalSize;
		}

		private Rect CalcDivisionBounds(TickBarControl tickBarControl)
		{
			var divisionBounds = new Rect();

			if (tickBarControl.DivisionDrawing != null)
				divisionBounds = divisionBounds.WithBounds(tickBarControl.DivisionDrawing.Bounds);

			foreach (var subDivision in tickBarControl.SubDivisions)
				divisionBounds = divisionBounds.WithBounds(subDivision.Drawing.Bounds);

			return divisionBounds;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var tickBarControl = TickBarControl;

			if (tickBarControl == null)
				return XamlConstants.ZeroSize;

			var itemConstraint = XamlConstants.InfiniteSize;
			var orientation = Orientation;
			var size = new OrientedSize(orientation);

			foreach (TickBarItem item in Children)
			{
				item.Measure(itemConstraint);

				size = size.StackSize(item.DesiredSize);
			}

			var divisionSize = CalcDivisionBounds(tickBarControl).Size.AsOriented(orientation);
			var itemDock = tickBarControl.ItemDock;

			if (itemDock is not TickBarItemDock.Center)
				size.Indirect += divisionSize.Indirect;

			return size.WithDirect(0).Size;
		}

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);

			var tickBarControl = TickBarControl;

			if (tickBarControl == null || (tickBarControl.SubDivisions.Count == 0 && tickBarControl.DivisionDrawing == null))
				return;

			var subDivisions = tickBarControl.SubDivisions;
			var orientation = Orientation;
			var orientedSize = new OrientedSize(orientation, RenderSize);
			var desiredSizeOriented = DesiredSize.AsOriented(orientation);
			var delta = Children.Count > 1 ? orientedSize.Direct / (Children.Count - 1) : 0;
			var offset = new OrientedPoint(orientation);
			var count = Children.Count;
			var useLayoutRounding = UseLayoutRounding;
			var drawingBrush = tickBarControl.DivisionDrawingBrush;
			var drawingBounds = drawingBrush?.Drawing?.Bounds ?? Rect.Empty;
			var itemDock = tickBarControl.ItemDock;
			var divisionSize = CalcDivisionBounds(tickBarControl).Size.AsOriented(orientation);

			if (itemDock is TickBarItemDock.Start)
				offset.Indirect += desiredSizeOriented.Indirect - divisionSize.Indirect;
			else if (itemDock is TickBarItemDock.End)
				offset.Indirect -= divisionSize.Indirect;

			for (var i = 0; i < count; i++)
			{
				if (drawingBrush?.Drawing != null)
				{
					var ptOffset = offset.Point;
					var itemDrawingRect = drawingBounds.WithOffset(ptOffset);

					if (useLayoutRounding)
						itemDrawingRect = itemDrawingRect.LayoutRound(RoundingMode.MidPointToEven);

					dc.DrawRectangle(drawingBrush, null, itemDrawingRect);
				}

				if (i == count - 1)
					break;

				RenderSubDivisions(subDivisions, 0, orientation, delta, offset, useLayoutRounding, dc);

				offset.Direct += delta;
			}
		}

		private static void RenderSubDivisions(TickBarSubDivisionCollection subDivisionCollection, int level, Orientation orientation, double delta, OrientedPoint offset, bool useLayoutRounding, DrawingContext dc)
		{
			var subDivision = subDivisionCollection[level];
			var subDivisionCount = subDivision.Count + 1;
			var subDivisionDrawing = subDivision.Drawing;

			if (subDivisionCount < 1 || subDivisionDrawing == null || delta < subDivision.ThresholdLength)
				return;

			var subDelta = delta / subDivisionCount;
			var subOffset = new OrientedPoint(orientation);
			var drawingBounds = subDivisionDrawing.Bounds;
			var drawingBrush = subDivision.Brush;

			for (var i = 0; i < subDivisionCount; i++)
			{
				if (level < subDivisionCollection.Count - 1)
					RenderSubDivisions(subDivisionCollection, level + 1, orientation, subDelta, offset.WithDirect(offset.Direct + subOffset.Direct), useLayoutRounding, dc);

				if (i == subDivisionCount - 1)
					break;

				subOffset.Direct += subDelta;

				var ptOffset = subOffset.Point;
				var subDivisionRect = drawingBounds.WithOffset(ptOffset).WithOffset(offset);

				if (useLayoutRounding)
					subDivisionRect = subDivisionRect.LayoutRound(RoundingMode.MidPointToEven);

				dc.DrawRectangle(drawingBrush, null, subDivisionRect);
			}
		}
	}
}