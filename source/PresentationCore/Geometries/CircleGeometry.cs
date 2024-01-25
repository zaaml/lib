// <copyright file="CircleGeometry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Geometries
{
	internal readonly struct CircleGeometry
	{
		private readonly EllipseGeometry _ellipseGeometry;

		public CircleGeometry()
		{
			_ellipseGeometry = new EllipseGeometry();
		}

		public Geometry Geometry => _ellipseGeometry;

		public void Update(double radius)
		{
			var size = new Size(2 * radius, 2 * radius);

			_ellipseGeometry.Center = GeometryUtils.CalcDisplayPoint(new Point(0, 0), size);
			_ellipseGeometry.RadiusX = radius;
			_ellipseGeometry.RadiusY = radius;
		}
	}
}