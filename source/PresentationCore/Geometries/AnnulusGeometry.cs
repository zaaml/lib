// <copyright file="AnnulusGeometry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Geometries
{
	internal readonly struct AnnulusGeometry
	{
		private readonly EllipseGeometry _innerCircle;
		private readonly EllipseGeometry _outerCircle;

		public AnnulusGeometry()
		{
			_innerCircle = new EllipseGeometry();
			_outerCircle = new EllipseGeometry();

			Geometry = new CombinedGeometry(GeometryCombineMode.Exclude, _outerCircle, _innerCircle);
		}

		public Geometry Geometry { get; }

		public void Update(double innerRadius, double outerRadius)
		{
			var radius1 = innerRadius;
			var radius2 = outerRadius;

			outerRadius = Math.Max(radius1, radius2);
			innerRadius = Math.Min(radius1, radius2);

			var size = new Size(2 * outerRadius, 2 * outerRadius);

			_innerCircle.Center = GeometryUtils.CalcDisplayPoint(new Point(0, 0), size);
			_innerCircle.RadiusY = innerRadius;
			_innerCircle.RadiusX = innerRadius;


			_outerCircle.Center = GeometryUtils.CalcDisplayPoint(new Point(0, 0), size);
			_outerCircle.RadiusY = outerRadius;
			_outerCircle.RadiusX = outerRadius;
		}
	}
}