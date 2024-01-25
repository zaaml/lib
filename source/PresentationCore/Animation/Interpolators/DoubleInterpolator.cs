// <copyright file="DoubleInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public sealed class DoubleInterpolator : PrimitiveInterpolator<double>
	{
		public static readonly DoubleInterpolator Instance = new();

		private DoubleInterpolator()
		{
		}

		protected internal override double EvaluateCore(double start, double end, double progress)
		{
			return start + (end - start) * progress;
		}
	}
}