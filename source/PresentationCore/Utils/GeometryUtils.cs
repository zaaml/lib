// <copyright file="GeometryUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Utils
{
	internal static class GeometryUtils
	{
		private static Point CalcCartesianPoint(double angle, double radius)
		{
			var angleRad = angle * Math.PI / 180.0;

			var x = radius * Math.Cos(angleRad);
			var y = radius * Math.Sin(angleRad);

			return new Point(x, y);
		}

		private static double CalcCenterAngle(ref double startAngle, ref double endAngle)
		{
			startAngle %= 360;
			endAngle %= 360;

			if (startAngle < 0)
				startAngle += 360;

			if (endAngle < 0)
				endAngle += 360;

			if (startAngle > endAngle)
			{
				var t = startAngle;

				startAngle = endAngle;
				endAngle = t;
			}

			return endAngle - startAngle;
		}

		private static Point CalcDisplayPoint(Point cartesianPoint, Size displaySize)
		{
			return new(cartesianPoint.X + displaySize.Width / 2, displaySize.Height / 2 - cartesianPoint.Y);
		}

		public static Geometry CreateAnnularSectorGeometry(double radius1, double radius2, double startAngle, double endAngle)
		{
			var outerRadius = Math.Max(radius1, radius2);
			var innerRadius = Math.Min(radius1, radius2);

			if (innerRadius.IsCloseTo(0))
				return CreateCircularSectorGeometry(outerRadius, startAngle, endAngle);

			var centerAngle = CalcCenterAngle(ref startAngle, ref endAngle);

			if (centerAngle.IsCloseTo(360.0) || centerAngle.IsCloseTo(0.0))
				return CreateAnnulusGeometry(radius1, radius2);

			var size = new Size(2 * outerRadius, 2 * outerRadius);
			var isLargeArc = Math.Abs(centerAngle) > 180.0;
			var outerArcStartPoint = CalcDisplayPoint(CalcCartesianPoint(startAngle, outerRadius), size);
			var outerArcEndPoint = CalcDisplayPoint(CalcCartesianPoint(startAngle + centerAngle, outerRadius), size);
			var innerArtStartPoint = CalcDisplayPoint(CalcCartesianPoint(startAngle + centerAngle, innerRadius), size);
			var innerArtEndPoint = CalcDisplayPoint(CalcCartesianPoint(startAngle, innerRadius), size);

			var pathFigure = new PathFigure
			{
				StartPoint = outerArcStartPoint,
				IsClosed = true
			};

			pathFigure.Segments.Add(new ArcSegment
			{
				Point = outerArcEndPoint,
				IsLargeArc = isLargeArc,
				Size = new Size(outerRadius, outerRadius),
				SweepDirection = SweepDirection.Counterclockwise
			});

			pathFigure.Segments.Add(new LineSegment(innerArtStartPoint, true));

			pathFigure.Segments.Add(new ArcSegment
			{
				Point = innerArtEndPoint,
				IsLargeArc = isLargeArc,
				Size = new Size(innerRadius, innerRadius),
				SweepDirection = SweepDirection.Clockwise
			});

			pathFigure.Segments.Add(new LineSegment(outerArcStartPoint, true));

			var pathGeometry = new PathGeometry();

			pathGeometry.Figures.Add(pathFigure);

			return pathGeometry;
		}

		public static Geometry CreateAnnulusGeometry(double radius1, double radius2)
		{
			var outerRadius = Math.Max(radius1, radius2);
			var innerRadius = Math.Min(radius1, radius2);
			var size = new Size(2 * outerRadius, 2 * outerRadius);

			var innerCircle = new EllipseGeometry(CalcDisplayPoint(new Point(0, 0), size), innerRadius, innerRadius);
			var outerCircle = new EllipseGeometry(CalcDisplayPoint(new Point(0, 0), size), outerRadius, outerRadius);

			return new CombinedGeometry(GeometryCombineMode.Exclude, outerCircle, innerCircle);
		}

		public static Geometry CreateCircleGeometry(double radius)
		{
			var size = new Size(2 * radius, 2 * radius);

			return new EllipseGeometry(CalcDisplayPoint(new Point(0, 0), size), radius, radius);
		}

		public static Geometry CreateCircularSectorGeometry(double radius, double startAngle, double endAngle)
		{
			var centerAngle = CalcCenterAngle(ref startAngle, ref endAngle);

			if (centerAngle.IsCloseTo(360.0) || centerAngle.IsCloseTo(0.0))
				return CreateCircleGeometry(radius);

			var size = new Size(2 * radius, 2 * radius);
			var isLargeArc = Math.Abs(centerAngle) > 180.0;
			var arcStartPoint = CalcDisplayPoint(CalcCartesianPoint(startAngle, radius), size);
			var arcEndPoint = CalcDisplayPoint(CalcCartesianPoint(startAngle + centerAngle, radius), size);
			var centerPoint = CalcDisplayPoint(new Point(0, 0), size);

			var pathFigure = new PathFigure
			{
				StartPoint = centerPoint,
				IsClosed = true
			};

			pathFigure.Segments.Add(new LineSegment(arcStartPoint, true));

			pathFigure.Segments.Add(new ArcSegment
			{
				Point = arcEndPoint,
				IsLargeArc = isLargeArc,
				Size = new Size(radius, radius),
				SweepDirection = SweepDirection.Counterclockwise
			});

			pathFigure.Segments.Add(new LineSegment(centerPoint, true));

			var pathGeometry = new PathGeometry();

			pathGeometry.Figures.Add(pathFigure);

			return pathGeometry;
		}
	}
}