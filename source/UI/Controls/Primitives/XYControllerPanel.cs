// <copyright file="XYControllerPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives
{
	public class XYControllerPanel : Panel
	{
		private XYController _xyController;

		internal Size ArrangeSize { get; private set; }

		private double RangeX { get; set; }

		private double RangeY { get; set; }

		public XYController XYController
		{
			get => _xyController;
			set
			{
				if (ReferenceEquals(_xyController, value))
					return;

				if (_xyController != null)
					_xyController.ItemCollection.Panel = null;

				_xyController = value;

				if (_xyController != null)
					_xyController.ItemCollection.Panel = this;

				OnRangesChanged();
			}
		}

		internal void ArrangeItem(XYControllerItem xyControllerItem)
		{
			if (XYController == null)
				return;

			xyControllerItem.Arrange(CalcArrangeRect(xyControllerItem, ArrangeSize));
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			ArrangeSize = finalSize;

			if (XYController == null)
				return finalSize;

			foreach (UIElement uiElement in Children)
			{
				if (uiElement is not XYControllerItem xyControllerItem)
				{
					uiElement.Arrange(new Rect(uiElement.DesiredSize));

					continue;
				}

				xyControllerItem.Arrange(CalcArrangeRect(xyControllerItem, finalSize));
			}

			return finalSize;
		}

		private Rect CalcArrangeRect(XYControllerItem xyControllerItem, Size finalSize)
		{
			var varWidth = Math.Max(finalSize.Width - xyControllerItem.DesiredSize.Width, 0);
			var varHeight = Math.Max(finalSize.Height - xyControllerItem.DesiredSize.Height, 0);

			var offsetX = RangeX.IsGreaterThan(0.0) ? (varWidth / RangeX) * xyControllerItem.X : 0.0;
			var offsetY = RangeY.IsGreaterThan(0.0) ? (varHeight / RangeY) * xyControllerItem.Y : 0.0;

			return new Rect(new Point(offsetX, offsetY), xyControllerItem.DesiredSize);
		}

		internal double GetPixelRatioX(XYControllerItem item)
		{
			if (XYController == null)
				return 0.0;

			var rangeX = new Range<double>(XYController.MinimumX, XYController.MaximumX);
			var deltaX = rangeX.Maximum - rangeX.Minimum;
			var varWidth = Math.Max(ArrangeSize.Width - item.DesiredSize.Width, 0);

			return deltaX.IsGreaterThan(0.0) ? varWidth / deltaX : 0.0;
		}

		internal double GetPixelRatioY(XYControllerItem item)
		{
			if (XYController == null)
				return 0.0;

			var rangeY = new Range<double>(XYController.MinimumY, XYController.MaximumY);
			var deltaY = rangeY.Maximum - rangeY.Minimum;
			var varHeight = Math.Max(ArrangeSize.Height - item.DesiredSize.Height, 0);

			return deltaY.IsGreaterThan(0.0) ? varHeight / deltaY : 0.0;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			foreach (UIElement uiElement in Children)
				uiElement.Measure(XamlConstants.InfiniteSize);

			return new Size(0, 0);
		}

		internal void MoveItem(XYControllerItem xyControllerItem, Point position)
		{
			var desiredSize = xyControllerItem.DesiredSize;
			var itemRect = new Rect(new Point(position.X - desiredSize.HalfWidth(), position.Y - desiredSize.HalfHeight()), desiredSize);

			var finalSize = ArrangeSize;
			var varWidth = Math.Max(finalSize.Width - xyControllerItem.DesiredSize.Width, 0);
			var varHeight = Math.Max(finalSize.Height - xyControllerItem.DesiredSize.Height, 0);

			var offsetX = itemRect.Left;
			var offsetY = itemRect.Top;

			xyControllerItem.X = varWidth.IsGreaterThan(0) ? RangeX * offsetX / varWidth : xyControllerItem.X;
			xyControllerItem.Y = varHeight.IsGreaterThan(0) ? RangeY * offsetY / varHeight : xyControllerItem.Y;
		}

		internal void OnRangesChanged()
		{
			UpdateRanges();
			InvalidateArrange();
		}

		private void UpdateRanges()
		{
			if (XYController == null)
			{
				RangeX = 1.0;
				RangeY = 1.0;
			}
			else
			{
				RangeX = XYController.MaximumX - XYController.MinimumX;
				RangeY = XYController.MaximumY - XYController.MinimumY;
			}
		}
	}
}