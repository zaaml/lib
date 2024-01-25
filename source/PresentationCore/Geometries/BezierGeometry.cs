// <copyright file="BezierGeometry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Geometries
{
	internal readonly struct BezierGeometry
	{
		private readonly BezierSegment _bezierSegment;
		private readonly PathFigure _pathFigure;

		public BezierGeometry()
		{
			_bezierSegment = new BezierSegment();
			_pathFigure = new PathFigure(new Point(), new PathSegment[] { _bezierSegment }, false);
			Geometry = new PathGeometry(new[] { _pathFigure });
		}

		public Geometry Geometry { get; }

		public void Update(Point beginPoint, Point endPoint)
		{
			GeometryUtils.GetBezierPoints(beginPoint, endPoint, out var p1, out var p2, out var p3, out var p4);

			_pathFigure.StartPoint = p1;
			_bezierSegment.Point1 = p2;
			_bezierSegment.Point2 = p3;
			_bezierSegment.Point3 = p4;
		}
	}
}