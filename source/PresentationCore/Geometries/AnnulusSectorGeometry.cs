// <copyright file="AnnulusSectorGeometry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Geometries
{
	internal readonly struct AnnulusSectorGeometry
	{
		private readonly PathFigure _annulusFigure;
		private readonly ArcSegment _innerArc;
		private readonly LineSegment _innerArcStartPointSegment;
		private readonly ArcSegment _outerArc;
		private readonly LineSegment _outerArcStartSegment;

		public AnnulusSectorGeometry()
		{
			_annulusFigure = new PathFigure
			{
				IsClosed = true
			};

			_outerArc = new ArcSegment
			{
				SweepDirection = SweepDirection.Counterclockwise
			};

			_annulusFigure.Segments.Add(_outerArc);

			_innerArcStartPointSegment = new LineSegment(new Point(), true);

			_annulusFigure.Segments.Add(_innerArcStartPointSegment);

			_innerArc = new ArcSegment
			{
				SweepDirection = SweepDirection.Clockwise
			};

			_annulusFigure.Segments.Add(_innerArc);

			_outerArcStartSegment = new LineSegment(new Point(), true);
			_annulusFigure.Segments.Add(_outerArcStartSegment);

			var geometry = new PathGeometry();

			geometry.Figures.Add(_annulusFigure);

			Geometry = geometry;
		}

		public Geometry Geometry { get; }

		public void Update(double innerRadius, double outerRadius, double startAngle, double endAngle)
		{
			var radius1 = innerRadius;
			var radius2 = outerRadius;

			outerRadius = Math.Max(radius1, radius2);
			innerRadius = Math.Min(radius1, radius2);

			var centerAngle = GeometryUtils.CalcCenterAngle(ref startAngle, ref endAngle);
			var size = new Size(2 * outerRadius, 2 * outerRadius);
			var isLargeArc = Math.Abs(centerAngle) > 180.0;
			var outerArcStartPoint = GeometryUtils.CalcDisplayPoint(GeometryUtils.CalcCartesianPoint(startAngle, outerRadius), size);
			var outerArcEndPoint = GeometryUtils.CalcDisplayPoint(GeometryUtils.CalcCartesianPoint(startAngle + centerAngle, outerRadius), size);
			var innerArcStartPoint = GeometryUtils.CalcDisplayPoint(GeometryUtils.CalcCartesianPoint(startAngle + centerAngle, innerRadius), size);
			var innerArtEndPoint = GeometryUtils.CalcDisplayPoint(GeometryUtils.CalcCartesianPoint(startAngle, innerRadius), size);

			_annulusFigure.StartPoint = outerArcStartPoint;

			_outerArc.Point = outerArcEndPoint;
			_outerArc.IsLargeArc = isLargeArc;
			_outerArc.Size = new Size(outerRadius, outerRadius);

			_innerArc.Point = innerArtEndPoint;
			_innerArc.IsLargeArc = isLargeArc;
			_innerArc.Size = new Size(innerRadius, innerRadius);

			_innerArcStartPointSegment.Point = innerArcStartPoint;
			_outerArcStartSegment.Point = outerArcStartPoint;
		}
	}
}