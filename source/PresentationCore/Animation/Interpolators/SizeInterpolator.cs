// <copyright file="SizeInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class SizeInterpolator : PrimitiveInterpolator<Size>
	{
		public static readonly SizeInterpolator Instance = new();

		private SizeInterpolator()
		{
		}

		protected internal override Size EvaluateCore(Size start, Size end, double progress)
		{
			var doubleInterpolator = DoubleInterpolator.Instance;

			var mw = doubleInterpolator.EvaluateCore(start.Width, end.Width, progress);
			var mh = doubleInterpolator.EvaluateCore(start.Height, end.Height, progress);

			return new Size(mw, mh);
		}
	}
}