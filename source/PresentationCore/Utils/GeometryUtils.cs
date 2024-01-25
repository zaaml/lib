// <copyright file="GeometryUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
	internal static class GeometryUtils
	{
		public static Point CalcCartesianPoint(double angle, double radius)
		{
			var angleRad = angle * Math.PI / 180.0;

			var x = radius * Math.Cos(angleRad);
			var y = radius * Math.Sin(angleRad);

			return new Point(x, y);
		}

		public static double CalcCenterAngle(ref double startAngle, ref double endAngle)
		{
			if (startAngle > endAngle)
				(startAngle, endAngle) = (endAngle, startAngle);

			if (endAngle - startAngle >= 360.0)
				return 360.0;

			startAngle %= 360;
			endAngle %= 360;

			if (startAngle < 0)
				startAngle += 360;

			if (endAngle < 0)
				endAngle += 360;

			if (startAngle > endAngle)
				endAngle += 360.0;

			return endAngle - startAngle;
		}

		public static Point CalcDisplayPoint(Point cartesianPoint, Size displaySize)
		{
			return new Point(cartesianPoint.X + displaySize.Width / 2, displaySize.Height / 2 - cartesianPoint.Y);
		}

		public static Point GetBezierPoint(double t, Point p0, Point p1, Point p2, Point p3)
		{
			var x = ((1 - t) * (1 - t) * (1 - t) * p0.X) + (3 * (1 - t) * (1 - t) * t * p1.X) +
			        (3 * (1 - t) * t * t * p2.X) + (t * t * t * p3.X);
			var y = ((1 - t) * (1 - t) * (1 - t) * p0.Y) + (3 * (1 - t) * (1 - t) * t * p1.Y) +
			        (3 * (1 - t) * t * t * p2.Y) + (t * t * t * p3.Y);

			return new Point(x, y);
		}

		public static void GetBezierPoints(Point beginPoint, Point endPoint, out Point p1, out Point p2, out Point p3, out Point p4)
		{
			var sourcePoint = beginPoint;
			var targetPoint = endPoint;

			var calcArrangeRect = new Rect(sourcePoint, targetPoint);
			var pathRect = new Rect(calcArrangeRect.Size);

			var t = sourcePoint.Y < targetPoint.Y ? pathRect.Top : pathRect.Bottom;
			var b = sourcePoint.Y > targetPoint.Y ? pathRect.Top : pathRect.Bottom;
			var l = sourcePoint.X < targetPoint.X ? pathRect.Left : pathRect.Right;
			var r = sourcePoint.X > targetPoint.X ? pathRect.Left : pathRect.Right;

			var tl = new Point(l, t);
			var br = new Point(r, b);

			p1 = tl;
			p2 = new Point(tl.X + (pathRect.Width / 2), tl.Y);
			p3 = new Point(br.X - (pathRect.Width / 2), br.Y);
			p4 = br;
		}

		public static double GetClosestPointToCubicBezier(int iterations, double start, double end, int slices, Point f, Point p0, Point p1, Point p2, Point p3)
		{
			while (true)
			{
				if (iterations <= 0)
					return (start + end) / 2;

				var tick = (end - start) / slices;
				var best = 0.0;
				var bestDistance = double.PositiveInfinity;
				var t = start;

				while (t <= end)
				{
					var p = GetBezierPoint(t, p0, p1, p2, p3);

					var dx = p.X - f.X;
					var dy = p.Y - f.Y;
					dx *= dx;
					dy *= dy;
					var currentDistance = dx + dy;
					if (currentDistance < bestDistance)
					{
						bestDistance = currentDistance;
						best = t;
					}

					t += tick;
				}

				iterations--;
				start = Math.Max(best - tick, 0d);
				end = Math.Min(best + tick, 1d);
			}
		}
	}
}