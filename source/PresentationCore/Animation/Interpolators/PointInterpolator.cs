// <copyright file="PointInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class PointInterpolator : PrimitiveInterpolator<Point>
	{
		public static readonly PointInterpolator Instance = new();

		private PointInterpolator()
		{
		}

		protected internal override Point EvaluateCore(Point start, Point end, double progress)
		{
			var doubleInterpolator = DoubleInterpolator.Instance;

			var mx = doubleInterpolator.EvaluateCore(start.X, end.X, progress);
			var my = doubleInterpolator.EvaluateCore(start.Y, end.Y, progress);

			return new Point(mx, my);
		}
	}
}