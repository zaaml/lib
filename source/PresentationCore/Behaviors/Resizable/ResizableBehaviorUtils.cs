// <copyright file="ResizableBehaviorUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal static class ResizableBehaviorUtils
	{
		private static ResizableHandleKind CalcResizeHandleKind(Point position, double height, double width, Thickness gbThickness, int dc)
		{
			var resPart = ResizableHandleKind.Undefined;

			resPart |= position.Y >= 0 && position.Y <= dc * gbThickness.Top ? ResizableHandleKind.Top : 0;
			resPart |= position.Y >= height - dc * gbThickness.Bottom && position.Y <= height ? ResizableHandleKind.Bottom : 0;
			resPart |= position.X >= 0 && position.X <= dc * gbThickness.Left ? ResizableHandleKind.Left : 0;
			resPart |= position.X >= width - dc * gbThickness.Right && position.X <= width ? ResizableHandleKind.Right : 0;

			return resPart;
		}

		public static ResizableHandleKind GetResizableHandleKind(FrameworkElement border, Point position, Thickness borderThickness)
		{
			var gbThickness = borderThickness;
			var width = border.ActualWidth;
			var height = border.ActualHeight;

			const int dc = 2;

			var resPart = CalcResizeHandleKind(position, height, width, gbThickness, 1);

			if (resPart != ResizableHandleKind.Undefined)
				resPart |= CalcResizeHandleKind(position, height, width, gbThickness, dc);

			if ((resPart & ResizableHandleKind.Top) != 0 && (resPart & ResizableHandleKind.Bottom) != 0)
			{
				if (position.Y < height / 2)
					resPart &= ~ResizableHandleKind.Bottom;
				else
					resPart &= ~ResizableHandleKind.Top;
			}

			if ((resPart & ResizableHandleKind.Left) != 0 && (resPart & ResizableHandleKind.Right) != 0)
			{
				if (position.X < width / 2)
					resPart &= ~ResizableHandleKind.Right;
				else
					resPart &= ~ResizableHandleKind.Left;
			}

			return resPart;
		}

		public static bool HasBottom(ResizableHandleKind handleKind)
		{
			return (handleKind & ResizableHandleKind.Bottom) != 0;
		}

		public static bool HasLeft(ResizableHandleKind handleKind)
		{
			return (handleKind & ResizableHandleKind.Left) != 0;
		}

		public static bool HasRight(ResizableHandleKind handleKind)
		{
			return (handleKind & ResizableHandleKind.Right) != 0;
		}

		public static bool HasTop(ResizableHandleKind handleKind)
		{
			return (handleKind & ResizableHandleKind.Top) != 0;
		}
	}
}