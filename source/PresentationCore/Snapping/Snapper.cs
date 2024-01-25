// <copyright file="Snapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Snapping
{
	internal static class Snapper
	{
		private static readonly List<SnapPointInfo> PointInfos;
		private static readonly List<Point> SideInfos;

		static Snapper()
		{
			SideInfos = new List<Point>
			{
				//Left
				new Point(-1, 0),

				//Top
				new Point(0, -1),

				//Right
				new Point(1, 0),

				//Bottom
				new Point(0, 1)
			};


			PointInfos = new List<SnapPointInfo>
			{
				//LeftBottom 
				SnapPointInfo.Create(new Point(0, 1), new Point(0, -1)),

				//LeftCenter
				SnapPointInfo.Create(new Point(0, 0.5), new Point(0, 0)),

				//LeftTop
				SnapPointInfo.Create(new Point(0, 0), new Point(0, 1)),

				//TopLeft
				SnapPointInfo.Create(new Point(0, 0), new Point(1, 0)),

				//TopCenter
				SnapPointInfo.Create(new Point(0.5, 0), new Point(0, 0)),

				//TopRight
				SnapPointInfo.Create(new Point(1, 0), new Point(-1, 0)),

				//RightTop
				SnapPointInfo.Create(new Point(1, 0), new Point(0, 1)),

				//RightCenter
				SnapPointInfo.Create(new Point(1, 0.5), new Point(0, 0)),

				//RightBottom
				SnapPointInfo.Create(new Point(1, 1), new Point(0, -1)),

				//BottomRight
				SnapPointInfo.Create(new Point(1, 1), new Point(-1, 0)),

				//BottomCenter
				SnapPointInfo.Create(new Point(0.5, 1), new Point(0, 0)),

				//BottomLeft
				SnapPointInfo.Create(new Point(0, 1), new Point(1, 0))
			};
		}

		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		internal static Rect Constraint(Rect bounds, Rect finalRect, SnapOptions snapOptions)
		{
			var actualHostBounds = bounds;

			if (actualHostBounds.Width == 0 && actualHostBounds.Height == 0)
				return finalRect;
			
			if ((snapOptions  & SnapOptions.Move) == 0)
				return actualHostBounds.IntersectionWith(finalRect);

			if (finalRect.Width > actualHostBounds.Width)
				finalRect.X = 0;
			else
			{
				if (finalRect.Left < actualHostBounds.Left)
					finalRect.X = actualHostBounds.X;
				else if (finalRect.Right > actualHostBounds.Right)
					finalRect.X += actualHostBounds.Right - finalRect.Right;
			}

			if (finalRect.Height > actualHostBounds.Height)
				finalRect.Y = 0;
			else
			{
				if (finalRect.Top < actualHostBounds.Top)
					finalRect.Y = actualHostBounds.Top;
				else if (finalRect.Bottom > actualHostBounds.Bottom)
					finalRect.Y += actualHostBounds.Bottom - finalRect.Bottom;
			}

			return actualHostBounds.IntersectionWith(finalRect);
		}

		internal static SnapSide ConvertDock(Dock dock)
		{
			return dock switch
			{
				Dock.Left => SnapSide.Left,
				Dock.Top => SnapSide.Top,
				Dock.Right => SnapSide.Right,
				Dock.Bottom => SnapSide.Bottom,
				_ => throw new ArgumentOutOfRangeException(nameof(dock))
			};
		}

		internal static Dock ConvertSnapSide(SnapSide side)
		{
			return side switch
			{
				SnapSide.Left => Dock.Left,
				SnapSide.Top => Dock.Top,
				SnapSide.Right => Dock.Right,
				SnapSide.Bottom => Dock.Bottom,
				_ => throw new ArgumentOutOfRangeException(nameof(side))
			};
		}

		public static Rect Snap(Rect bounds, Rect target, Rect source, SnapOptions options, SnapDefinition definition, SnapAdjustment targetAdjustment, SnapAdjustment sourceAdjustment, SnapSide snapSide)
		{
			var actualSnapSide = snapSide;

			return Snap(bounds, target, source, options, definition, targetAdjustment, sourceAdjustment, ref actualSnapSide, out var actualSnapPoint);
		}

		public static Rect Snap(Rect bounds, Rect target, Rect source, SnapOptions options, SnapDefinition definition, SnapAdjustment targetAdjustment, SnapAdjustment sourceAdjustment, ref SnapSide snapSide, out SnapPoint snapPoint)
		{
			if (definition == null)
				throw new ArgumentNullException(nameof(definition));

			snapPoint = SnapPoint.BottomCenter;

			var weight = double.MinValue;
			var result = Rect.Empty;

			const double containsWeight = 1e100;

			var actualSideOffset = definition.SideOffset + sourceAdjustment.SideOffset + targetAdjustment.SideOffset;
			var actualTargetCornerOffset = definition.TargetCornerOffset + targetAdjustment.CornerOffset;
			var actualSourceCornerOffset = definition.SourceCornerOffset + sourceAdjustment.CornerOffset;

			var limitSides = (options & SnapOptions.Fit) == 0 ? 1 : int.MaxValue;

			foreach (var sideRule in definition.EnumerateActualSideRules(snapSide).Take(limitSides))
			{
				foreach (var pointRule in sideRule.PointRules)
				{
					var rect = Snap(target, source, sideRule.Side, pointRule, actualSideOffset, actualTargetCornerOffset, actualSourceCornerOffset);
					var currentWeight = bounds.Contains(rect) ? containsWeight : 0.0;

					currentWeight += bounds.IntersectionWith(rect).Round().Square();

					if (currentWeight > weight == false) 
						continue;

					weight = currentWeight;
					snapSide = sideRule.Side;
					snapPoint = pointRule.Source;
					result = rect;
				}
			}

			return Constraint(bounds, result, options);
		}

		private static Rect Snap(Rect target, Rect source, SnapSide side, SnapPointRule pointRule, double sideOffset, double targetPointOffset, double sourcePointOffset)
		{
			var targetPointPosition = PointInfos[(int) pointRule.Target].GetPointPosition(target, targetPointOffset);
			var sourcePointPosition = PointInfos[(int) pointRule.Source].GetPointPosition(source, sourcePointOffset);

			var sideOffsetModifier = SideInfos[(int) side];
			var sideOffsetPoint = new Point(sideOffsetModifier.X * sideOffset, sideOffsetModifier.Y * sideOffset);

			return source.WithOffset(targetPointPosition).WithOffset(sourcePointPosition.Negate()).WithOffset(sideOffsetPoint);
		}

		private readonly struct SnapPointInfo
		{
			private SnapPointInfo(Point relativePoint, Point relativeOffset)
			{
				RelativePoint = relativePoint;
				RelativeOffset = relativeOffset;
			}

			private Point RelativePoint { get; }
			private Point RelativeOffset { get; }

			public static SnapPointInfo Create(Point relativePoint, Point relativeOffset)
			{
				return new SnapPointInfo(relativePoint, relativeOffset);
			}

			public Point GetPointPosition(Rect rect, double offset)
			{
				var clientOffset = new Point(RelativeOffset.X * offset, RelativeOffset.Y * offset);
				var clientPosition = new Point(rect.Width * RelativePoint.X, rect.Height * RelativePoint.Y).WithOffset(clientOffset);

				return clientPosition.WithOffset(rect.GetTopLeft());
			}
		}
	}
}