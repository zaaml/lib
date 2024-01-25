// <copyright file="RectInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class RectInterpolator : PrimitiveInterpolator<Rect>
	{
		public static readonly RectInterpolator Instance = new();

		private RectInterpolator()
		{
		}

		protected internal override Rect EvaluateCore(Rect start, Rect end, double progress)
		{
			var doubleInterpolator = DoubleInterpolator.Instance;

			var mx = doubleInterpolator.EvaluateCore(start.X, end.X, progress);
			var my = doubleInterpolator.EvaluateCore(start.Y, end.Y, progress);
			var mw = doubleInterpolator.EvaluateCore(start.Width, end.Width, progress);
			var mh = doubleInterpolator.EvaluateCore(start.Height, end.Height, progress);

			return new Rect(mx, my, mw, mh);
		}
	}
}