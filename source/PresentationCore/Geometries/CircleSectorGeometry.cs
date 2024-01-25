// <copyright file="CircleSectorGeometry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Geometries
{
	internal readonly struct CircleSectorGeometry
	{
		private readonly ArcSegment _arcSegment;
		private readonly LineSegment _endArcLineSegment;
		private readonly PathFigure _pathFigure;
		private readonly LineSegment _startArcLineSegment;

		public CircleSectorGeometry()
		{
			_pathFigure = new PathFigure
			{
				IsClosed = true
			};

			_startArcLineSegment = new LineSegment(new Point(), true);
			_pathFigure.Segments.Add(_startArcLineSegment);

			_arcSegment = new ArcSegment
			{
				SweepDirection = SweepDirection.Counterclockwise
			};
			_pathFigure.Segments.Add(_arcSegment);

			_endArcLineSegment = new LineSegment(new Point(), true);
			_pathFigure.Segments.Add(_endArcLineSegment);

			var pathGeometry = new PathGeometry();

			pathGeometry.Figures.Add(_pathFigure);

			Geometry = pathGeometry;
		}

		public Geometry Geometry { get; }

		public void Update(double radius, double startAngle, double endAngle)
		{
			var centerAngle = GeometryUtils.CalcCenterAngle(ref startAngle, ref endAngle);
			var size = new Size(2 * radius, 2 * radius);
			var isLargeArc = Math.Abs(centerAngle) > 180.0;
			var arcStartPoint = GeometryUtils.CalcDisplayPoint(GeometryUtils.CalcCartesianPoint(startAngle, radius), size);
			var arcEndPoint = GeometryUtils.CalcDisplayPoint(GeometryUtils.CalcCartesianPoint(startAngle + centerAngle, radius), size);
			var centerPoint = GeometryUtils.CalcDisplayPoint(new Point(0, 0), size);

			_pathFigure.StartPoint = centerPoint;
			_startArcLineSegment.Point = arcStartPoint;
			_arcSegment.Point = arcEndPoint;
			_arcSegment.IsLargeArc = isLargeArc;
			_arcSegment.Size = new Size(radius, radius);
			_endArcLineSegment.Point = centerPoint;
		}
	}
}